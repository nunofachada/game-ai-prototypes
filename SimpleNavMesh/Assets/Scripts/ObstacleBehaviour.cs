/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;

public class ObstacleBehaviour : WaypointCycler
{

    // Speed of obstacle movement between waypoints
    [SerializeField] [Range(0f, 20f)]
    private float speed = 10f;

    // Minimum distance to waypoint to trigger a new waypoint
    [SerializeField]
    private float minWaypointDist = 0.5f;

    // Reference to the agent's rigid body
    private Rigidbody rb;

    // Get reference to the agent's rigid body
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Move obstacle agent between waypoints
    private void FixedUpdate()
    {
        Vector3 vel;

        // Are we close enough to the current waypoint?
        if ((CurrentWaypoint - transform.position).magnitude < minWaypointDist)
        {
            // If so, get a new waypoint
            NextWaypoint();
        }

        // Determine velocity to the current waypoint
        vel = (CurrentWaypoint - transform.position).normalized * speed;

        // Move towards the next waypoint at the calculated velocity
        rb.MovePosition(transform.position + vel * Time.fixedDeltaTime);
    }
}
