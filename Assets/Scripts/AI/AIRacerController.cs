using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AIRacerController : MonoBehaviour
{
    public AIWaypointFollower waypointFollower;
    public Transform visualBody;
    public Rigidbody rb;

    [Header("Tuning")]
    public float acceleration = 26f;
    public float maxSpeed = 32f;
    public float brakeForce = 35f;
    public float turnSpeed = 55f;
    public float leanAngle = 20f;
    public float airControl = 0.4f;

    public bool CanControl { get; set; } = false;

    private bool _grounded;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _grounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 1.2f);
        if (!CanControl || waypointFollower == null)
        {
            return;
        }

        ApplySteering();
        ApplyMovement();
        ApplyLean();
    }

    private void ApplySteering()
    {
        Vector3 desiredDir = (waypointFollower.target.position - transform.position).normalized;
        Vector3 currentDir = transform.forward;
        float steerDir = Vector3.SignedAngle(currentDir, desiredDir, Vector3.up);
        float steerInput = Mathf.Clamp(steerDir / 45f, -1f, 1f);

        float speedFactor = Mathf.InverseLerp(0f, maxSpeed, rb.velocity.magnitude);
        float turnAmount = steerInput * turnSpeed * Mathf.Lerp(0.6f, 1f, speedFactor) * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnAmount, 0f));
    }

    private void ApplyMovement()
    {
        float targetSpeed = waypointFollower != null ? waypointFollower.targetSpeed : maxSpeed * 0.6f;
        float speedNoise = Random.Range(-1.5f, 1.5f);
        float desiredSpeed = Mathf.Clamp(targetSpeed + speedNoise, 8f, maxSpeed);

        float currentForwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
        float throttle = desiredSpeed > currentForwardSpeed ? 1f : 0.2f;
        rb.AddForce(transform.forward * throttle * acceleration, ForceMode.Acceleration);

        if (desiredSpeed < currentForwardSpeed)
        {
            rb.AddForce(-transform.forward * brakeForce * 0.5f, ForceMode.Acceleration);
        }

        if (!_grounded)
        {
            rb.AddTorque(new Vector3(-throttle, steerNoise(), 0f) * airControl, ForceMode.Acceleration);
        }
    }

    private float steerNoise()
    {
        return Random.Range(-0.2f, 0.2f);
    }

    private void ApplyLean()
    {
        if (visualBody == null) return;
        float targetLean = -Vector3.SignedAngle(transform.forward, waypointFollower.transform.forward, Vector3.up) / 90f;
        targetLean = Mathf.Clamp(targetLean * leanAngle, -leanAngle, leanAngle);
        visualBody.localRotation = Quaternion.Slerp(visualBody.localRotation, Quaternion.Euler(0f, 0f, targetLean), Time.fixedDeltaTime * 4f);
    }
}
