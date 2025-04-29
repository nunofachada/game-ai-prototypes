/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    /// <summary>
    /// The base class for all scenarios.
    /// </summary>
    public abstract class AbstractScenario : ScriptableObject
    {
        // Location of the scenarios (i.e., of the serialized scriptable objects
        // representing the scenarios) within the resources folder
        private const string configFolder = "ScenarioConfig";

        // Assets file extension
        private const string aExt = "asset";

        // Path to the scenarios, including the assets folder
        private static string FullConfigFolder =>
            $"Assets/Resources/{configFolder}";

        /// <summary>
        /// Procedurally generate a 2D image by filling a vector of pixels.
        /// </summary>
        /// <param name="pixels">
        /// Pixels vector to fill with procedurally generated data.
        /// </param>
        /// <param name="xDim">Image width.</param>
        /// <param name="yDim">Image height.</param>
        public abstract IEnumerator Generate(Color[] pixels, int xDim, int yDim);

        /// <summary>
        /// Returns an instance of the scenario.
        /// </summary>
        /// <param name="type">The concrete type of the scenario.</param>
        /// <returns>An instance of the scenario.</returns>
        public static AbstractScenario GetInstance(Type type)
        {
            // Name of asset
            string assetName = $"{type.Name}";

            // Try to load a saved scenario of this type
            AbstractScenario scenario =
                Resources.Load<AbstractScenario>($"{configFolder}/{assetName}");

            // If there's no saved scenario of this type, create a new one
            if (scenario is null)
            {
                // Create an instance of the scenario
                scenario = CreateInstance(type) as AbstractScenario;

                // Set this scenario as an asset for later loading
                AssetDatabase.CreateAsset(
                    scenario, $"{FullConfigFolder}/{assetName}.{aExt}");
            }

            // Return the instance of the scenario
            return scenario;
        }

        /// <summary>
        /// Fill the image with the specified color.
        /// </summary>
        /// <param name="pixels">Image to fill.</param>
        /// <param name="color">Color to use.</param>
        protected virtual void Fill(Color[] pixels, Color color)
        {
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        }
    }
}