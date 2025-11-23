using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DirtTrackRacer.Player;

namespace DirtTrackRacer.Track
{
    public class LapManager : MonoBehaviour
    {
        [SerializeField] private Text currentLapText;
        [SerializeField] private Text lastLapText;
        [SerializeField] private Text bestLapText;
        [SerializeField] private int totalLaps = 3;
        [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();
        [SerializeField] private MotorcycleController playerController;

        private int currentCheckpointIndex;
        private int currentLap = 1;
        private float lapStartTime;
        private float lastLapTime;
        private float bestLapTime = float.MaxValue;

        private void Start()
        {
            lapStartTime = Time.time;
            UpdateUI();
        }

        public void ProcessCheckpoint(Checkpoint checkpoint, MotorcycleController controller)
        {
            if (checkpoint == null || controller == null) return;
            if (checkpoint.OrderIndex != currentCheckpointIndex) return;

            controller.LastCheckpointPosition = checkpoint.transform.position + Vector3.up * 1f;
            controller.LastCheckpointRotation = checkpoint.transform.rotation;

            currentCheckpointIndex++;
            if (currentCheckpointIndex >= checkpoints.Count)
            {
                CompleteLap();
                currentCheckpointIndex = 0;
            }
        }

        private void CompleteLap()
        {
            float now = Time.time;
            lastLapTime = now - lapStartTime;
            lapStartTime = now;

            if (lastLapTime < bestLapTime)
            {
                bestLapTime = lastLapTime;
            }

            currentLap = Mathf.Min(currentLap + 1, totalLaps);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (currentLapText != null)
            {
                currentLapText.text = $"Lap {currentLap}/{totalLaps}";
            }

            if (lastLapText != null)
            {
                lastLapText.text = lastLapTime > 0f ? $"Last: {lastLapTime:F2}s" : "Last: --";
            }

            if (bestLapText != null)
            {
                bestLapText.text = bestLapTime < float.MaxValue ? $"Best: {bestLapTime:F2}s" : "Best: --";
            }
        }
    }
}
