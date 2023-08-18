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

        // A fake target to aid in the movement decision
        private GameObject wanderFakeTarget;

        // Use Awake() to initialize a fake target
        protected override void Awake()
        {
            base.Awake();
            wanderFakeTarget = new GameObject();
            wanderFakeTarget.hideFlags = HideFlags.HideInHierarchy;
        }

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
            targetOrientation = wanderOrientation + transform.eulerAngles.z;

            // Calculate the center of the wander circle
            targetPosition = ((Vector2)transform.position)
                + wanderOffset * Deg2Vec(transform.eulerAngles.z);

            // Calculate the target location
            targetPosition += wanderRadius * Deg2Vec(targetOrientation);

            // Use a temporary target for Face superclass to aim at
            wanderFakeTarget.transform.position = targetPosition;
            wanderFakeTarget.transform.eulerAngles =
                new Vector3(0f, 0f, targetOrientation);

            // Ask superclass method to look at target
            sout = base.GetSteering(wanderFakeTarget);

            // Set the linear acceleration to maximum in the direction of the
            // agent's current orientation
            sout = new SteeringOutput(
                MaxAccel * Deg2Vec(transform.eulerAngles.z),
                sout.Angular);

            // Return steering behaviour
            return sout;
        }

        // Draw gizmos, namely a sphere around the fake target
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                wanderFakeTarget.transform.position, Mathf.Max(0.1f, wanderRadius));
        }
    }
}