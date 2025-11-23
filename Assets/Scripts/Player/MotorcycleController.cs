using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MotorcycleController : MonoBehaviour
{
    [Header("Tuning")]
    public float acceleration = 30f;
    public float maxSpeed = 35f;
    public float brakeForce = 40f;
    public float turnSpeed = 60f;
    public float leanAngle = 25f;
    public float airControl = 0.5f;
    public float boostMultiplier = 1.4f;

    [Header("References")]
    public PlayerInput input;
    public Transform visualBody;
    public Rigidbody rb;

    public bool CanControl { get; set; } = true;

    private bool _grounded;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _grounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 1.2f);
        if (!CanControl || input == null)
        {
            return;
        }

        HandleMovement();
        HandleTurning();
        ApplyLean();
    }

    private void HandleMovement()
    {
        float throttle = Mathf.Clamp(input.Throttle, -1f, 1f);
        float boost = input.IsBoosting ? boostMultiplier : 1f;

        Vector3 forwardForce = transform.forward * (throttle * acceleration * boost);
        if (rb.velocity.magnitude < maxSpeed * boost || Vector3.Dot(rb.velocity, transform.forward) < 0)
        {
            rb.AddForce(forwardForce, ForceMode.Acceleration);
        }

        if (input.IsBraking || throttle < 0f)
        {
            rb.AddForce(-transform.forward * brakeForce, ForceMode.Acceleration);
        }

        if (!_grounded)
        {
            Vector3 torque = new Vector3(-input.Throttle, input.Steer, 0f) * airControl;
            rb.AddTorque(torque, ForceMode.Acceleration);
        }
    }

    private void HandleTurning()
    {
        float steer = Mathf.Clamp(input.Steer, -1f, 1f);
        float speedFactor = Mathf.InverseLerp(0f, maxSpeed, rb.velocity.magnitude);
        float turnAmount = steer * turnSpeed * Mathf.Lerp(0.5f, 1f, speedFactor) * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private void ApplyLean()
    {
        if (visualBody == null) return;
        float steer = Mathf.Clamp(input.Steer, -1f, 1f);
        float targetLean = -steer * leanAngle;
        Quaternion leanRot = Quaternion.Euler(0f, 0f, targetLean);
        visualBody.localRotation = Quaternion.Slerp(visualBody.localRotation, leanRot, Time.fixedDeltaTime * 5f);
    }
}
