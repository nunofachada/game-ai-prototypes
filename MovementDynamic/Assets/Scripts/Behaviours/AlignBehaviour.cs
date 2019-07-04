/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada 
 * */
using UnityEngine;

public class AlignBehaviour : SteeringBehaviour
{

    // Target angle radius in degrees
    public float targetAngleRadius = 1f;
    // Slowdown angle radius in degrees
    public float slowdownAngleRadius = 10f;
    // Time to target
    public float timeToTarget = 0.1f;

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
            orientation =
                target.transform.eulerAngles.z - agent.transform.eulerAngles.z;

            // Map the orientation difference to the (-180, 180) interval
            while (orientation > 180) orientation -= 360;
            while (orientation < -180) orientation += 360;

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
                    agent.maxRotation * orientationAbs / slowdownAngleRadius;
            }
            else
            {
                // If we're outside the slowdown radius, go for max rotation
                desiredAngularVelocity = agent.maxRotation;
            }

            // Set the correct sign in the desired angular velocity
            desiredAngularVelocity *= orientation / orientationAbs;

            // Determine the angular force (difference in desired angular
            // velocity and current angular velocity, divided by the time
            // to target)
            angular = (desiredAngularVelocity - agent.Rb.angularVelocity)
                / timeToTarget;

            // Check if angular force/acceleration is too great
            angularAbs = Mathf.Abs(angular);
            if (angularAbs > agent.maxAngularAccel)
            {
                // If so set it to the maximum allowed value
                angular /= angularAbs;
                angular *= agent.maxAngularAccel;
            }
        }

        // Output the steering
        return new SteeringOutput(linear, angular);
    }
}
