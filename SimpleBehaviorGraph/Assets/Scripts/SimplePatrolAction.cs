/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using Unity.Behavior;

namespace GameAIPrototypes.BehaviorTrees
{
    /// <summary>
    /// Simple patrol behavior based on Unity.Behavior.PatrolAction.
    /// Remove support for navmesh and use Vector3 list instead of game object
    /// list.
    /// </summary>
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "SimplePatrol",
        description: "Moves a GameObject along way points using its transform.",
        category: "Action/Navigation",
        story: "[Agent] patrols along [Waypoints]")]
    internal partial class SimplePatrolAction : Unity.Behavior.Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<List<Vector3>> Waypoints;
        [SerializeReference] public BlackboardVariable<float> Speed;
        [SerializeReference] public BlackboardVariable<float> WaypointWaitTime = new BlackboardVariable<float>(1.0f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
        [SerializeReference] public BlackboardVariable<bool> ArriveSlowdown = new(false);

        [Tooltip("Should patrol restart from the latest point?")]
        [SerializeReference] public BlackboardVariable<bool> PreserveLatestPatrolPoint = new(false);


        [CreateProperty]
        private Vector3 m_CurrentTarget;
        [CreateProperty]
        private int m_CurrentPatrolPoint = 0;
        [CreateProperty]
        private bool m_Waiting;
        [CreateProperty]
        private float m_WaypointWaitTimer;

        protected override Status OnStart()
        {
            if (Agent.Value == null)
            {
                LogFailure("No agent assigned.");
                return Status.Failure;
            }

            if (Waypoints.Value == null || Waypoints.Value.Count == 0)
            {
                LogFailure("No waypoints to patrol assigned.");
                return Status.Failure;
            }

            Initialize();

            m_Waiting = false;
            m_WaypointWaitTimer = 0.0f;

            MoveToNextWaypoint();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Waypoints.Value == null)
            {
                return Status.Failure;
            }

            if (m_Waiting)
            {
                if (m_WaypointWaitTimer > 0.0f)
                {
                    m_WaypointWaitTimer -= Time.deltaTime;
                }
                else
                {
                    m_WaypointWaitTimer = 0f;
                    m_Waiting = false;
                    MoveToNextWaypoint();
                }
            }
            else
            {
                float distance = GetDistanceToWaypoint();
                Vector3 agentPosition = Agent.Value.transform.position;

                if (distance <= DistanceThreshold)
                {
                    m_WaypointWaitTimer = WaypointWaitTime.Value;
                    m_Waiting = true;
                }

                float speed = ArriveSlowdown.Value ? Mathf.Min(Speed, distance) : Speed;

                Vector3 toDestination = m_CurrentTarget - agentPosition;
                //toDestination.y = 0.0f;
                toDestination.Normalize();
                agentPosition += toDestination * (speed * Time.deltaTime);
                Agent.Value.transform.position = agentPosition;
                Agent.Value.transform.forward = toDestination;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {

        }

        protected override void OnDeserialize()
        {
            Initialize();
        }

        private void Initialize()
        {
            m_CurrentPatrolPoint = PreserveLatestPatrolPoint.Value ? m_CurrentPatrolPoint - 1 : -1;
        }

        private float GetDistanceToWaypoint()
        {
            Vector3 targetPosition = m_CurrentTarget;
            Vector3 agentPosition = Agent.Value.transform.position;
            agentPosition.y = targetPosition.y; // Ignore y for distance check.
            return Vector3.Distance(
                agentPosition,
                targetPosition
            );
        }

        private void MoveToNextWaypoint()
        {
            m_CurrentPatrolPoint = (m_CurrentPatrolPoint + 1) % Waypoints.Value.Count;

            m_CurrentTarget = Waypoints.Value[m_CurrentPatrolPoint];
        }
    }
}
