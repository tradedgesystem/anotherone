using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index;
    public LapManager lapManager;

    private void OnTriggerEnter(Collider other)
    {
        var racer = other.GetComponentInParent<RacerIdentifier>();
        if (racer == null || lapManager == null) return;
        lapManager.ProcessCheckpoint(racer.RacerId, index);
    }
}
