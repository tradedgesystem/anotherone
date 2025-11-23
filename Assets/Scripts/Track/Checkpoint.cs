using UnityEngine;

namespace DirtTrackRacer.Track
{
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour
    {
        public int OrderIndex;
        [SerializeField] private LapManager lapManager;

        private void Awake()
        {
            if (lapManager == null)
            {
                lapManager = FindObjectOfType<LapManager>();
            }

            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var controller = other.GetComponentInParent<DirtTrackRacer.Player.MotorcycleController>();
            if (controller == null || lapManager == null) return;

            lapManager.ProcessCheckpoint(this, controller);
        }
    }
}
