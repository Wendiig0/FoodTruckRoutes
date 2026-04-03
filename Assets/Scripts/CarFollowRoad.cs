using System;
using UnityEngine;

public class CarFollowRoad : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;
    public float waypointReachDistance = 0.2f;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    void Update()
    {
        if (!isMoving || waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];

        MoveToWaypoint(target);
        RotateToWaypoint(target);
        CheckWaypointReached(target);
    }

    void MoveToWaypoint(Transform target)
    {
        // Move towards the target waypoint
        transform.position = Vector3.MoveTowards(
            transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    void RotateToWaypoint(Transform target)
    {
        // Smoothly rotate towards the target waypoint
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void CheckWaypointReached(Transform target)
    {
        // Check if the waypoint is reached
        if (Vector3.Distance(transform.position, target.position) <= waypointReachDistance)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                isMoving = false;
            }
        }
    }

    // Call this to start the car moving (e.g., from LevelManager on level start)
    public void StartMoving()
    {
        currentWaypointIndex = 0;
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
