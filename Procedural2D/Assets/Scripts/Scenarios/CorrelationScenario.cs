/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using NaughtyAttributes;
using Random = System.Random;

namespace AIUnityExamples.Procedural2D.Scenarios
{
    public class CorrelationScenario : AbstractScenario
    {
        [SerializeField]
        [Dropdown(nameof(RandomNames))]
        private string randGenerator;

        [SerializeField] [Range(1, 1 << 10)]
        private int generatorCount;

        // Names of known scenarios
        [NonSerialized]
        private string[] randomNames;

        // Get scenario names
        private string[] RandomNames
        {
            get
            {
                // Did we initialize scenario names already?
                if (randomNames is null)
                    randomNames = PRNGHelper.KnownPRNGs;

                // Return existing scenario names
                return randomNames;
            }
        }

        public override void Generate(Color[] pixels, int width, int height)
        {
            int[] seeds = new int[generatorCount];
            for (int i = 0; i < generatorCount; i++) seeds[i] = i;

            // Array of random number generators
            Random[] rnd = new Random[seeds.Length];

            // Instantiate the random number generators
            for (int i = 0; i < seeds.Length; i++)
            {
                rnd[i] = PRNGHelper.PRNGInstance(randGenerator, seeds[i]);
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