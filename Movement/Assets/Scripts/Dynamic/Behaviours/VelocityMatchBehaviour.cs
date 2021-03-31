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
    public class VelocityMatchBehaviour : SteeringBehaviour
    {
        [SerializeField] private float timeToTarget = 0.1f;

        // Velocity match behaviour
        public override SteeringOutput GetSteering(GameObject target)
        {
            // Initialize linear and angular forces to zero
            Vector2 linear = Vector2.zero;
            float angular = 0f; // Not used

            // Do I have a target?
            if (target != null)
            {
                // Get the target's rigid body
                Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();

                // Get the target's velocity (if its rigid body is not null)
                Vector2 targetVelocity =
                    targetRb != null ? targetRb.velocity : Vector2.zero;

                // Acceleration tries to get to the target's velocity
                linear = (targetVelocity - Velocity) / timeToTarget;

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
