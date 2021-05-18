/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExamples.Procedural2D.Scenarios
{
    public class MidpointDispScenario : StochasticScenario
    {
        [SerializeField]
        [Range(0, 0.5f)]
        private float topBottomPadding = 0.2f;

        // Helper struct for line data
        private struct LineData
        {
            public int Left { get; }
            public int Right { get; }
            public int Randness { get; }

            public LineData(int start, int width, int randness)
            {
                Left = start;
                Right = width;
                Randness = randness;
            }
        }

        public override void Generate(Color[] pixels, int width, int height)
        {
            base.Generate(pixels, width, height);

            // An array which will contain the procedurally generated heights
            int[] heights = new int[width];

            // A queue to help us perform the midpoint displacement
            Queue<LineData> queue = new Queue<LineData>();

            // Enqueue the first line
            queue.Enqueue(new LineData(0, width - 1, height));

            // Determine the start and end heights of the first line
            heights[0] = PRNG.Next(height);
            heights[width - 1] = PRNG.Next(height);

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
                heights[center] += PRNG.Next(-data.Randness, data.Randness + 1);

                // If the current subline can be further divided...
                if (data.Right - data.Left >  2)
                {
                    // ...divide it and add resulting sublines to the queue for
                    // future processing
                    queue.Enqueue(new LineData(data.Left, center, data.Randness / 2));
                    queue.Enqueue(new LineData(center, data.Right, data.Randness / 2));
                }
            }

            // Normalize line to better fit in image
            Normalize(
                heights,
                (int)(height * topBottomPadding),
                (int)(height - height * topBottomPadding));

            // Fill image with white
            Fill(pixels, Color.white);

            // Draw 2D landscape on image
            for (int x = 0; x < width; x++)
            {
                // Place the pixel in the image
                pixels[heights[x] * width + x] = Color.black;

                // Create a line between unconnected neighbors
                if (x > 0 && Math.Abs(heights[x - 1] - heights[x]) > 1)
                {
                    // Determine high and low neighbor points
                    int xHigh, xLow;
                    int toFill = Math.Abs(heights[x - 1] - heights[x]) / 2;
                    int yHigh = Math.Max(heights[x - 1], heights[x]);
                    int yLow = Math.Min(heights[x - 1], heights[x]);
                    if (yHigh == heights[x - 1])
                    {
                        xHigh = x - 1;
                        xLow = x;
                    }
                    else
                    {
                        xHigh = x;
                        xLow = x - 1;
                    }

                    // Vertically connect high and low neighbor points
                    for (int dy = 1; dy <= toFill; dy++)
                    {
                        pixels[(yHigh - dy) * width + xHigh] = Color.black;
                        if ((yHigh - dy) != (yLow + dy))
                            pixels[(yLow + dy) * width + xLow] = Color.black;
                    }
                }
            }
        }

        // Perform heights normalization to better fit in the image
        private void Normalize(int[] heights, int bottom, int top)
        {
            int min = heights[0];
            int max = heights[0];
            int origRange, newRange;

            // Determine minimum and maximum heights
            foreach (int h in heights)
            {
                if (h < min) min = h;
                else if (h > max) max = h;
            }

            // Determine original and new ranges
            origRange = max - min;
            newRange = top - bottom;

            // Perform actual normalization
            for (int i = 0; i < heights.Length; i++)
            {
                heights[i] = (heights[i] - min) * newRange / origRange + bottom;
            }
        }
    }
}