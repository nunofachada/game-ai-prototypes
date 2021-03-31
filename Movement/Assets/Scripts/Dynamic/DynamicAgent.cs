/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using AIUnityExamples.Movement.Core;

namespace AIUnityExamples.Movement.Dynamic
{
    // This class defines movement for dynamic agents
    public class DynamicAgent : MonoBehaviour
    {
        // TODO Set target by movement (not by agent), not all movements need target
        // TODO Implement priorities, maybe just add group ID to movements

        // Maximum acceleration for this agent
        [SerializeField] private float maxAccel = 1f;

        // Maximum speed for this agent
        [SerializeField] private float maxSpeed = 1f;

        // Maximum angular acceleration for this agent
        [SerializeField] private float maxAngularAccel = 1f;

        // Maximum rotation (angular velocity) for this agent
        [SerializeField] private float maxRotation = 1f;

        // The tag for this agent's target
        [SerializeField] private string targetTag = "";

        // Agent steering behaviours
        private ISteeringBehaviour[] steeringBehaviours = default;

        // The agent's rigid body
        private Rigidbody2D rb;

        // Maximum acceleration for this agent
        public float MaxAccel => maxAccel;

        // Maximum speed for this agent
        public float MaxSpeed => maxSpeed;

        // Maximum angular acceleration for this agent
        public float MaxAngularAccel => maxAngularAccel;

        // Maximum rotation (angular velocity) for this agent
        public float MaxRotation => maxRotation;

        // The tag for this agent's target
        public string TargetTag => targetTag;

        // Current angular velocity of this agent
        public float AngularVelocity => rb.angularVelocity;

        // Current velocity of this agent
        public Vector2 Velocity => rb.velocity;

        // Use this for initialization
        private void Start()
        {

            // Keep reference to rigid body
            rb = GetComponent<Rigidbody2D>();

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

            // Get a weighted steering behaviour from the existing behaviours
            foreach (ISteeringBehaviour behaviour in steeringBehaviours)
            {
                // Current steering behaviour
                SteeringOutput steer = behaviour.GetSteering(target);

                // Include current behaviour in the overall weighted behaviour
                steerWeighted = new SteeringOutput(
                    steerWeighted.Linear + behaviour.Weight * steer.Linear,
                    steerWeighted.Angular + behaviour.Weight * steer.Angular);
            }

            // Limit acceleration
            steerWeighted = new SteeringOutput(
                Vector2.ClampMagnitude(steerWeighted.Linear, maxAccel),
                Mathf.Min(steerWeighted.Angular, maxAngularAccel));

            // Apply steering
            rb.AddForce(steerWeighted.Linear);
            rb.AddTorque(steerWeighted.Angular * Mathf.Deg2Rad);

            // Limit speed
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }

            // Limit rotation (angular velocity)
            if (rb.angularVelocity > maxRotation)
            {
                rb.angularVelocity = maxRotation;
            }
        }
    }
}