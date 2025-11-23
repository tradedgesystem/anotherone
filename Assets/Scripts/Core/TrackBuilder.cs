using System.Collections.Generic;
using UnityEngine;

public class TrackBuilder : MonoBehaviour
{
    public int totalLaps = 5;
    public float trackWidth = 6f;
    public List<Vector3> waypoints = new List<Vector3>();
    public List<Checkpoint> checkpoints = new List<Checkpoint>();
    public Vector3 StartForward { get; private set; }

    private const float FloorSize = 80f;

    public void BuildTrack()
    {
        BuildFloor();
        BuildLayout();
        BuildCheckpoints();
    }

    private void BuildFloor()
    {
        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "DirtFloor";
        floor.transform.localScale = Vector3.one * (FloorSize / 10f); // Plane is 10 units by default
        floor.GetComponent<Renderer>().material.color = new Color(0.35f, 0.25f, 0.15f);
    }

    private void BuildLayout()
    {
        // Define waypoints for a clockwise indoor loop.
        waypoints = new List<Vector3>
        {
            new Vector3(0, 0, -10), // start straight
            new Vector3(0, 0, 20), // tabletop ramp region
            new Vector3(10, 0, 40),
            new Vector3(30, 0, 42), // bermed right
            new Vector3(46, 0, 28),
            new Vector3(48, 0, 6),
            new Vector3(40, 0, -12),
            new Vector3(20, 0, -24),
            new Vector3(-2, 0, -22),
            new Vector3(-20, 0, -12),
            new Vector3(-32, 0, 8),
            new Vector3(-30, 0, 28),
            new Vector3(-12, 0, 36),
            new Vector3(0, 0, 26)
        };
        StartForward = (waypoints[1] - waypoints[0]).normalized;

        // Build segments and features.
        for (int i = 0; i < waypoints.Count; i++)
        {
            Vector3 from = waypoints[i];
            Vector3 to = waypoints[(i + 1) % waypoints.Count];
            CreateFlatSegment(from, to, trackWidth * 0.9f);
        }

        CreateTabletop(new Vector3(0, 0, 12), new Vector3(0, 0, 22));
        CreateWhoops(new Vector3(46, 0, 26), Vector3.back, 6, 2.5f);
        CreateBerm(new Vector3(30, 0, 42), new Vector3(46, 0, 28), true);
        CreateBerm(new Vector3(-32, 0, 8), new Vector3(-30, 0, 28), false);
    }

    private void CreateFlatSegment(Vector3 from, Vector3 to, float width)
    {
        Vector3 mid = (from + to) * 0.5f;
        Vector3 dir = to - from;
        float length = dir.magnitude;
        if (length < 0.1f) return;
        var segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        segment.name = "Segment";
        segment.transform.position = mid + Vector3.down * 0.1f;
        segment.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        segment.transform.localScale = new Vector3(width, 0.5f, length);
        var renderer = segment.GetComponent<Renderer>();
        renderer.material.color = new Color(0.4f, 0.3f, 0.2f);
    }

    private void CreateTabletop(Vector3 start, Vector3 end)
    {
        Vector3 dir = (end - start).normalized;
        float length = Vector3.Distance(start, end);
        float rampLength = 4f;

        // Entry ramp
        CreateRamp(start - dir * rampLength, start, 0.5f);
        // Flat top
        var top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.name = "Tabletop";
        top.transform.position = (start + end) * 0.5f + Vector3.up * 1f;
        top.transform.rotation = Quaternion.LookRotation(dir);
        top.transform.localScale = new Vector3(trackWidth, 0.6f, length);
        top.GetComponent<Renderer>().material.color = new Color(0.45f, 0.32f, 0.22f);
        // Exit ramp
        CreateRamp(end, end + dir * rampLength, -0.5f);
    }

    private void CreateRamp(Vector3 start, Vector3 end, float heightDelta)
    {
        Vector3 dir = (end - start);
        float length = dir.magnitude;
        var ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ramp.name = "Ramp";
        ramp.transform.position = (start + end) * 0.5f + Vector3.up * (heightDelta * 0.5f + 0.25f);
        ramp.transform.rotation = Quaternion.LookRotation(dir.normalized) * Quaternion.Euler(heightDelta > 0 ? -10f : 10f, 0f, 0f);
        ramp.transform.localScale = new Vector3(trackWidth, 0.5f + Mathf.Abs(heightDelta), length);
        ramp.GetComponent<Renderer>().material.color = new Color(0.45f, 0.32f, 0.22f);
    }

    private void CreateWhoops(Vector3 start, Vector3 direction, int bumps, float spacing)
    {
        Vector3 dir = direction.normalized;
        for (int i = 0; i < bumps; i++)
        {
            var bump = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bump.name = "Whoop";
            Vector3 center = start + dir * spacing * i;
            bump.transform.position = center + Vector3.up * 0.6f;
            bump.transform.rotation = Quaternion.LookRotation(dir);
            bump.transform.localScale = new Vector3(trackWidth * 0.6f, 0.8f, spacing * 0.9f);
            bump.GetComponent<Renderer>().material.color = new Color(0.5f, 0.35f, 0.25f);
        }
    }

    private void CreateBerm(Vector3 from, Vector3 to, bool rightTurn)
    {
        Vector3 mid = (from + to) * 0.5f;
        Vector3 dir = (to - from).normalized;
        var berm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        berm.name = "Berm";
        berm.transform.position = mid + Vector3.up * 0.3f;
        berm.transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0f, 0f, rightTurn ? -20f : 20f);
        berm.transform.localScale = new Vector3(trackWidth * 1.2f, 1f, Vector3.Distance(from, to));
        berm.GetComponent<Renderer>().material.color = new Color(0.42f, 0.3f, 0.2f);
    }

    private void BuildCheckpoints()
    {
        checkpoints.Clear();
        for (int i = 0; i < waypoints.Count; i++)
        {
            GameObject cpObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cpObj.name = $"Checkpoint_{i}";
            cpObj.transform.position = waypoints[i] + Vector3.up * 0.25f;
            cpObj.transform.localScale = new Vector3(trackWidth, 1f, 1f);
            cpObj.transform.rotation = Quaternion.identity;
            var collider = cpObj.GetComponent<Collider>();
            collider.isTrigger = true;
            var cp = cpObj.AddComponent<Checkpoint>();
            cp.index = i;
            cpObj.GetComponent<Renderer>().enabled = false; // invisible helper
            checkpoints.Add(cp);
        }
    }
}
