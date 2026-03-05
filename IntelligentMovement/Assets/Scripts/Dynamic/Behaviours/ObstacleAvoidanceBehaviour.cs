/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using UnityEngine;
using GameAIPrototypes.Movement.Core;

namespace GameAIPrototypes.Movement.Dynamic.Behaviours
{
    public class ObstacleAvoidanceBehaviour : SeekBehaviour
    {
        // Number of rays
        private const int numRays = 3;

        // Minimum distance to a wall, should be greater than the radius of the
        // agent
        [SerializeField] private float avoidDist = 2f;

        // Lookahead distance for raycast
        [SerializeField] private float lookaheadDist = 4;

        // Angle between whisker rays and main ray
        [SerializeField] [Range(0, 90)] private float whiskerAngle = 30;

        // Whisker length relative to main ray
        [SerializeField] [Range(0, 1)] private float whiskerRelLength = 0.6f;

        // Where the casted rays ends, for gizmo drawing purposes
        private Vector2[] endRays;

        // Ray hits per frame
        private RaycastHit2D[] raycastHits;

        // Obstacle layer mask, for ray casting purposes
        private int obstLayerMask;

        // A fake target to aid in the movement decision
        private GameObject fakeTarget;

        // Use Awake() to initialize a fake target
        private void Awake()
        {
            // Array of casted rays
            endRays = new Vector2[numRays];

            // Main ray and two whiskers
            raycastHits = new RaycastHit2D[numRays];

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
            // Whisker variables
            float whiskLookaheadDist = lookaheadDist * whiskerRelLength;

            // Initialize linear and angular forces to zero
            SteeringOutput sout = new SteeringOutput(Vector2.zero, 0);

            // Determine the collision ray direction
            Vector2 rayDir = Velocity.normalized;
            Vector2 whisker1Dir = Quaternion.Euler(0, 0, -whiskerAngle) * rayDir;
            Vector2 whisker2Dir = Quaternion.Euler(0, 0, whiskerAngle) * rayDir;

            // Fire the main ray
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, rayDir, lookaheadDist, obstLayerMask);

            // Fire the secondary rays
            RaycastHit2D hitWhisk1 = Physics2D.Raycast(
                transform.position, whisker1Dir, whiskLookaheadDist, obstLayerMask);

            RaycastHit2D hitWhisk2 = Physics2D.Raycast(
                transform.position, whisker2Dir, whiskLookaheadDist, obstLayerMask);

            // Keep the end of the casted rays, for gizmo drawing purposes
            endRays[0] = (Vector2)transform.position + rayDir * lookaheadDist;
            endRays[1] = (Vector2)transform.position + whisker1Dir * whiskLookaheadDist;
            endRays[2] = (Vector2)transform.position + whisker2Dir * whiskLookaheadDist;

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
            foreach (Vector2 endRay in endRays)
                Gizmos.DrawLine(transform.position, endRay);
        }

    }
}