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
    public abstract class SteeringBehaviour : MonoBehaviour, ISteeringBehaviour
    {
        [SerializeField] private float weight = 1f;

        private DynamicAgent agent;
        private Rigidbody2D rb;

        public float Weight => weight;

        // Maximum acceleration for this agent
        protected float MaxAccel => agent.MaxAccel;

        // Maximum speed for this agent
        protected float MaxSpeed => agent.MaxSpeed;

        // Maximum angular acceleration for this agent
        protected float MaxAngularAccel => agent.MaxAngularAccel;

        // Maximum rotation (angular velocity) for this agent
        protected float MaxRotation => agent.MaxRotation;

        // The tag for this agent's target
        protected string TargetTag => agent.TargetTag;

        // Current angular velocity of this agent
        protected float AngularVelocity => rb.angularVelocity;

        // Current velocity of this agent
        protected Vector2 Velocity => rb.velocity;

        // Tag for this agent
        protected string Tag => agent.tag;

        // Use this for initialization
        protected virtual void Start()
        {
            agent = GetComponent<DynamicAgent>();
            rb = GetComponent<Rigidbody2D>();
        }

        public abstract SteeringOutput GetSteering(GameObject target);

        public static Vector2 Deg2Vec(float angle)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float Vec2Deg(Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }

        public static GameObject CreateTarget(Vector3 position, float orientation)
        {
            GameObject target = new GameObject();
            target.transform.position = position;
            target.transform.Rotate(new Vector3(0, 0, orientation), Space.World);
            return target;
        }
    }
}