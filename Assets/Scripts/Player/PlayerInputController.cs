using UnityEngine;

namespace DirtTrackRacer.Player
{
    [RequireComponent(typeof(MotorcycleController))]
    public class PlayerInputController : MonoBehaviour
    {
        private MotorcycleController controller;

        private void Awake()
        {
            controller = GetComponent<MotorcycleController>();
        }

        private void Update()
        {
            float throttle = Mathf.Clamp01(Input.GetAxis("Vertical"));
            float steer = Input.GetAxis("Horizontal");
            float brake = Input.GetKey(KeyCode.Space) ? 1f : 0f;

            controller.SetInputs(throttle, brake, steer);

            if (Input.GetKeyDown(KeyCode.R))
            {
                controller.ResetToCheckpoint();
            }
        }
    }
}
