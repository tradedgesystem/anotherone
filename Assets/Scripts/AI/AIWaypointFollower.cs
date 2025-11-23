using System.Collections.Generic;
using UnityEngine;

public class AIWaypointFollower : MonoBehaviour
{
    public Transform target;
    public List<Vector3> waypoints = new List<Vector3>();
    public float targetSpeed = 20f;
    public float turnResponsiveness = 5f;

    private int _currentIndex;

    private void Start()
    {
        if (target == null)
        {
            var t = new GameObject(name + "_Target").transform;
            target = t;
        }
        if (waypoints.Count > 0)
        {
            target.position = waypoints[0];
        }
    }

    private void Update()
    {
        if (target == null || waypoints.Count == 0) return;
        Vector3 current = waypoints[_currentIndex];
        Vector3 to = (current - target.position);
        Vector3 flat = new Vector3(to.x, 0f, to.z);
        if (flat.magnitude < 3f)
        {
            _currentIndex = (_currentIndex + 1) % waypoints.Count;
            current = waypoints[_currentIndex];
            flat = new Vector3(current.x - target.position.x, 0f, current.z - target.position.z);
        }

        Vector3 desiredDir = flat.normalized;
        Vector3 smoothedDir = Vector3.Slerp(target.forward, desiredDir, Time.deltaTime * turnResponsiveness);
        target.rotation = Quaternion.LookRotation(smoothedDir, Vector3.up);
        target.position += target.forward * targetSpeed * Time.deltaTime;
    }
}
