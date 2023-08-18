/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;
using System;
using System.Collections.Generic;

namespace AIUnityExamples.Movement.Paths
{
    // Ideas:
    // 1 - Make paths using a wander-style generator
    // 2 - Allow for explicit paths specified in the Unity Editor in another IPath
    // class
    public class LineSegmentPath : AbstractPath
    {
        private struct Waypoint : IComparable<Waypoint>
        {
            public Vector2 Point { get; }
            public float Distance { get; }
            public Vector2 NormDirToNext { get; }

            public Waypoint(Vector2 point, Vector2 nextPoint, float distance)
            {
                Point = point;
                Distance = distance;
                NormDirToNext = (nextPoint - point).normalized;
            }

            public int CompareTo(Waypoint other)
            {
                if (other.Distance > Distance) return -1;
                else if (other.Distance < Distance) return 1;
                else return 0;
            }
        }

        private List<Waypoint> waypoints;

        private float totalDistance;

        public override float GetParam(Vector2 position, float lastParam)
        {
            // Required variables
            Vector2 relPoint, lineSegDir, normLineSegDir, paramPoint;
            float dotProd, lineSegLen;
            int prevIdx, nextIdx;

            // If only one point in path, always return zero
            if (waypoints.Count == 1) return 0f;

            // Get index of point relative to the last parameter
            GetPositionIndex(lastParam, out prevIdx, out nextIdx);

            // Project position onto the line segment
            relPoint = position - waypoints[prevIdx].Point;
            lineSegDir = waypoints[nextIdx].Point - waypoints[prevIdx].Point;
            lineSegLen = lineSegDir.magnitude;
            normLineSegDir = lineSegDir;
            if (lineSegLen > .000001f) normLineSegDir /= lineSegLen;
            dotProd = Vector2.Dot(normLineSegDir, relPoint);
            dotProd = Mathf.Clamp(dotProd, 0.0f, lineSegLen);
            paramPoint = waypoints[prevIdx].Point + normLineSegDir * dotProd;

            // Get distance from last point in path to projected position
            float paramToReturn = waypoints[prevIdx].Distance
                + (paramPoint - waypoints[prevIdx].Point).magnitude;

            Debug.Log($"LastParam={lastParam} NewParam={paramToReturn}");

            return paramToReturn;
        }

        public override Vector2 GetPosition(float param)
        {
            // The indexes
            int prevIdx, nextIdx;

            // How much to go from previous waypoint
            float localParam;

            // Offset from previous waypoint towards next waypoint
            Vector2 offset;

            // Obtain the indexes
            GetPositionIndex(param, out prevIdx, out nextIdx);

            // How much to go from previous waypoint?
            localParam = param - waypoints[prevIdx].Distance;

            // Determine offset vector from previous waypoint
            offset = waypoints[prevIdx].NormDirToNext * localParam;

            // Return position
            return waypoints[prevIdx].Point + offset;
        }

        private void GetPositionIndex(float param, out int prevIdx, out int nextIdx)
        {

            // If only one point in path, return that point
            if (waypoints.Count == 1)
            {
                prevIdx = 0;
                nextIdx = 0;
            }

            // Parameter cannot be larger than the total path distance, if so
            // remove extra length
            while (param > totalDistance) param -= totalDistance;

            // Search for index of line segment containing lastParam
            nextIdx =
                waypoints.BinarySearch(
                    new Waypoint(Vector2.zero, Vector2.zero, param));
            nextIdx = nextIdx >= 0 ? nextIdx : ~nextIdx;

            // If next index is out of range, wrap it to zero
            if (nextIdx >= waypoints.Count) nextIdx = 0;

            // If index is zero the previous index is the last index
            if (nextIdx == 0) prevIdx = waypoints.Count - 1;
            // Otherwise its just the previous index
            else prevIdx = nextIdx - 1;

            Debug.Log($"Prev={prevIdx} Next={nextIdx} Param={param}");
        }

        protected override void BuildPath()
        {
            // Initialize required variables
            float distanceSoFar = 0;
            Vector2 previousPoint = PathPoints[0];
            waypoints = new List<Waypoint>(PathPoints.Count);

            // Build path
            for (int i = 0; i < PathPoints.Count; i++)
            {
                // Get current point
                Vector2 currentPoint = PathPoints[i];
                // Get next point (may wrap around)
                Vector2 nextPoint = i + 1 < PathPoints.Count
                    ? PathPoints[i + 1]
                    : PathPoints[0];

                // Determine distance so far
                distanceSoFar += (previousPoint - currentPoint).magnitude;

                // Add new waypoint to path
                waypoints.Add(new Waypoint(currentPoint, nextPoint, distanceSoFar));

                // Make previous point the current point
                previousPoint = currentPoint;
            }

            // If its a closed path, add distance from last to first
            if (IsClosedPath) distanceSoFar +=
                (previousPoint - PathPoints[0]).magnitude;

            // Keep the total distance in an instance variable
            totalDistance = distanceSoFar;

            Debug.Log("Total distance: " + totalDistance);
        }
    }
}