using UnityEngine;

/// <summary>
/// DEBUG ONLY - Remove this script before releasing the game.
/// Attach to any GameObject in the scene.
/// </summary>
public class DebugCarController : MonoBehaviour
{
    [Header("Debug Controls")]
    public CarFollowRoad car;

    void Update()
    {
        // Space = Start moving
        if (Input.GetKeyDown(KeyCode.Space))
        {
            car.StartMoving();
            Debug.Log("[DEBUG] Car started moving.");
        }

        // S = Stop moving
        if (Input.GetKeyDown(KeyCode.S))
        {
            car.StopMoving();
            Debug.Log("[DEBUG] Car stopped.");
        }

        // R = Reset car to first waypoint position
        if (Input.GetKeyDown(KeyCode.R))
        {
            car.StopMoving();
            if (car.waypoints != null && car.waypoints.Length > 0)
            {
                car.transform.position = car.waypoints[0].position;
                Debug.Log("[DEBUG] Car reset to start.");
            }

            // Small delay then restart so you can re-test quickly
            car.StartMoving();
        }
    }

    void OnGUI()
    {
        // Show controls on screen during Play mode
        GUI.color = Color.yellow;
        GUI.Label(new Rect(10, 10, 300, 20), "[DEBUG] SPACE = Start  |  S = Stop  |  R = Reset & Restart");
    }
}