﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;

namespace GameAIPrototypes.SimpleNavMesh
{
    public class GoalBehaviour : WaypointCycler
    {

        // Update is called once per frame
        private void Update()
        {
            // Rotate goal
            transform.Rotate(Vector3.one * 100 * Time.deltaTime);
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
}