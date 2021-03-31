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
    public class ArriveBehaviour : SteeringBehaviour
    {
        // Target radius
        [SerializeField] private float targetRadius = 0.1f;
        // Slowdown radius
        [SerializeField] private float slowdownRadius = 20f;
        // Time to target
        [SerializeField] private float timeToTarget = 1f;

        // Arrive behaviour
        public override SteeringOutput GetSteering(GameObject target)
        {
            // Initialize linear and angular forces to zero
            Vector2 linear = Vector2.zero;
            float angular = 0f; // Not used

            // Do I have a target?
            if (target != null)
            {
                // Desired speed and velocity
                float desiredSpeed;
                Vector2 desiredVelocity;

                // Get the direction to the target
                Vector2 dir = target.transform.position - transform.position;

                // Get distance to target
                float distance = dir.magnitude;

                // Are we at the target radius yet?
                if (distance < targetRadius)
                {
                    // Return no steering whatsoever
                    return new SteeringOutput(Vector2.zero, 0f);
                }
                // Are we within the slowdown radius?
                else if (distance < slowdownRadius)
                {
                    // Adjust desired speed depending on distance to target
                    desiredSpeed = MaxSpeed * distance / slowdownRadius;
                }
                else
                {
                    // If we're outside the slowdown radius, go for max speed
                    desiredSpeed = MaxSpeed;
                }

                // Desired velocity combines desired speed and direction to target
                desiredVelocity = dir.normalized * desiredSpeed;

                // Linear acceleration tries to get to the target velocity
                linear = (desiredVelocity - Velocity) / timeToTarget;

                // Check if acceleration is too fast
                if (linear.magnitude > MaxAccel)
                {
                    linear = linear.normalized * MaxAccel;
                }
            }

            // Output the steering
            return new SteeringOutput(linear, angular);
        }
    }
}