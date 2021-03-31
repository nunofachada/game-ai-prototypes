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
    public class EvadeBehaviour : FleeBehaviour
    {
        // Maximum prediction time
        [SerializeField] private float maxPrediction = 4f;

        // A fake target to aid in the movement decision
        private GameObject fakeTarget;

        // Use Awake() to initialize a fake target
        private void Awake()
        {
            fakeTarget = new GameObject();
            fakeTarget.hideFlags = HideFlags.HideInHierarchy;
            fakeTarget.transform.eulerAngles = Vector3.zero;
        }

        // Evade behaviour
        // This is essentially the same code as Pursue, so we should abstract it
        // to another class or method
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
                float speed = Velocity.magnitude;

                // Check if speed is too small to give reasonable predicition time
                if (speed <= distance / maxPrediction) prediction = maxPrediction;
                // Otherwise determine the prediction time
                else prediction = distance / speed;

                // Set fake target position
                fakeTarget.transform.position =
                    targetPosition + targetVelocity * prediction;

                // Use seek superclass to determine steering
                sout = base.GetSteering(fakeTarget);
            }

            // Output the steering
            return sout;
        }

    }
}