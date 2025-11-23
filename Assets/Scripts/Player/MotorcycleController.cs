using UnityEngine;

namespace DirtTrackRacer.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class MotorcycleController : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField] private float accelerationForce = 1600f;
        [SerializeField] private float brakeForce = 1800f;
        [SerializeField] private float maxSpeed = 30f;
        [SerializeField] private float turnTorque = 80f;
        [SerializeField] private float maxLeanAngle = 35f;
        [SerializeField] private float leanSpeed = 6f;

        [Header("Suspension")]
        [SerializeField] private Transform frontWheel;
        [SerializeField] private Transform rearWheel;
        [SerializeField] private float suspensionRestDistance = 0.5f;
        [SerializeField] private float suspensionStrength = 18000f;
        [SerializeField] private float suspensionDamping = 2200f;
        [SerializeField] private LayerMask groundMask = ~0;

        [Header("Visuals")]
        [SerializeField] private Transform leanTransform;

        private Rigidbody rb;
        private float throttleInput;
        private float brakeInput;
        private float steerInput;

        public Vector3 LastCheckpointPosition { get; set; }
        public Quaternion LastCheckpointRotation { get; set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = new Vector3(0f, -0.4f, 0f);
            LastCheckpointPosition = transform.position;
            LastCheckpointRotation = transform.rotation;
        }

        private void FixedUpdate()
        {
            ApplySuspension(frontWheel);
            ApplySuspension(rearWheel);

            ApplyDrive();
            ApplyTurning();
            UpdateLeanVisual();
        }

        private void ApplyDrive()
        {
            Vector3 forward = transform.forward;
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            float currentSpeed = flatVel.magnitude;

            if (throttleInput > 0f && currentSpeed < maxSpeed)
            {
                rb.AddForce(forward * (throttleInput * accelerationForce));
            }

            if (brakeInput > 0f)
            {
                Vector3 brake = -rb.velocity.normalized * (brakeInput * brakeForce);
                rb.AddForce(brake);
            }
        }

        private void ApplyTurning()
        {
            float targetLean = steerInput * maxLeanAngle;
            float currentLean = Vector3.SignedAngle(transform.up, Vector3.up, transform.forward);
            float leanDelta = Mathf.Clamp(targetLean - currentLean, -1f, 1f);
            rb.AddRelativeTorque(Vector3.forward * leanDelta * leanSpeed, ForceMode.Acceleration);

            rb.AddTorque(Vector3.up * steerInput * turnTorque, ForceMode.Acceleration);
        }

        private void ApplySuspension(Transform wheel)
        {
            if (wheel == null) return;

            RaycastHit hit;
            Vector3 origin = wheel.position;
            if (Physics.Raycast(origin, -transform.up, out hit, suspensionRestDistance + 0.5f, groundMask))
            {
                float distance = hit.distance;
                float compression = 1f - Mathf.Clamp01((distance - 0.05f) / suspensionRestDistance);
                float springForce = compression * suspensionStrength;

                Vector3 wheelVelocity = rb.GetPointVelocity(origin);
                float verticalVelocity = Vector3.Dot(transform.up, wheelVelocity);
                float damperForce = -verticalVelocity * suspensionDamping;

                Vector3 force = transform.up * (springForce + damperForce);
                rb.AddForceAtPosition(force, origin);

                wheel.position = hit.point + transform.up * 0.05f;
            }
            else
            {
                wheel.localPosition = new Vector3(wheel.localPosition.x, -suspensionRestDistance, wheel.localPosition.z);
            }
        }

        private void UpdateLeanVisual()
        {
            if (leanTransform == null) return;
            float targetLean = steerInput * maxLeanAngle;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, -targetLean);
            leanTransform.localRotation = Quaternion.Slerp(leanTransform.localRotation, targetRot, Time.deltaTime * leanSpeed);
        }

        public void SetInputs(float throttle, float brake, float steer)
        {
            throttleInput = Mathf.Clamp01(throttle);
            brakeInput = Mathf.Clamp01(brake);
            steerInput = Mathf.Clamp(steer, -1f, 1f);
        }

        public void ResetToCheckpoint()
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(LastCheckpointPosition, LastCheckpointRotation);
        }
    }
}
