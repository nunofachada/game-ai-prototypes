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
        private enum SeedOptions { SameSeeds, SequentialSeeds, RandomSeeds }

        [SerializeField]
        [Dropdown(nameof(RandomNames))]
        private string randGenerator;

        [SerializeField] [Range(1, 1 << 10)]
        private int generatorCount;

        [SerializeField]
        private SeedOptions seedOptions;

        [SerializeField]
        private int baseSeed;

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
            // Array of seeds for seeding the random number generator instances
            int[] seeds = new int[generatorCount];

            // Array of random number generator instances
            Random[] rnd = new Random[seeds.Length];

            // Seed generator for seeding the random number generator instances
            Func<int> seedGen = null;

            // Determine the method of obtaining seeds
            switch (seedOptions)
            {
                case SeedOptions.SameSeeds:
                    // Always use the same seed (bad!)
                    seedGen = () => baseSeed;
                    break;
                case SeedOptions.SequentialSeeds:
                    // Use incremental seeds
                    seedGen = () => baseSeed++;
                    break;
                case SeedOptions.RandomSeeds:
                    // Use random seeds
                    Random randSeeder =
                        PRNGHelper.PRNGInstance(randGenerator, baseSeed);
                    seedGen = () => randSeeder.Next();
                    break;
            }

            // Obtain seeds for the random number generator instances
            for (int i = 0; i < generatorCount; i++)
            {
                seeds[i] = seedGen?.Invoke() ?? 0;
            }

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