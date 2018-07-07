using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongTrack : MonoBehaviour
{
    [SerializeField] private PlayerTrack track;
    [SerializeField] private float speed;

    private Transform playerCenter; // Player reference position

    private int currentWaypointIndex = -1;
    private Transform previousWayPoint, nextWayPoint;
    private Quaternion initialRotation, targetRotation; // Used to smoothly rotate player between waypoints

    public float SidewaysOffset { get; set; }

    public float SpeedModifier { get; set; } = 1f; // Used to move player forward or back for boosting/braking

    public event EventHandler OnReachedFinalWaypoint;

    void Start()
    {
        if (track.WayPoints.Count < 2)
        {
            Debug.LogError("Track is too short");
            enabled = false;
        }

        // Set the player reference transform to the starting position, make it face the first waypoint
        playerCenter = new GameObject("Player Position On Track").transform;
        playerCenter.position = track.WayPoints[0].position;
        playerCenter.rotation = Quaternion.LookRotation(track.WayPoints[1].position - playerCenter.position - Vector3.up * (track.WayPoints[1].position.y - playerCenter.position.y));
        WaypointReached();
    }

    void Update()
    {
        float tPosition = Mathf.Sqrt((playerCenter.position - previousWayPoint.position).sqrMagnitude /
                                     (nextWayPoint.position - previousWayPoint.position).sqrMagnitude); // Current value of t
        playerCenter.position += (nextWayPoint.position - playerCenter.position).normalized * speed * SpeedModifier * Time.deltaTime; // Move player reference position towards next waypoint at desired speed
        float currentWidth = track.GetWidth(currentWaypointIndex, tPosition);
        Vector3 playerPosition = playerCenter.position + playerCenter.right * SidewaysOffset * currentWidth / 2; // Determine player actual position from sideways offset and track width at current position

        playerCenter.rotation = Quaternion.Lerp(initialRotation, targetRotation, tPosition); // Smoothly rotate player towards next waypoint

        // Set player actual position and rotation
        transform.position = playerPosition;
        transform.rotation = playerCenter.rotation;

        if (tPosition >= 1f)
        {
            WaypointReached();
        }
    }

    private void WaypointReached()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex == track.WayPoints.Count - 1)
        {
            OnReachedFinalWaypoint?.Invoke(this, null);
            this.enabled = false;
            return;
        }

        previousWayPoint = track.WayPoints[currentWaypointIndex];
        nextWayPoint = track.WayPoints[currentWaypointIndex + 1];
        // Begin rotating towards the next waypoint
        targetRotation = Quaternion.LookRotation(new Vector3(nextWayPoint.position.x - playerCenter.position.x,
            0, nextWayPoint.position.z - playerCenter.position.z));
        initialRotation = playerCenter.rotation;
    }
}
