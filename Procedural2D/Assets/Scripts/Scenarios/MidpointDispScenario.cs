/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using LibGameAI.PCG;
using LibGameAI.Util;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    public class MidpointDispScenario : StochasticScenario
    {
        [SerializeField]
        [Range(0, 0.5f)]
        private float imagePadding = 0.2f;

        [SerializeField]
        [Range(0, 1f)]
        private float roughness = 0.5f;

        public override void Generate(Color[] pixels, int width, int height)
        {
            base.Generate(pixels, width, height);

            // An array which will contain the procedurally generated heights
            float[] heights = new float[width];

            // Perform midpoint displacement
            Noise.MPD(heights, roughness, () => (float)PRNG.NextDouble());

            // Normalize line to better fit in image
            MMath.Normalize(
                heights,
                height * imagePadding,
                height - height * imagePadding - 1);

            // Fill image with white
            Fill(pixels, Color.white);

            // Draw 2D landscape on image
            for (int x = 0; x < width; x++)
            {
                // Place the pixel in the image
                pixels[(int)heights[x] * width + x] = Color.black;

                // Create a line between unconnected neighbors
                if (x > 0 && Math.Abs(heights[x - 1] - heights[x]) > 1)
                {
                    // Determine high and low neighbor points
                    int xHigh, xLow;
                    int toFill = Math.Abs((int)heights[x - 1] - (int)heights[x]) / 2;
                    int yHigh = Math.Max((int)heights[x - 1], (int)heights[x]);
                    int yLow = Math.Min((int)heights[x - 1], (int)heights[x]);
                    if (yHigh == (int)heights[x - 1])
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
    }
}