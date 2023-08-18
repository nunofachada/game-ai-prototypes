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
    public class ObstacleAvoidanceBehaviour : SeekBehaviour
    {
        // Minimum distance to a wall, should be greater than the radius of the
        // agent
        [SerializeField] private float avoidDist = 2f;

        // Lookahead distance for raycast
        [SerializeField] private float lookaheadDist = 4;

        // Where the casted ray ends, for gizmo drawing purposes
        private Vector2 endRay;

        // Obstacle layer mask, for ray casting purposes
        private int obstLayerMask;

        // A fake target to aid in the movement decision
        private GameObject fakeTarget;

        // Use Awake() to initialize a fake target
        private void Awake()
        {
            fakeTarget = new GameObject();
            fakeTarget.hideFlags = HideFlags.HideInHierarchy;
            fakeTarget.transform.eulerAngles = Vector3.zero;
        }

        // Initial setup
        protected override void Start()
        {
            base.Start();
            obstLayerMask = LayerMask.GetMask("Obstacle");
        }

        // Obstacle avoidance behaviour
        public override SteeringOutput GetSteering(GameObject target)
        {
            // Initialize linear and angular forces to zero
            SteeringOutput sout = new SteeringOutput(Vector2.zero, 0);

            // Determine the collision ray direction
            Vector2 rayDir = Velocity.normalized;

            // Find the collision
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, rayDir, lookaheadDist, obstLayerMask);

            // Keep the end of the casted ray, for gizmo drawing purposes
            endRay = (Vector2)transform.position + rayDir * lookaheadDist;

            // Do we have a collision?
            if (hit)
            {
                // Set up a fake target for Seek
                fakeTarget.transform.position =
                    ((Vector2)transform.position) + hit.normal * avoidDist;

                // Delegate to seek
                sout = base.GetSteering(fakeTarget);
            }

            // Output the steering
            return sout;
        }

        // Draw gizmos, basically draw the casted ray
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, endRay);
        }

    }
}