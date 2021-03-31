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
    public class AlignBehaviour : SteeringBehaviour
    {
        // Target angle radius in degrees
        [SerializeField] private float targetAngleRadius = 1f;
        // Slowdown angle radius in degrees
        [SerializeField] private float slowdownAngleRadius = 10f;
        // Time to target
        [SerializeField] private float timeToTarget = 0.1f;

        // Align behaviour
        public override SteeringOutput GetSteering(GameObject target)
        {
            // Initialize linear and angular forces to zero
            Vector2 linear = Vector2.zero;
            float angular = 0f; // Not used

            // Do I have a target?
            if (target != null)
            {
                // Orientation differences (actual and absolute values)
                float orientation, orientationAbs;

                // Desired angular velocity and absolute angular force to apply
                float desiredAngularVelocity, angularAbs;

                // Get the orientation difference to the target
                orientation = Mathf.DeltaAngle(
                    transform.eulerAngles.z,
                    target.transform.eulerAngles.z);

                // Get the absolute orientation difference
                orientationAbs = Mathf.Abs(orientation);

                // Are we within the target angle radius yet?
                if (orientationAbs < targetAngleRadius)
                {
                    // Return no steering whatsoever
                    return new SteeringOutput(Vector2.zero, 0f);
                }
                // Are we within the slowdown angle radius?
                else if (orientationAbs < slowdownAngleRadius)
                {
                    // Adjust desired angular velocity depending current
                    // orientation
                    desiredAngularVelocity =
                        MaxRotation * orientationAbs / slowdownAngleRadius;
                }
                else
                {
                    // If we're outside the slowdown radius, go for max rotation
                    desiredAngularVelocity = MaxRotation;
                }

                // Set the correct sign in the desired angular velocity
                desiredAngularVelocity *= orientation / orientationAbs;

                // Determine the angular force (difference in desired angular
                // velocity and current angular velocity, divided by the time
                // to target)
                angular = (desiredAngularVelocity - AngularVelocity)
                    / timeToTarget;

                // Check if angular force/acceleration is too great
                angularAbs = Mathf.Abs(angular);
                if (angularAbs > MaxAngularAccel)
                {
                    // If so set it to the maximum allowed value
                    angular /= angularAbs;
                    angular *= MaxAngularAccel;
                }
            }

            // Output the steering
            return new SteeringOutput(linear, angular);
        }
    }
}