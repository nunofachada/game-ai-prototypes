/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace AIUnityExamples.Movement.Core
{
    // Helper class to determine game area and obtain random positions within it
    public class GameArea
    {
        public float Xmax { get; }
        public float Xmin { get; }
        public float Ymax { get; }
        public float Ymin { get; }

        // Create new game area limits object
        public GameArea()
        {
            // Get world bounds
            GameObject bg = GameObject.FindWithTag("Background");

            SpriteRenderer sr = bg.GetComponent<SpriteRenderer>();
            Bounds bounds = sr.sprite.bounds;

            // Determine and keep game area limits
            Xmax = (bounds.center.x - bounds.extents.x) * sr.transform.lossyScale.x;
            Xmin = (bounds.center.x + bounds.extents.x) * sr.transform.lossyScale.x;
            Ymax = (bounds.center.y - bounds.extents.y) * sr.transform.lossyScale.y;
            Ymin = (bounds.center.y + bounds.extents.y) * sr.transform.lossyScale.y;
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
            Vector2 newPosition;
            switch (wall)
            {
                case 'E':
                    newPosition = new Vector2(Xmax * 0.9f, currentPos.y);
                    break;
                case 'W':
                    newPosition = new Vector2(Xmin * 0.9f, currentPos.y);
                    break;
                case 'N':
                    newPosition = new Vector2(currentPos.x, Ymax * 0.9f);
                    break;
                case 'S':
                    newPosition = new Vector2(currentPos.x, Ymin * 0.9f);
                    break;
                default:
                    newPosition = new Vector2();
                    break;
            }
            return newPosition;
        }
    }
}