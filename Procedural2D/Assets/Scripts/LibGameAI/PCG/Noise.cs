/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;
using LibGameAI.Util;

namespace LibGameAI.PCG
{
    public static class Noise
    {
        // Helper struct for line data
        private struct SubLine
        {
            public int Left { get; }
            public int Right { get; }
            public float Displacement { get; }

            public SubLine(int left, int right, float randness)
            {
                Left = left;
                Right = right;
                Displacement = randness;
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
        /// <param name="seed">
        /// Seed for the random number generator used to generate the landscape.
        /// </param>
        public static void MPD(float[] heights, float roughness, int? seed = null)
        {
            // Width of heights vector
            int width = heights.Length;

            // Random number generator
            Random random= seed.HasValue ? new Random(seed.Value) : new Random();

            // A queue to help us perform the midpoint displacement
            Queue<SubLine> queue = new Queue<SubLine>();

            // Enqueue the first line
            queue.Enqueue(new SubLine(0, width - 1, 1));

            // Determine the start and end heights of the first line
            heights[0] = random.Range(-roughness, roughness);
            heights[width - 1] = random.Range(-roughness, roughness);

            // Perform midpoint displacement on all sublines until no more
            // sublines are possible
            while (queue.Count >  0)
            {
                // Process next subline
                SubLine subLine = queue.Dequeue();

                // Obtain center of current subline
                int center = (subLine.Left + subLine.Right) / 2;

                // Set height of current subline as the mean between its
                // endpoints
                heights[center] =
                    (heights[subLine.Left] + heights[subLine.Right]) / 2;

                // Add some randomness
                heights[center] +=
                    random.Range(-subLine.Displacement, subLine.Displacement);

                // If the current subline can be further divided...
                if (subLine.Right - subLine.Left >  2)
                {
                    // ...divide it and add resulting sublines to the queue for
                    // future processing
                    queue.Enqueue(new SubLine(
                        subLine.Left, center, subLine.Displacement * roughness));
                    queue.Enqueue(new SubLine(
                        center, subLine.Right, subLine.Displacement * roughness));
                }
            }
        }
    }
}