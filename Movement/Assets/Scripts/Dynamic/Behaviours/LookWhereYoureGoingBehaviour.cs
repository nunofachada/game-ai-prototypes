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
    public class LookWhereYoureGoingBehaviour : AlignBehaviour
    {
        private GameObject lookFakeTarget;

        // Use Awake() to initialize our fake target
        private void Awake()
        {
            lookFakeTarget = new GameObject();
            lookFakeTarget.hideFlags = HideFlags.HideInHierarchy;
            lookFakeTarget.transform.position = Vector3.zero;
        }

        // Look Where You're Going behaviour
        public override SteeringOutput GetSteering(GameObject target)
        {
            // Initialize linear and angular forces to zero
            SteeringOutput sout = new SteeringOutput(Vector2.zero, 0);

            // Am I moving?
            if (Velocity.magnitude > 0)
            {
                // Determine the orientation of our fake target, which should
                // be in the direction we're going
                float angle = Vec2Deg(Velocity);

                // Set orientation of our fake target
                lookFakeTarget.transform.eulerAngles = new Vector3(0f, 0f, angle);

                // Use align superclass to determine the torque to return based
                // on the temporary target orientation (which is the direction
                // we're going)
                sout = base.GetSteering(lookFakeTarget);
            }

            // Output the steering
            return sout;
        }
    }
}