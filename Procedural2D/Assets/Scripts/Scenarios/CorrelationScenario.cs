/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using NaughtyAttributes;
using LibGameAI.PRNG;
using Random = System.Random;

namespace AIUnityExamples.Procedural2D.Scenarios
{
    public class CorrelationScenario : AbstractScenario
    {
        public enum PRNG { System, XorShift128, LCG }

        [SerializeField]
        private PRNG randGenerator = PRNG.System;

        [SerializeField]
        private int[] seeds = { 1, 2, 3 };

        public override void Generate(Color[] pixels, int width, int height)
        {
            if (seeds is null || seeds.Length == 0)
            {
                Debug.LogWarning(
                    $"The '{nameof(seeds)}' parameter must have at least one seed");
                return;
            }

            // Array of random number generators
            Random[] rnd = new Random[seeds.Length];

            // Instantiate the random number generators
            for (int i = 0; i < seeds.Length; i++)
            {
                switch (randGenerator)
                {
                    case PRNG.System:
                        rnd[i] = new Random(seeds[i]);
                        break;
                    case PRNG.XorShift128:
                        rnd[i] = new XorShift128(seeds[i]);
                        break;
                    case PRNG.LCG:
                        rnd[i] = new LCG(seeds[i]);
                        break;
                    default:
                        Debug.LogWarning("Unknown PRNG, using System's");
                        rnd[i] = new Random(seeds[i]);
                        break;
                }
            }

            // Fill vector of pixels with random black or white pixels
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // Get a random value between 0 and 1
                    double val = rnd[j % rnd.Length].NextDouble();

                    // Determine color based on obtained random value
                    Color color = val < 0.5 ? Color.white : Color.black;

                    // Set color in pixels array
                    pixels[i * width + j] = color;
                }
            }
        }
    }
}