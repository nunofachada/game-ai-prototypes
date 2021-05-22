/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

namespace LibGameAI.PCG
{
    public static class Noise
    {
        // Helper struct for line data
        private struct LineData
        {
            public int Left { get; }
            public int Right { get; }
            public float Randness { get; }

            public LineData(int start, int width, float randness)
            {
                Left = start;
                Right = width;
                Randness = randness;
            }
        }

        /// <summary>
        /// Midpoint displacement algorithm for 2D landscapes.
        /// </summary>
        /// <param name="heights">
        /// Array to fill with landscape heights, zero-centered in average.
        /// </param>
        /// <param name="roughness">
        /// Landscape roughness, between 0 (flat landscape) and 1 (very rough
        /// landscape).
        /// </param>
        /// <param name="nextFloat">
        /// Function which should return a random float between 0 and 1.
        /// </param>
        public static void MPD(
            float[] heights, float roughness, Func<float> nextFloat)
        {
            // Width of heights vector
            int width = heights.Length;

            // Function which returns random value between -max and max
            Func<float, float> rnd = (float max) => max * (nextFloat() * 2 - 1);

            // A queue to help us perform the midpoint displacement
            Queue<LineData> queue = new Queue<LineData>();

            // Enqueue the first line
            queue.Enqueue(new LineData(0, width - 1, 1));

            // Determine the start and end heights of the first line
            heights[0] = rnd(1);
            heights[width - 1] = rnd(1);

            // Perform midpoint displacement on all sublines until no more
            // sublines are possible
            while (queue.Count >  0)
            {
                // Process next subline
                LineData data = queue.Dequeue();

                // Obtain center of current subline
                int center = (data.Left + data.Right + 1) / 2;

                // Set height of current subline as the mean between its
                // endpoints
                heights[center] = (heights[data.Left] + heights[data.Right]) / 2;

                // Add some randomness
                heights[center] += rnd(data.Randness);

                // If the current subline can be further divided...
                if (data.Right - data.Left >  2)
                {
                    // ...divide it and add resulting sublines to the queue for
                    // future processing
                    queue.Enqueue(new LineData(
                        data.Left, center, data.Randness * roughness));
                    queue.Enqueue(new LineData(
                        center, data.Right, data.Randness * roughness));
                }
            }
        }
    }
}