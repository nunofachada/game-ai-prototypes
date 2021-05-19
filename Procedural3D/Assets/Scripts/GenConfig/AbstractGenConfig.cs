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
        // within the resources folder
        private const string configFolder = "GenConfig";

        // Assets file extension
        private const string aExt = "asset";

        // ID separator
        private const string idSep = "___";

        // Path to the generation configurators, including the assets folder
        private static string FullConfigFolder =>
            $"Assets/Resources/{configFolder}";

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
        /// <param name="id">Unique ID of the generator script.</param>
        /// <returns>An instance of the generator configurator.</returns>
        public static AbstractGenConfig GetInstance(Type type, int id)
        {
            // Name of asset
            string assetName = $"{type.Name}{idSep}{id}";

            // Try to load a saved configurator of this type
            AbstractGenConfig genConfig =
                Resources.Load<AbstractGenConfig>($"{configFolder}/{assetName}");

            // If there's no saved configurator of this type, create a new one
            if (genConfig is null)
            {
                // Create an instance of the configurator
                genConfig = CreateInstance(type) as AbstractGenConfig;

                // Set this configurator as an asset for later loading
                AssetDatabase.CreateAsset(
                    genConfig, $"{FullConfigFolder}/{assetName}.{aExt}");
            }

            // Return the instance of the generator configurator
            return genConfig;
        }

        /// <summary>
        /// Clear unused configurator instances.
        /// </summary>
        public static void ClearUnusedInstances()
        {
            // Folders where to search for configurator instances
            string[] assetFolders = { FullConfigFolder };

            // Find how many generators are active
            int nGens = GameObject
                .Find("Controller")
                ?.GetComponents<Generator>()?.Length ?? 0;

            // For each configurator instance found....
            foreach (string asset in AssetDatabase.FindAssets("", assetFolders))
            {
                // Get path to configurator asset
                string path = AssetDatabase.GUIDToAssetPath(asset);

                // Find essential character positions in path
                int dotPos = path.LastIndexOf('.');
                int undPos = path.LastIndexOf(idSep);

                // Were both characters found in string?
                if (undPos >= 0 && dotPos >= 0)
                {
                    // Obtain substring with Generator ID
                    string idStr = path.Substring(
                        undPos + idSep.Length,
                        dotPos - (undPos + idSep.Length));

                    // Try and convert it to an int
                    if (int.TryParse(idStr, out int id))
                    {
                        // If if it's an ID of an existing generator, leave it
                        // be and go check the next one
                        if (id < nGens) continue;
                    }
                }

                // If we get here, delete the configurator asset
                AssetDatabase.DeleteAsset(path);
            }
        }
    }
}