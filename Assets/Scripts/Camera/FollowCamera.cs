using UnityEngine;

namespace DirtTrackRacer.CameraSystem
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);
        [SerializeField] private float followSpeed = 6f;
        [SerializeField] private float lookSpeed = 8f;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPos = target.position + target.rotation * offset;
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSpeed);

            Quaternion lookRot = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * lookSpeed);
        }
    }
}
