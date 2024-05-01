/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using UnityEngine;
using GameAIPrototypes.ProceduralLandscape.GenConfig;
using NaughtyAttributes;
using LibGameAI.Util;

namespace GameAIPrototypes.ProceduralLandscape
{
    /// <summary>
    /// Represents and configures a PCG generator.
    /// </summary>
    public class Generator : MonoBehaviour
    {
        // ID
        private int id = -1;

        /// <summary>
        /// Returns a unique and sequential ID for each Generator script
        /// instance, based on the order the scripts appear in the PCG game
        /// object.
        /// </summary>
        /// <value>Unique Generator script ID.</value>
        public int ID
        {
            get
            {
                if (id < 0)
                {
                    Generator[] gens = GetComponents<Generator>();
                    for (int i = 0; i < gens.Length; i++)
                    {
                        if (this == gens[i])
                        {
                            id = i;
                            break;
                        }
                    }
                }
                return id;
            }
        }

        // //////////////////// //
        // Generator parameters //
        // //////////////////// //
        [SerializeField]
        [Dropdown(nameof(GeneratorNames))]
        [OnValueChanged(nameof(OnChangeGeneratorName))]
        private string generatorName;

        [SerializeField]
        [Expandable]
        [OnValueChanged(nameof(OnChangeGeneratorType))]
        private AbstractGenConfig generatorConfig;

        private enum PostProcess { None, Normalize, Scale }

        [SerializeField]
        private PostProcess postProcessing = PostProcess.None;

        [SerializeField]
        [ShowIf(nameof(PostNormalize))]
        private float maxHeight = 1;

        [SerializeField]
        [ShowIf(nameof(PostScaling))]
        private float scaleFactor = 1;

        public bool PostNormalize => postProcessing == PostProcess.Normalize;
        public bool PostScaling => postProcessing == PostProcess.Scale;

        public float MaxHeight => maxHeight;
        public float ScaleFactor => scaleFactor;

        public bool IsModifier => generatorConfig.IsModifier;

        // ///////////////////////////////////// //
        // Instance variables not used in editor //
        // ///////////////////////////////////// //

        // Names of known generation methods
        [System.NonSerialized]
        private string[] generatorNames;

        // ////////// //
        // Properties //
        // ////////// //

        // Get generation method names
        private ICollection<string> GeneratorNames
        {
            get
            {
                // Did we initialize generator names already?
                if (generatorNames is null)
                {
                    // Get generator names
                    generatorNames = ClassManager<AbstractGenConfig>
                        .Instance
                        .ReplaceNames(SimpleName)
                        .ClassNames;
                    // Sort them, but None always appears first
                    System.Array.Sort(
                        generatorNames,
                        (a, b) => a.Equals("None")
                            ? -1
                            : (b.Equals("None") ? 1 : a.CompareTo(b)));
                }

                // Return existing generator names
                return generatorNames;
            }
        }

        // /////////////// //
        // Private Methods //
        // /////////////// //

        // Callback invoked when user changes generator type in editor
        private void OnChangeGeneratorType()
        {
            if (generatorConfig is null)
            {
                // Cannot allow this field to be empty, so set it back to what
                // is specified in the generation method name
                Debug.Log(
                    $"The {nameof(generatorConfig)} field cannot be empty");
                OnChangeGeneratorName();
            }
            else
            {
                // Update generation method name accordingly to what is now set
                // in the generation configurator fields
                generatorName = ClassManager<AbstractGenConfig>
                    .Instance.GetNameFromType(generatorConfig.GetType());
            }
        }

        // Callback invoked when user changes generation method name in editor
        private void OnChangeGeneratorName()
        {
            // Make sure gen. method type is updated accordingly
            System.Type genConfigType = ClassManager<AbstractGenConfig>
                .Instance
                .GetTypeFromName(generatorName);
            generatorConfig = AbstractGenConfig
                .GetInstance(genConfigType, ID);
        }


        /// <summary>
        /// Apply generator on the given heightmap.
        /// </summary>
        /// <param name="heights">The current heightmap.</param>
        /// <returns>
        /// A new heightmap, possibly based on the current heighmap.
        /// </returns>
        public float[,] Generate(float[,] heights)
        {
            return generatorConfig.Generate(heights);
        }

        private void Reset()
        {
            generatorName = SimpleName(typeof(PostProcessingOnlyConfig).FullName);
            OnChangeGeneratorName();
        }

        /// <summary>
        /// Simplify the name of a generator by removing the namespace
        /// and the "Config" substring in the end.
        /// </summary>
        /// <param name="fqName">
        /// The fully qualified name of the generator.
        /// </param>
        /// <returns>
        /// The simplified name of the generator.
        /// </returns>
        private string SimpleName(string fqName)
        {
            string simpleName = fqName;

            // Strip namespace
            if (simpleName.Contains("."))
            {
                simpleName = fqName.Substring(fqName.LastIndexOf(".") + 1);
            }

            // Strip "Config"
            if (simpleName.EndsWith("Config"))
            {
                simpleName = simpleName.Substring(
                    0, simpleName.Length - "Config".Length);
            }

            // Return simple name
            return simpleName;
        }
    }
}