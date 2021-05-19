/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Reflection;
using UnityEngine;
using NaughtyAttributes;
using LibGameAI.Util;
using Random = System.Random;

namespace AIUnityExamples.Procedural2D.Scenarios
{
    public class CorrelationScenario : AbstractScenario
    {
        [SerializeField]
        [Dropdown(nameof(RandomNames))]
        private string randGenerator;

        [SerializeField]
        private int[] seeds = { 1, 2, 3 };

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
                {
                    // Spin up the scenario class manager with custom
                    // scenario naming and get the scenario names
                    randomNames = ClassManager<Random>
                        .Instance
                        .FilterTypes(t =>
                            t.FullName.Contains("System") ||
                            t.FullName.Contains("LibGameAI.PRNG"))
                        .ReplaceNames(n => n.Remove(0, n.LastIndexOf(".") + 1))
                        .ClassNames;

                    // Sort them
                    Array.Sort(randomNames);
                }

                // Return existing scenario names
                return randomNames;
            }
        }

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

            Type rndType = ClassManager<Random>.Instance.GetTypeFromName(randGenerator);
            ConstructorInfo rndConstr = rndType.GetConstructor(new Type[] { typeof(int) });

            if (rndConstr is null)
            {
                Debug.LogWarning(
                    $"The {rndType} PRNG does not have a constructor which accepts int.");
                return;
            }

            // Instantiate the random number generators
            for (int i = 0; i < seeds.Length; i++)
            {
                rnd[i] = rndConstr.Invoke(new object[] { seeds[i] }) as Random;
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