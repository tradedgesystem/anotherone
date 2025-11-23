using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5f, -7f);
    public float followSmoothness = 5f;
    public float lookAtHeightOffset = 1.2f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSmoothness);

        Vector3 lookPoint = target.position + Vector3.up * lookAtHeightOffset;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPoint - transform.position), Time.deltaTime * followSmoothness);
    }
}
