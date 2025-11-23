using UnityEngine;

namespace DirtTrackRacer.AI
{
    [RequireComponent(typeof(Player.MotorcycleController))]
    public class AIWaypointFollower : MonoBehaviour
    {
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private float targetReachThreshold = 5f;
        [SerializeField] private float lookAheadDistance = 10f;
        [SerializeField] private float speedBias = 0.7f;

        private int currentIndex;
        private Player.MotorcycleController controller;
        private Rigidbody rb;

        private void Awake()
        {
            controller = GetComponent<Player.MotorcycleController>();
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            Vector3 target = waypoints[currentIndex].position;
            Vector3 toTarget = target - transform.position;
            Vector3 flatDirection = Vector3.ProjectOnPlane(toTarget, Vector3.up).normalized;

            float steer = Vector3.SignedAngle(transform.forward, flatDirection, Vector3.up) / 45f;
            steer = Mathf.Clamp(steer, -1f, 1f);

            float desiredSpeed = Mathf.Clamp01(toTarget.magnitude / lookAheadDistance);
            float throttle = Mathf.Lerp(speedBias, 1f, desiredSpeed);
            float brake = Mathf.Clamp01(-Vector3.Dot(transform.forward, rb.velocity.normalized));

            controller.SetInputs(throttle, brake, steer);

            if (toTarget.magnitude <= targetReachThreshold)
            {
                currentIndex = (currentIndex + 1) % waypoints.Length;
            }
        }
    }
}
