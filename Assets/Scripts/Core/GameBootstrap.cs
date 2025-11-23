using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    private TrackBuilder _trackBuilder;
    private LapManager _lapManager;
    private RaceHUD _hud;
    private CountdownController _countdown;

    private readonly List<RacerIdentifier> _racers = new List<RacerIdentifier>();
    private MotorcycleController _playerController;
    private readonly List<AIRacerController> _aiControllers = new List<AIRacerController>();

    private const int DefaultTotalLaps = 5;

    private void Awake()
    {
        if (FindObjectsOfType<GameBootstrap>().Length > 1)
        {
            Debug.LogWarning("Multiple GameBootstrap instances detected. Keeping the first one.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        BuildTrack();
        SpawnRacers();
        SetupCamera();
        SetupHUD();
        SetupLapManager();
        SetupCountdown();
    }

    private void BuildTrack()
    {
        var trackObject = new GameObject("TrackBuilder");
        _trackBuilder = trackObject.AddComponent<TrackBuilder>();
        _trackBuilder.totalLaps = DefaultTotalLaps;
        _trackBuilder.BuildTrack();
    }

    private void SpawnRacers()
    {
        if (_trackBuilder == null || _trackBuilder.waypoints == null || _trackBuilder.waypoints.Count == 0)
        {
            Debug.LogError("TrackBuilder missing or has no waypoints. Cannot spawn racers.");
            return;
        }

        Vector3 startPos = _trackBuilder.waypoints[0];
        Vector3 forward = _trackBuilder.StartForward;
        float laneOffset = _trackBuilder.trackWidth * 0.3f;
        Vector3 laneRight = Vector3.Cross(Vector3.up, forward).normalized;

        var playerObj = CreateBike("Player", startPos - laneRight * laneOffset, forward, true);
        _playerController = playerObj.GetComponent<MotorcycleController>();

        var ai1 = CreateBike("AI Rider 1", startPos, forward, false);
        var ai2 = CreateBike("AI Rider 2", startPos + laneRight * laneOffset, forward, false);

        SetupAI(ai1.GetComponent<AIRacerController>());
        SetupAI(ai2.GetComponent<AIRacerController>());
    }

    private GameObject CreateBike(string name, Vector3 position, Vector3 forward, bool isPlayer)
    {
        GameObject bike = new GameObject(name);
        bike.transform.position = position + Vector3.up * 1f;
        bike.transform.forward = forward;

        var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.SetParent(bike.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.8f, 0.6f, 2.2f);

        var rb = bike.AddComponent<Rigidbody>();
        rb.mass = 180f;
        rb.drag = 0.5f;
        rb.angularDrag = 1.5f;

        var collider = bike.AddComponent<CapsuleCollider>();
        collider.height = 2.0f;
        collider.radius = 0.5f;
        collider.center = new Vector3(0f, 0.5f, 0f);

        var identifier = bike.AddComponent<RacerIdentifier>();
        identifier.RacerId = name;
        _racers.Add(identifier);

        if (isPlayer)
        {
            var input = bike.AddComponent<PlayerInput>();
            var controller = bike.AddComponent<MotorcycleController>();
            controller.input = input;
            controller.visualBody = body.transform;
        }
        else
        {
            var aiController = bike.AddComponent<AIRacerController>();
            aiController.visualBody = body.transform;
            _aiControllers.Add(aiController);
        }

        return bike;
    }

    private void SetupAI(AIRacerController aiController)
    {
        if (aiController == null)
        {
            Debug.LogWarning("AI controller missing; cannot configure AI racer.");
            return;
        }

        var followerObj = new GameObject("AIWaypointFollower");
        followerObj.transform.position = _trackBuilder.waypoints[0];
        var follower = followerObj.AddComponent<AIWaypointFollower>();
        follower.waypoints = _trackBuilder.waypoints;
        follower.targetSpeed = Random.Range(18f, 24f);
        follower.turnResponsiveness = 4f;
        follower.target = new GameObject(aiController.name + "_PathTarget").transform;
        follower.target.position = _trackBuilder.waypoints[0];

        aiController.waypointFollower = follower;
        aiController.rb = aiController.GetComponent<Rigidbody>();
    }

    private void SetupCamera()
    {
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        var camera = camObj.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.Skybox;

        var follow = camObj.AddComponent<CameraFollow>();
        follow.target = _playerController != null ? _playerController.transform : null;
        follow.offset = new Vector3(0, 5f, -8f);
        follow.followSmoothness = 6f;
        follow.lookAtHeightOffset = 1.2f;
    }

    private void SetupHUD()
    {
        GameObject hudObj = new GameObject("RaceHUD");
        _hud = hudObj.AddComponent<RaceHUD>();
        _hud.SetLapInfo(1, _trackBuilder != null ? _trackBuilder.totalLaps : DefaultTotalLaps);
    }

    private void SetupLapManager()
    {
        GameObject lapObj = new GameObject("LapManager");
        _lapManager = lapObj.AddComponent<LapManager>();
        _lapManager.totalLaps = _trackBuilder != null ? _trackBuilder.totalLaps : DefaultTotalLaps;
        _lapManager.checkpoints = _trackBuilder != null ? _trackBuilder.checkpoints : new List<Checkpoint>();
        _lapManager.BindCheckpoints();

        foreach (var racer in _racers)
        {
            _lapManager.RegisterRacer(racer.RacerId, racer.transform);
        }

        _lapManager.OnLapCompleted += HandleLapCompleted;
        _lapManager.OnRaceFinished += HandleRaceFinished;
    }

    private void SetupCountdown()
    {
        GameObject countdownObj = new GameObject("CountdownController");
        _countdown = countdownObj.AddComponent<CountdownController>();
        _countdown.Init(3, EnableRaceControls, UpdateCountdownText);

        DisableAllControls();
    }

    private void DisableAllControls()
    {
        if (_playerController != null) _playerController.CanControl = false;
        foreach (var ai in _aiControllers)
        {
            ai.CanControl = false;
        }
    }

    private void EnableRaceControls()
    {
        if (_playerController != null) _playerController.CanControl = true;
        foreach (var racer in _racers)
        {
            var ai = racer.GetComponent<AIRacerController>();
            if (ai != null) ai.CanControl = true;
        }

        _lapManager?.StartRace();
    }

    private void UpdateCountdownText(string text)
    {
        _hud?.ShowCountdownText(text);
    }

    private void HandleLapCompleted(string racerId, int lap)
    {
        if (_hud == null || _lapManager == null || _playerController == null) return;

        var playerId = _playerController.GetComponent<RacerIdentifier>().RacerId;
        if (racerId == playerId)
        {
            _hud.SetLapInfo(_lapManager.GetLapForRacer(playerId), _lapManager.totalLaps);
            _hud.SetLapTimes(_lapManager.GetCurrentLapTime(playerId), _lapManager.GetBestLap(playerId));
        }
    }

    private void HandleRaceFinished(string racerId)
    {
        if (_playerController == null || _lapManager == null || _hud == null) return;
        var playerId = _playerController.GetComponent<RacerIdentifier>().RacerId;
        if (racerId != playerId) return;

        int position = _lapManager.GetRacePosition(playerId);
        _hud.ShowRaceFinished($"Finished P{position}! Press R to restart, Esc to quit.");
    }

    private void Update()
    {
        if (_playerController == null || _lapManager == null || _hud == null) return;

        string playerId = _playerController.GetComponent<RacerIdentifier>().RacerId;
        int position = _lapManager.GetRacePosition(playerId);
        _hud.SetPosition(position, _racers.Count);
        _hud.SetLapTimes(_lapManager.GetCurrentLapTime(playerId), _lapManager.GetBestLap(playerId));

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
