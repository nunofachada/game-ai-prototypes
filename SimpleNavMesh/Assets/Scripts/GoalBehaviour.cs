using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBehaviour : WaypointCycler
{

    // Update is called once per frame
    private void Update()
    {
        // Rotate goal
        transform.Rotate(new Vector3(1f, 1f, 1f));
    }

    // If nav agent collides with goal, move goal to next waypoint
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "NavAgent")
        {
            NextWaypoint();
            transform.position = CurrentWaypoint;
        }
    }
}
