/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada 
 * */

using UnityEngine;

// This class defines movement for dynamic agents
public class DynamicAgent : MonoBehaviour
{

    // Maximum acceleration for this agent
    public float maxAccel;

    // Maximum speed for this agent
    public float maxSpeed;

    // Maximum angular acceleration for this agent
    public float maxAngularAccel;

    // Maximum rotation (angular velocity) for this agent
    public float maxRotation;

    // The tag for this agent's target
    public string targetTag;

    // Agent steering behaviours
    private ISteeringBehaviour[] steeringBehaviours;

    // The agent's rigid body
    public Rigidbody2D Rb { get; private set; }

    // Use this for initialization
    private void Start()
    {

        // Keep reference to rigid body
        Rb = GetComponent<Rigidbody2D>();

        // Get steering behaviours defined for this agent
        steeringBehaviours = GetComponents<ISteeringBehaviour>();
    }

    // This is called every physics update
    private void FixedUpdate()
    {
        // Is there any target for me?
        GameObject target = targetTag != ""
            ? GameObject.FindWithTag(targetTag)
            : null;

        // Obtain steering behaviours
        SteeringOutput steerWeighted = new SteeringOutput();

        foreach (ISteeringBehaviour behaviour in steeringBehaviours)
        {
            SteeringOutput steer = behaviour.GetSteering(target);
            steerWeighted = new SteeringOutput(
                steerWeighted.Linear + behaviour.Weight * steer.Linear,
                steerWeighted.Angular + behaviour.Weight * steer.Angular);
        }

        // Apply steering
        Rb.AddForce(steerWeighted.Linear);
        Rb.AddTorque(steerWeighted.Angular);

        // Limit speed
        if (Rb.velocity.magnitude > maxSpeed)
        {
            Rb.velocity = Rb.velocity.normalized * maxSpeed;
        }

        // Limit rotation (angular velocity)
        if (Rb.angularVelocity > maxRotation)
        {
            Rb.angularVelocity = maxRotation;
        }
    }
}
