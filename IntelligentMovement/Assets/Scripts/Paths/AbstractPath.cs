/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using System.Collections.Generic;

namespace AIUnityExamples.Movement.Paths
{
    public abstract class AbstractPath : MonoBehaviour, IPath
    {
        [SerializeField] private bool isClosedPath = true;

        private AbstractPathGenerator pathGenerator;

        public bool IsClosedPath => isClosedPath;

        protected IList<Vector2> PathPoints { get; private set; }

        private void Awake()
        {
            // Obtain a path generator
            AbstractPathGenerator pathGenerator =
                GetComponent<AbstractPathGenerator>();

            // Generate a path
            PathPoints = pathGenerator?.GeneratePath();

            // At least one point should have been generated
            if (PathPoints == null || PathPoints.Count < 2)
                Debug.LogError(
                    "No path was generated! At least two points are required! "
                    + "Are you missing a generator?");

            // Delegate path building to subclasses
            BuildPath();
        }

        protected void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            Gizmos.color = Color.red;
            foreach (Vector2 point in PathPoints)
                Gizmos.DrawSphere(new Vector3(point.x, point.y, 1), 0.5f);
        }

        public abstract float GetParam(Vector2 position, float lastParam);

        public abstract Vector2 GetPosition(float param);

        protected abstract void BuildPath();
    }
}