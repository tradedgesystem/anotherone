using System;
using System.Collections.Generic;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 5;
    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    public Action<string, int> OnLapCompleted;
    public Action<string> OnRaceFinished;

    private class RacerProgress
    {
        public Transform transform;
        public int currentLap = 1;
        public int lastCheckpoint = -1;
        public float raceStartTime;
        public float lapStartTime;
        public float bestLap = Mathf.Infinity;
        public bool finished;
    }

    private readonly Dictionary<string, RacerProgress> _progress = new Dictionary<string, RacerProgress>();
    private bool _raceActive;

    public void BindCheckpoints()
    {
        foreach (var cp in checkpoints)
        {
            if (cp != null)
            {
                cp.lapManager = this;
            }
        }
    }

    public void RegisterRacer(string racerId, Transform racerTransform)
    {
        if (string.IsNullOrEmpty(racerId) || racerTransform == null)
        {
            Debug.LogWarning("Cannot register racer without id or transform.");
            return;
        }
        if (_progress.ContainsKey(racerId)) return;
        _progress[racerId] = new RacerProgress
        {
            transform = racerTransform,
            currentLap = 1,
            lastCheckpoint = checkpoints.Count - 1 // allow start line crossing to count as first checkpoint
        };
    }

    public void StartRace()
    {
        _raceActive = true;
        float time = Time.time;
        foreach (var entry in _progress.Values)
        {
            entry.raceStartTime = time;
            entry.lapStartTime = time;
        }
    }

    public void ProcessCheckpoint(string racerId, int checkpointIndex)
    {
        if (!_raceActive || !_progress.ContainsKey(racerId)) return;
        var data = _progress[racerId];
        if (data.finished) return;

        int expected = (data.lastCheckpoint + 1) % checkpoints.Count;
        if (checkpointIndex != expected) return;

        data.lastCheckpoint = checkpointIndex;

        if (checkpointIndex == 0)
        {
            CompleteLap(racerId, data);
        }
    }

    private void CompleteLap(string racerId, RacerProgress data)
    {
        float lapTime = Time.time - data.lapStartTime;
        data.bestLap = Mathf.Min(data.bestLap, lapTime);
        data.lapStartTime = Time.time;
        data.currentLap++;
        OnLapCompleted?.Invoke(racerId, data.currentLap - 1);

        if (data.currentLap > totalLaps)
        {
            data.finished = true;
            OnRaceFinished?.Invoke(racerId);
        }
    }

    public int GetLapForRacer(string racerId)
    {
        return _progress.TryGetValue(racerId, out var data) ? data.currentLap : 1;
    }

    public float GetCurrentLapTime(string racerId)
    {
        return _progress.TryGetValue(racerId, out var data) ? Time.time - data.lapStartTime : 0f;
    }

    public float GetBestLap(string racerId)
    {
        if (_progress.TryGetValue(racerId, out var data))
        {
            return data.bestLap == Mathf.Infinity ? 0f : data.bestLap;
        }
        return 0f;
    }

    public int GetRacePosition(string racerId)
    {
        if (!_progress.ContainsKey(racerId)) return _progress.Count;
        var ordered = new List<KeyValuePair<string, RacerProgress>>(_progress);
        ordered.Sort((a, b) => CompareProgress(b.Value, a.Value));
        for (int i = 0; i < ordered.Count; i++)
        {
            if (ordered[i].Key == racerId) return i + 1;
        }
        return ordered.Count;
    }

    private int CompareProgress(RacerProgress a, RacerProgress b)
    {
        if (a.finished && !b.finished) return 1;
        if (!a.finished && b.finished) return -1;
        if (a.currentLap != b.currentLap) return a.currentLap.CompareTo(b.currentLap);
        if (a.lastCheckpoint != b.lastCheckpoint) return a.lastCheckpoint.CompareTo(b.lastCheckpoint);

        Transform nextA = GetNextCheckpointTransform(a.lastCheckpoint);
        Transform nextB = GetNextCheckpointTransform(b.lastCheckpoint);
        float distA = nextA != null ? Vector3.Distance(a.transform.position, nextA.position) : float.MaxValue;
        float distB = nextB != null ? Vector3.Distance(b.transform.position, nextB.position) : float.MaxValue;
        return -distA.CompareTo(distB); // closer to next checkpoint ranks ahead
    }

    private Transform GetNextCheckpointTransform(int lastIndex)
    {
        if (checkpoints.Count == 0) return null;
        int next = (lastIndex + 1) % checkpoints.Count;
        return checkpoints[next]?.transform;
    }
}
