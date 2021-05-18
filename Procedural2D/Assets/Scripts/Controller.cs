/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using AIUnityExamples.Procedural2D.Scenarios;

namespace AIUnityExamples.Procedural2D
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private RawImage image;

        [SerializeField]
        [Range(0.1f, 100f)]
        private float scale = 1;

        [SerializeField]
        [Dropdown(nameof(ScenarioNames))]
        [OnValueChanged(nameof(OnChangeScenarioName))]
        private string scenarioName;

        [SerializeField]
        [Expandable]
        [OnValueChanged(nameof(OnChangeScenarioType))]
        private AbstractScenario scenarioConfig;

        // Names of known scenarios
        [System.NonSerialized]
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
                    // Get scenario names
                    scenarioNames = ScenarioManager.Instance.ScenarioNames;
                    // Sort them
                    System.Array.Sort(scenarioNames);
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
                scenarioName = ScenarioManager.Instance.GetNameFromType(
                    scenarioConfig.GetType());
            }
        }

        // Callback invoked when user changes scenario name in editor
        private void OnChangeScenarioName()
        {
            // Make sure scenario type is updated accordingly
            System.Type scenarioType =
                ScenarioManager.Instance.GetTypeFromName(scenarioName);
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

        [Button("Save", enabledMode: EButtonEnableMode.Editor)]
        private void Save()
        {
            // Encode texture into PNG
            byte[] bytes = (image.texture as Texture2D)?.EncodeToPNG();

            // Get a temporary file name
            string filename = Path.Combine(
                Path.GetTempPath(), Path.GetRandomFileName() + ".png");

            // Write to a file in the project folder
            File.WriteAllBytes(filename, bytes);

            // Inform user of where file was saved to
            Debug.Log($"Image saved as {filename}");
        }

        [Button("Clear", enabledMode: EButtonEnableMode.Editor)]
        private void Clear()
        {
            image.texture = null;
        }
    }
}