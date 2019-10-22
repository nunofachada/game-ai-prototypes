/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;

public class WanderBehaviour : FaceBehaviour
{

    // Wander forward offset
    [SerializeField] private float wanderOffset = 4f;

    // Radius of the wander circle
    [SerializeField] private float wanderRadius = 1f;

    // The maximum rate at which the wander orientation can change
    [SerializeField] private float wanderRate = 0.5f;

    // The current orientation of the wander target
    private float wanderOrientation = 0f;

    // Wander behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Target orientation and position
        float targetOrientation;
        Vector2 targetPosition;

        // Steering output to return
        SteeringOutput sout;

        // Update the wander orientation
        wanderOrientation +=
            (Random.Range(0, 1f) - Random.Range(0, 1f)) * wanderRate;

        // Calculate the combined target orientation
        targetOrientation = wanderOrientation + Agent.transform.eulerAngles.z;

        // Calculate the center of the wander circle
        targetPosition = ((Vector2)Agent.transform.position)
            + wanderOffset * Deg2Vec(Agent.transform.eulerAngles.z);

        // Calculate the target location
        targetPosition += wanderRadius * Deg2Vec(targetOrientation);

        // Instantiate a temporary target for Face to aim at
        target = CreateTarget(targetPosition, targetOrientation);

        // Ask superclass method to look at target
        sout = base.GetSteering(target);

        // Destroy temporary target
        Destroy(target);

        // Set the linear acceleration to maximum in the direction of the
        // agent's current orientation
        sout = new SteeringOutput(
            Agent.MaxAccel * Deg2Vec(Agent.transform.eulerAngles.z),
            sout.Angular);

        // Return steering behaviour
        return sout;
    }

}
