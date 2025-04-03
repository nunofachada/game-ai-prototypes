/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using LibGameAI.DecisionTrees;

namespace GameAIPrototypes.BehaviorTrees
{
    public class AIBehaviour : WaypointCycler
    {
        // Speed of AI agent movement
        [SerializeField]
        [Range(0f, 20f)]
        private float speed = 6f;

        // Minimum distance to waypoint to trigger a new waypoint
        [SerializeField]
        private float minWaypointDist = 0.5f;

        // Player in sight distance
        [SerializeField]
        private float playerInSightDistance = 10f;

        // Random decision duration
        [SerializeField]
        private float randomDecisionDurationInSeconds = 1.5f;

        // References the player
        private GameObject player;

        // Reference the WTF object for when AI is surprised
        private GameObject wtf;

        // The root of the decision tree
        private IDecisionTreeNode root;

        // The last target acquired
        private Vector3 lastTargetAcquired = Vector3.zero;

        // Get reference to the agent's rigid body, the player and the wtf object
        private void Awake()
        {
            player = GameObject.Find("PlayerAgent");
            wtf = transform.GetChild(0).gameObject;
            wtf.SetActive(false);
        }

        // Create the decision tree
        protected override void Start()
        {
            // Call base class Start()
            base.Start();

            // Create the leaf actions
            IDecisionTreeNode seek = new ActionNode(SeekAction);
            IDecisionTreeNode nothing = new ActionNode(StandStillAction);
            IDecisionTreeNode patrol = new ActionNode(PatrolAction);

            // Create the random decision behaviour node
            RandomDecisionBehaviour rdb = new RandomDecisionBehaviour(
                () => Random.value,
                () => Time.time,
                randomDecisionDurationInSeconds,
                0.55f);
            IDecisionTreeNode rndNode = new DecisionNode(
                rdb.RandomDecision, seek, nothing);

            // Create the root node
            root = new DecisionNode(PlayerInSight, rndNode, patrol);
        }

        // Run the decision tree and execute the returned action
        private void Update()
        {
            ActionNode actionNode = root.MakeDecision() as ActionNode;
            actionNode.Execute();
        }

        // Check if player is in sight
        private bool PlayerInSight()
        {
            Vector3 playerPosition = player.transform.position;
            float distance = (playerPosition - transform.position).magnitude;
            if (distance < playerInSightDistance) return true;
            return false;
        }

        // Seek player action
        private void SeekAction()
        {
            // Move towards player
            MoveTowardsTarget(player.transform.position);
        }

        // Stand still action
        private void StandStillAction()
        {
            lastTargetAcquired = transform.position;
            wtf.SetActive(true);
        }

        // Patrol waypoints action
        private void PatrolAction()
        {
            // Are we close enough to the current waypoint?
            if ((CurrentWaypoint - transform.position).magnitude < minWaypointDist)
            {
                // If so, get a new waypoint
                NextWaypoint();
            }

            // Move towards the next waypoint
            MoveTowardsTarget(CurrentWaypoint);
        }

        // Move towards target
        private void MoveTowardsTarget(Vector3 targetPos)
        {
            // Determine velocity to the target
            Vector3 vel = (targetPos - transform.position).normalized * speed;

            // Move towards the target  at the calculated velocity
            transform.position += vel * Time.deltaTime;

            // Agent is moving, thus it's not surprised
            wtf.SetActive(false);

            // Keep reference to last target acquired
            lastTargetAcquired = targetPos;
        }

        // Draw gizmos at waypoints
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (lastTargetAcquired != Vector3.zero)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, lastTargetAcquired);
            }
        }
    }
}