/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace GameAIPrototypes.Movement.Core
{
    // Helper class to determine game area and obtain random positions within it
    public class GameArea : MonoBehaviour
    {
        public float Xmax { get; private set; }
        public float Xmin { get; private set; }
        public float Ymax { get; private set; }
        public float Ymin { get; private set; }

        // Configure game area limits object
        private void Awake()
        {
            // Get the sprite renderer and its bounds
            SpriteRenderer sr = GetComponentInParent<SpriteRenderer>();
            Bounds bounds = sr.bounds;

            // Determine and keep game area limits
            Xmax = bounds.min.x;
            Xmin = bounds.max.x;
            Ymax = bounds.min.y;
            Ymin = bounds.max.y;
        }

        // Determine random position within game area
        public Vector2 RandomPosition(float margin)
        {
            return new Vector2(
                Random.Range(Xmin * margin, Xmax * margin),
                Random.Range(Ymin * margin, Ymax * margin));
        }

        // Determine opposite position within game area
        public Vector2 OppositePosition(char wall, Vector2 currentPos)
        {
            Vector2 newPosition = wall switch
            {
                'E' => new Vector2(Xmax * 0.9f, currentPos.y),
                'W' => new Vector2(Xmin * 0.9f, currentPos.y),
                'N' => new Vector2(currentPos.x, Ymax * 0.9f),
                'S' => new Vector2(currentPos.x, Ymin * 0.9f),
                _ => new Vector2(),
            };
            return newPosition;
        }

        // Draw gizmos, namely borders around the game area
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blueViolet;

            Vector3[] points = new Vector3[]
            {
                new Vector3(Xmin, Ymin, 0),
                new Vector3(Xmin, Ymax, 0),

                new Vector3(Xmin, Ymax, 0),
                new Vector3(Xmax, Ymax, 0),

                new Vector3(Xmax, Ymax, 0),
                new Vector3(Xmax, Ymin, 0),

                new Vector3(Xmax, Ymin, 0),
                new Vector3(Xmin, Ymin, 0)
            };

            Gizmos.DrawLineList(points);
        }
    }
}