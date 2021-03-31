/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using AIUnityExamples.Movement.Core;

namespace AIUnityExamples.Movement.Dynamic.Behaviours
{
    public class SeparationBehaviour : SteeringBehaviour
    {
        // The threshold to take action
        [SerializeField] private float threshold = 5f;

        // The constant coefficient of decay for the inverse square law force
        [SerializeField] private float decayCoefficient = 8f;

        // Separation behaviour
        public override SteeringOutput GetSteering(GameObject target)
        {
            // Initialize linear and angular forces to zero
            Vector2 linear = Vector2.zero;
            float angular = 0f; // Not used

            // Find similar agents to me
            GameObject[] neighbours = GameObject.FindGameObjectsWithTag(Tag);

            // Loop through each target
            foreach (GameObject neighbour in neighbours)
            {
                // Direction and distance to current neighbour
                Vector2 direction;
                float distance;

                // Is the neighbour me? Then skip current iteration!
                if (neighbour == gameObject) continue;

                // Get direction and distance from current neighbour
                direction = transform.position - neighbour.transform.position;
                distance = direction.magnitude;

                // Check if neighbour is close
                if (distance < threshold)
                {
                    // Determine strength of repulsion
                    float strength = Mathf.Min(
                        decayCoefficient / (distance * distance),
                        MaxAccel);

                    // Add the acceleration
                    linear += strength * direction.normalized;
                }
            }

            // Output the steering
            return new SteeringOutput(linear, angular);
        }

        // Draw gizmos, namely the separation force field
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, threshold);
        }
    }
}