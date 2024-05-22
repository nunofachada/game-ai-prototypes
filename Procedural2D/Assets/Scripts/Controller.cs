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
using GameAIPrototypes.Procedural2D.Scenarios;

namespace GameAIPrototypes.Procedural2D
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
        private Vector2Int resolution = new Vector2Int(800, 600);

        [SerializeField]
        [Expandable]
        [OnValueChanged(nameof(OnChangeScenarioType))]
        private AbstractScenario scenarioConfig;

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
                    // Make sure PRNG helper is initialized
                    PRNGHelper.Instance.Init();

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
            // Texture where to place the scenario-generated pixels
            Texture2D texture;

            // Texture dimensions, by default equal to scenario resolution, but
            // this may change, to better fit the UI image
            int xTex = resolution.x;
            int yTex = resolution.y;
            int xEmpty = 0;
            int yEmpty = 0;

            // Vector of pixels, to be filled by the chosen scenario
            Color[] pixels = new Color[resolution.x * resolution.y];

            // Obtain aspect ratio of UI image
            Rect imgRatio = image.GetPixelAdjustedRect();

            // Compare aspect ratio of UI image and scenario pixels and
            // determine resolution of scenario texture to have same aspect
            // ratio as UI image
            if ((float)resolution.x / resolution.y > imgRatio.width / imgRatio.height)
            {
                yTex = (int)(xTex * imgRatio.height / imgRatio.width);
                yEmpty = yTex - resolution.y;
            }
            else if ((float)resolution.x / resolution.y < imgRatio.width / imgRatio.height)
            {
                xTex = (int)(yTex * imgRatio.width / imgRatio.height);
                xEmpty = xTex - resolution.x;
            }

            // Create texture
            texture = new Texture2D(xTex, yTex);
            texture.filterMode = FilterMode.Point;

            // Run scenario, generate pixels
            scenarioConfig.Generate(pixels, resolution.x, resolution.y);

            // Set texture pixels, copy from scenario, set remaining as black
            for (int y = 0; y < yTex; y++)
            {
                for (int x = 0; x < xTex; x++)
                {
                    Color pixel = Color.black;
                    int xPixel = x - xEmpty / 2;
                    int yPixel = y - yEmpty / 2;
                    if (xPixel >= 0 && xPixel < resolution.x && yPixel >= 0 && yPixel < resolution.y)
                    {
                        pixel = pixels[yPixel * resolution.x + xPixel];
                    }
                    texture.SetPixel(x, y, pixel);
                }
            }

            // Apply texture pixels (load them to the GPU)
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
        private static string SimplifyName(string fqName)
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