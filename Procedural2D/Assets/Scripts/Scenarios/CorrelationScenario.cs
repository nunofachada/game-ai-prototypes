/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using Random = System.Random;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    public class CorrelationScenario : AbstractScenario
    {
        private enum SeedOptions { SameSeeds, SequentialSeeds, RandomSeeds }

        private const int MAX_COLORS = 9;
        private static readonly Color[] colors = new Color[MAX_COLORS]
        {
            Color.white, Color.black, Color.blue, Color.red, Color.green,
            Color.gray, Color.magenta, Color.yellow, Color.cyan
        };

        [SerializeField]
        [Dropdown(nameof(RandomNames))]
        private string randGenerator;

        [SerializeField] [Range(1, 1 << 10)]
        private int generatorCount;

        [SerializeField]
        private SeedOptions seedOptions;

        [SerializeField]
        private int baseSeed;

        [SerializeField] [Range(2, MAX_COLORS)]
        private int numberOfColors;

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
                    randomNames = PRNGHelper.Instance.KnownPRNGs;

                // Return existing scenario names
                return randomNames;
            }
        }

        public override IEnumerator Generate(Color[] pixels, int xDim, int yDim)
        {
            // Get a copy of the base seed
            int localBaseSeed = baseSeed;

            // Array of random number generator instances
            Random[] rnd = new Random[generatorCount];

            // Seed generator for seeding the random number generator instances
            Func<int> seedGen = null;

            // Determine the method of obtaining seeds
            switch (seedOptions)
            {
                case SeedOptions.SameSeeds:
                    // Always use the same seed (bad!)
                    seedGen = () => localBaseSeed;
                    break;
                case SeedOptions.SequentialSeeds:
                    // Use incremental seeds
                    seedGen = () => localBaseSeed++;
                    break;
                case SeedOptions.RandomSeeds:
                    // Use random seeds
                    Random randSeeder =
                        PRNGHelper.Instance.CreatePRNG(randGenerator, localBaseSeed);
                    seedGen = () => randSeeder.Next();
                    break;
            }

            // Instantiate the random number generators
            for (int i = 0; i < generatorCount; i++)
            {
                rnd[i] = PRNGHelper.Instance.CreatePRNG(
                    randGenerator, seedGen.Invoke());
            }

            // Fill vector of pixels with random black or white pixels
            for (int y = 0; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    // Get a random value to choose a color
                    int colorIndex = rnd[x % rnd.Length].Next(numberOfColors);

                    // Set color in pixels array
                    pixels[y * xDim + x] = colors[colorIndex];
                }
            }

            return null;
        }
    }
}