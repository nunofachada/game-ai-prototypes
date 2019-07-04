/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada 
 * */
using UnityEngine;

public class LookWhereYoureGoingBehaviour : AlignBehaviour
{

    // Look Where You're Going behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        SteeringOutput sout = new SteeringOutput(Vector2.zero, 0);

        // Am I moving?
        if (rb.velocity.magnitude > 0)
        {
            // Determine the orientation of our temporary target, which should
            // be in the direction we're going
            float angle = Vec2Deg(rb.velocity);

            // Create our temporary target
            GameObject tempTarget = CreateTarget(Vector3.zero, angle);

            // Use align superclass to determine the torque to return based
            // on the temporary target orientation (which is the direction
            // we're going)
            sout = base.GetSteering(tempTarget);

            // Destroy the temporary copy of our target
            Destroy(tempTarget);
        }

        // Output the steering
        return sout;
    }

}
