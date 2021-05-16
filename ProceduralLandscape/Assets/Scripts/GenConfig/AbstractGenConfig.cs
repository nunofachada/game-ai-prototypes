/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using UnityEditor;

namespace AIUnityExamples.ProceduralLandscape.GenConfig
{
    /// <summary>
    /// The base class for all generator configurators.
    /// </summary>
    public abstract class AbstractGenConfig : ScriptableObject
    {
        // Location of the generation configurators (i.e., of the
        // serialized scriptable objects representing the configurators)
        private const string configFolder = "GenConfig";

        /// <summary>
        /// Generate a landscape.
        /// </summary>
        /// <param name="heights">
        /// Bidimensional array to fill with the generated landscape.
        /// </param>
        public abstract void Generate(float[,] heights);

        /// <summary>
        /// Returns an instance of the generator configurator.
        /// </summary>
        /// <param name="type">The concrete type of the configurator.</param>
        /// <returns>An instance of the generator configurator.</returns>
        public static AbstractGenConfig GetInstance(Type type)
        {
            // Try to load a saved configurator of this type
            AbstractGenConfig genConfig =
                Resources.Load<AbstractGenConfig>(
                    $"{configFolder}/{type.Name}");

            // If there's no saved configurator of this type, create a new one
            if (genConfig is null)
            {
                // Create an instance of the configurator
                genConfig = CreateInstance(type) as AbstractGenConfig;

                // Set this configurator as an asset for later loading
                AssetDatabase.CreateAsset(
                    genConfig,
                    $"Assets/Resources/{configFolder}/{type.Name}.asset");
            }

            // Return the instance of the generator configurator
            return genConfig;
        }

    }
}