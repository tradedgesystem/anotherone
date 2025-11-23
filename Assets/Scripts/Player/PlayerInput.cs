using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float Throttle { get; private set; }
    public float Steer { get; private set; }
    public bool IsBraking { get; private set; }
    public bool IsBoosting { get; private set; }

    private float _boostCooldown = 2f;
    private float _lastBoostTime = -10f;

    private void Update()
    {
        Throttle = Input.GetAxis("Vertical");
        Steer = Input.GetAxis("Horizontal");
        IsBraking = Input.GetKey(KeyCode.Space);

        bool boostKey = Input.GetKey(KeyCode.LeftShift);
        if (boostKey && Time.time - _lastBoostTime > _boostCooldown)
        {
            IsBoosting = true;
            _lastBoostTime = Time.time;
        }
        else
        {
            IsBoosting = false;
        }
    }
}
