/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    public class CA1DScenario : StochasticScenario
    {
        [SerializeField]
        private byte rule = 30;

        [SerializeField]
        private bool singleCenterPixel = true;

        private const int bitsInByte = 8;

        protected override bool RandActive => !singleCenterPixel;

        // TODO Move CA1D generic functionality to libGameAI
        public override void Generate(Color[] pixels, int xDim, int yDim)
        {
            base.Generate(pixels, xDim, yDim);

            // Array of colors to generate according to the three previous
            // bits (2^3 = 8 values)
            Color[] ruleList = new Color[bitsInByte];

            // Local copy of rule
            int localRule = rule;

            // Get colors to generate according to three previous bits
            for (int i = 0; i < bitsInByte; i++)
            {
                int bit = localRule & 1;
                ruleList[i] = bit == 1 ? Color.black : Color.white;
                localRule >>= 1;
            }

            // Initialize first (bottom) line
            if (singleCenterPixel)
            {
                for (int i = 0; i < xDim; i++)
                {
                    pixels[i] = Color.white;
                }

                // Put just one black pixel in the middle
                pixels[xDim / 2] = Color.black;
            }
            else
            {
                // Randomly initialize the bottom line
                for (int i = 0; i < xDim; i++)
                {
                    pixels[i] = PRNG.NextDouble() < 0.5 ? Color.white : Color.black;
                }
            }

            // Run cellular automata rule on image
            for (int y = 1; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    // Find indexes of rule bits in previous line
                    int i2 = x > 0 ? x - 1 : xDim - 1;
                    int i1 = x;
                    int i0 = x < xDim - 1 ? x + 1 : 0;

                    // Get bits in previous line
                    int v2 = pixels[(y - 1) * xDim + i2] == Color.black ? 1 : 0;
                    int v1 = pixels[(y - 1) * xDim + i1] == Color.black ? 1 : 0;
                    int v0 = pixels[(y - 1) * xDim + i0] == Color.black ? 1 : 0;

                    // Get rule index
                    int ruleIdx = (v2 << 2) + (v1 << 1) + v0;

                    // Apply rule to current position
                    pixels[y * xDim + x] = ruleList[ruleIdx];
                }
            }
        }
    }
}