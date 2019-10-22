/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;

public class PursueBehaviour : SeekBehaviour
{
    // Maximum prediction time
    [SerializeField] private float maxPrediction = 4f;

    // Pursue behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        SteeringOutput sout = new SteeringOutput(Vector2.zero, 0);

        // Do I have a target?
        if (target != null)
        {
            // Our target's velocity (this is inefficient!)
            Vector2 targetVelocity =
                target.GetComponent<Rigidbody2D>()?.velocity ?? Vector2.zero;
            // Our target's position
            Vector2 targetPosition = target.transform.position;

            // Our prediction
            float prediction;

            // Determine distance to target
            Vector2 direction = target.transform.position - transform.position;
            float distance = direction.magnitude;

            // Determine the agent's current speed
            float speed = Agent.Velocity.magnitude;

            // Check if speed is too small to give reasonable predicition time
            if (speed <= distance / maxPrediction) prediction = maxPrediction;
            // Otherwise determine the prediction time
            else prediction = distance / speed;

            // Create our temporary target
            GameObject tempTarget = CreateTarget(
                targetPosition + targetVelocity * prediction, 0f);

            // Use seek superclass to determine steering
            sout = base.GetSteering(tempTarget);

            // Destroy the temporary copy of our target
            Destroy(tempTarget);
        }

        // Output the steering
        return sout;
    }

}
