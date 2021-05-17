/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace AIUnityExamples.Procedural2D.Scenarios
{
    public class CA1DScenario : StochasticScenario
    {
        [SerializeField]
        private byte rule = 30;

        [SerializeField]
        private bool singleCenterPixel = true;

        private const int bitsInByte = 8;

        protected override bool RandActive => !singleCenterPixel;

        public override void Generate(Color[] pixels, int width, int height)
        {
            base.Generate(pixels, width, height);

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
                // Put just one black pixel in the middle
                pixels[width / 2] = Color.black;
            }
            else
            {
                // Randomly initialize the bottom line
                for (int i = 0; i < width; i++)
                {
                    pixels[i] = PRNG.NextDouble() < 0.5 ? Color.white : Color.black;
                }
            }

            // Run cellular automata rule on image
            for (int i = 1; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // Find indexes of rule bits in previous line
                    int i2 = j > 0 ? j - 1 : width - 1;
                    int i1 = j;
                    int i0 = j < width - 1 ? j + 1 : 0;

                    // Get bits in previous line
                    int v2 = pixels[(i - 1) * width + i2] == Color.black ? 1 : 0;
                    int v1 = pixels[(i - 1) * width + i1] == Color.black ? 1 : 0;
                    int v0 = pixels[(i - 1) * width + i0] == Color.black ? 1 : 0;

                    // Get rule index
                    int ruleIdx = (v2 << 2) + (v1 << 1) + v0;

                    // Apply rule to current position
                    pixels[i * width + j] = ruleList[ruleIdx];
                }
            }
        }
    }
}