/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using UnityEngine;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    public class PerlinScenario : StochasticScenario
    {
        [SerializeField]
        private float span = 10f;

        [SerializeField]
        private List<Color> heightColors = new List<Color>()
        {
            new Color(0.22f, 0.52f, 0.81f), // Ocean blue
            new Color(0.76f, 0.70f, 0.50f), // Sand
            new Color(0.28f, 0.44f, 0.22f), // Grass
            new Color(0.45f, 0.29f, 0.22f), // Mountain brown
            new Color(0.95f, 0.96f, 0.98f)  // Snowy white
        };

        public override void Generate(Color[] pixels, int xDim, int yDim)
        {
            base.Generate(pixels, xDim, yDim);

            // Add some random x and y displacement, so we can generate
            // different landscapes with this scenario
            float xDisp = (float)PRNG.NextDouble() * span * xDim;
            float yDisp = (float)PRNG.NextDouble() * span * yDim;

            for (int y = 0; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    // Get the Perlin noise value for this pixel
                    float value = Mathf.PerlinNoise(
                        xDisp + span * y / xDim,
                        yDisp + span * x / yDim);

                    // Sometimes Perlin returns values above 1, so lets make
                    // sure that doesn't happen
                    value = Mathf.Clamp(value, 0f, 0.999f);

                    // Determine the color according to the height returned by
                    // the Perlin function
                    Color color = heightColors[(int)(value * heightColors.Count)];

                    // Set that color in the current pixel
                    pixels[y * xDim + x] = color;
                }
            }
        }
    }
}
