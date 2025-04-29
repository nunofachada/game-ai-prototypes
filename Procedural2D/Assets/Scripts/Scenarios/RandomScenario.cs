/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using System.Collections;


namespace GameAIPrototypes.Procedural2D.Scenarios
{
    public class RandomScenario : StochasticScenario
    {
        [SerializeField]
        [Range(0, 1)]
        private float blackChance = 0.5f;

        public override IEnumerator Generate(Color[] pixels, int xDim, int yDim)
        {
            base.Generate(pixels, xDim, yDim);

            // Fill vector of pixels with random black or white pixels
            for (int y = 0; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    // Get a random value between 0 and 1
                    double val = PRNG.NextDouble();

                    // Determine color based on obtained random value
                    Color color = val > blackChance ? Color.white : Color.black;

                    // Set color in pixels array
                    pixels[y * xDim + x] = color;
                }
            }
            return null;
        }
    }
}