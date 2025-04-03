/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using Unity.Behavior;
using System.Collections.Generic;

namespace GameAIPrototypes.BehaviorTrees
{
    /// <summary>
    /// Draws the AI-related gizmos, namely for waypoints and line to target.
    /// </summary>
    public class AIGizmos : MonoBehaviour
    {
        // References to the necessary blackboard variables
        private BlackboardVariable<List<Vector3>> bbWaypoints;
        private BlackboardVariable<bool> isPatrolling;
        private BlackboardVariable<Transform> player;

        private void Start()
        {
            // Get a reference to the blackboard
            BlackboardReference bbRef =
                GetComponent<BehaviorGraphAgent>().BlackboardReference;

            // Get references to the necessary blackboard variables
            // If some references don't exist, show error
            if (
                !bbRef.GetVariable("Waypoints", out bbWaypoints)
                ||
                !bbRef.GetVariable("IsPatrolling", out isPatrolling)
                ||
                !bbRef.GetVariable("Player", out player)
            ) Debug.LogError("Missing blackboard variables!");

        }

        // Draw gizmos!
        private void OnDrawGizmos()
        {
            // Get waypoints from the blackboard variable reference
            List<Vector3> waypoints = bbWaypoints.Value;

            // Draw blue wire spheres in waypoints
            Gizmos.color = Color.blue;
            foreach (Vector3 waypoint in waypoints)
            {
                Gizmos.DrawWireSphere(waypoint, 0.5f);
            }

            // If our AI agent is chasing the player, draw a yellow line
            // showing the pursuit
            if (!isPatrolling.Value)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, player.Value.position);
            }
        }
    }
}
