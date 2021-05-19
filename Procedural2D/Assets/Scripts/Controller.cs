/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using NaughtyAttributes;
using LibGameAI.Util;
using AIUnityExamples.Procedural2D.Scenarios;

namespace AIUnityExamples.Procedural2D
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private RawImage image;

        [SerializeField]
        [Dropdown(nameof(ScenarioNames))]
        [OnValueChanged(nameof(OnChangeScenarioName))]
        private string scenarioName;

        [SerializeField]
        [Expandable]
        [OnValueChanged(nameof(OnChangeScenarioType))]
        private AbstractScenario scenarioConfig;

        [SerializeField]
        [Range(0.1f, 100f)]
        private float scale = 1;

        // Names of known scenarios
        [NonSerialized]
        private string[] scenarioNames;

        // ////////// //
        // Properties //
        // ////////// //

        // Get scenario names
        private ICollection<string> ScenarioNames
        {
            get
            {
                // Did we initialize scenario names already?
                if (scenarioNames is null)
                {
                    // Spin up the scenario class manager with custom
                    // scenario naming and get the scenario names
                    scenarioNames = ClassManager<AbstractScenario>
                        .Instance
                        .ReplaceNames(SimplifyName)
                        .ClassNames;

                    // Sort them
                    Array.Sort(scenarioNames);
                }

                // Return existing scenario names
                return scenarioNames;
            }
        }

        // /////////////// //
        // Private Methods //
        // /////////////// //

        // Callback invoked when user changes scenario type in editor
        private void OnChangeScenarioType()
        {
            if (scenarioConfig is null)
            {
                // Cannot allow this field to be empty, so set it back to what
                // is specified in the scenario name
                Debug.Log(
                    $"The {nameof(scenarioConfig)} field cannot be empty");
                OnChangeScenarioName();
            }
            else
            {
                // Update scenario name accordingly to what is now set
                // in the generation configurator fields
                scenarioName = ClassManager<AbstractScenario>
                    .Instance
                    .GetNameFromType(scenarioConfig.GetType());
            }
        }

        // Callback invoked when user changes scenario name in editor
        private void OnChangeScenarioName()
        {
            // Make sure scenario type is updated accordingly
            Type scenarioType = ClassManager<AbstractScenario>
                .Instance
                .GetTypeFromName(scenarioName);
            scenarioConfig = AbstractScenario.GetInstance(scenarioType);
        }

        [Button("Generate", enabledMode: EButtonEnableMode.Editor)]
        private void Generate()
        {
            // Image width and height
            Rect rect = image.GetPixelAdjustedRect();
            int width = (int)(rect.width / scale);
            int height = (int)(rect.height / scale);

            //Debug.Log($"{width} {height}");

            // Create a vector of pixels
            Color[] pixels = new Color[width * height];

            // Texture to show on screen, to be randomly created
            Texture2D texture = new Texture2D(width, height);

            // Generate scenario
            scenarioConfig.Generate(pixels, width, height);

            // Set and apply texture pixels
            texture.SetPixels(pixels);
            texture.Apply();

            // Place texture in image
            image.texture = texture;
        }

        [Button("Save Image", enabledMode: EButtonEnableMode.Editor)]
        private void SaveImage()
        {
            // Encode texture into PNG
            byte[] bytes = (image.texture as Texture2D)?.EncodeToPNG();

            // Ask user for file name to save
            string filename = EditorUtility.SaveFilePanel(
                "Save image as PNG",
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                $"{scenarioName.ToLower()}.png",
                "png");

            // Write to a file in the project folder
            if (!string.IsNullOrEmpty(filename))
            {
                // Save image to file
                File.WriteAllBytes(filename, bytes);

                // Inform user of where image was saved to
                Debug.Log($"Image saved as {filename}");
            }
        }

        [Button("Clear", enabledMode: EButtonEnableMode.Editor)]
        private void Clear()
        {
            image.texture = null;
        }

        /// <summary>
        /// Simplify the name of a scenario by removing the namespace
        /// and the "Scenario" substring in the end.
        /// </summary>
        /// <param name="fqName">
        /// The fully qualified name of the scenario.
        /// </param>
        /// <returns>
        /// The simplified name of the scenario.
        /// </returns>
        public static string SimplifyName(string fqName)
        {
            string simpleName = fqName;

            // Strip namespace
            if (simpleName.Contains("."))
            {
                simpleName = fqName.Substring(fqName.LastIndexOf(".") + 1);
            }

            // Strip "Config"
            if (simpleName.EndsWith("Scenario"))
            {
                simpleName = simpleName.Substring(
                    0, simpleName.Length - "Scenario".Length);
            }

            // Return simple name
            return simpleName;
        }
    }
}