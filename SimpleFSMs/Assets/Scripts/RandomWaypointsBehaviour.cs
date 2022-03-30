/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

// Agents with this component move between random waypoints
public class RandomWaypointsBehaviour : MonoBehaviour
{
    // Maximum speed
    [SerializeField] private float maxSpeed = 8f;

    // Plane walls
    private PlaneWalls planeWalls;

    // Current waypoint
    private Vector3 waypoint;

    // Get reference to plane walls
    private void Awake()
    {
        planeWalls = GameObject.Find("Plane").GetComponent<PlaneWalls>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        waypoint = transform.position;
    }

    // Move between random waypoints
    private void Update()
    {
        // Determine a new random waypoint?
        if ((waypoint - transform.position).magnitude < 0.1f * maxSpeed)
        {
            // Determine plane limits
            Vector4 planeLimits = planeWalls.Limits;
            // Determine a new random waypoint
            waypoint = new Vector3(
                Random.Range(planeLimits.y, planeLimits.x),
                transform.position.y,
                Random.Range(planeLimits.w, planeLimits.z));
        }

        // Get velocity from the vector form of the orientation
        Vector3 linear = (waypoint - transform.position).normalized
            * maxSpeed * Time.deltaTime;

        // Apply steering
        transform.Translate(linear, Space.World);
    }

    // Draw waypoint
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(waypoint, 0.4f);
    }
}
