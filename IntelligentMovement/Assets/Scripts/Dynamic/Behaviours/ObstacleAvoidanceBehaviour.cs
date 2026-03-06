/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

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

        // Where the casted rays ends and if they hit, for gizmo drawing purposes
        private (Vector2 end, bool hit)[] endRays;

        // Array of ray information
        private (Vector2 dir, float lookaheadDist)[] rayInfo;

        // Obstacle layer mask, for ray casting purposes
        private int obstLayerMask;

        // A fake target to aid in the movement decision
        private GameObject fakeTarget;

        // Use Awake() to initialize a fake target
        private void Awake()
        {
            // Array of casted rays
            endRays = new (Vector2, bool)[numRays];

            // Information on the main ray and two whiskers
            rayInfo = new (Vector2, float)[numRays];

            // Get the obstacle layer
            obstLayerMask = LayerMask.GetMask("Obstacle");

            // Initialize the fake object which will serve as target to avoid
            // collisions
            fakeTarget = new GameObject();
            fakeTarget.hideFlags = HideFlags.HideInHierarchy;
            fakeTarget.transform.eulerAngles = Vector3.zero;
        }

        // Initial setup
        protected override void Start()
        {
            base.Start();
        }

        // Obstacle avoidance behaviour
        public override SteeringOutput GetSteering(GameObject target)
        {
            // Initialize linear and angular forces to zero
            SteeringOutput sout = new SteeringOutput(Vector2.zero, 0);

            // Obtain main ray direction
            Vector2 mainDir = Velocity.normalized;

            // Determine whisker rays lookahead distance
            float whiskLookaheadDist = lookaheadDist * whiskerRelLength;

            // Configure rays
            rayInfo[0] = (mainDir, lookaheadDist);
            rayInfo[1] = (Quaternion.Euler(0, 0, -whiskerAngle) * mainDir, whiskLookaheadDist);
            rayInfo[2] = (Quaternion.Euler(0, 0, whiskerAngle) * mainDir, whiskLookaheadDist);

            // Detect hits and find a fake target if any hits detected
            Vector2 hitNormalSum = Vector2.zero;

            for (int i = 0; i < numRays; i++)
            {
                // Keep the ray ending for Gizmo drawing purposes
                endRays[i] = (
                    (Vector2)transform.position + rayInfo[i].dir * rayInfo[i].lookaheadDist,
                    false);

                // Check if the current ray hits something
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position, rayInfo[i].dir,
                    rayInfo[i].lookaheadDist, obstLayerMask);

                // If a hit occurred, update fake target vector
                if (hit)
                {
                    hitNormalSum += hit.normal;
                    endRays[i].hit = true;
                }
            }

            // If the fake target vector has any magniture (is not zero), then
            // delegate steering to seek that fake target
            if (hitNormalSum.magnitude > 0)
            {
                // Update the fake target position with respect to this agent
                fakeTarget.transform.position =
                    ((Vector2)transform.position) + hitNormalSum * avoidDist;

                // Delegate steering towards the fake target to Seek
                sout = base.GetSteering(fakeTarget);
            }

            // Output the steering
            return sout;
        }

        // Draw gizmos, basically draw the casted ray
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            foreach ((Vector2 ray, bool hit) endRay in endRays)
            {
                Gizmos.color = endRay.hit ? Color.red : Color.yellow;
                Gizmos.DrawLine(transform.position, endRay.ray);
            }
        }

    }
}