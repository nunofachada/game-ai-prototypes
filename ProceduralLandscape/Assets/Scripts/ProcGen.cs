/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using UnityEngine;
using AIUnityExamples.ProceduralLandscape.GenConfig;
using NaughtyAttributes;

namespace AIUnityExamples.ProceduralLandscape
{
    public class ProcGen : MonoBehaviour
    {
        // ///////// //
        // Constants //
        // ///////// //
        private const string generalStr = ":: General parameters ::";
        private const string generatorStr = ":: Generator parameters ::";

        // ////////////////// //
        // General parameters //
        // ////////////////// //

        [BoxGroup(generalStr)]
        [SerializeField]
        [Range(0, 1)]
        private float maxAltitude = 0.1f;

        // //////////////////// //
        // Generator parameters //
        // //////////////////// //

        [BoxGroup(generatorStr)]
        [SerializeField]
        [Dropdown(nameof(GeneratorNames))]
        [OnValueChanged(nameof(OnChangeGeneratorName))]
        private string generatorName;

        [BoxGroup(generatorStr)]
        [SerializeField]
        [Expandable]
        [OnValueChanged(nameof(OnChangeGeneratorType))]
        private AbstractGenConfig generatorConfig;

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
                    generatorNames = GenConfigManager.Instance.GeneratorNames;
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

        // /////// //
        // Methods //
        // ////// //

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
                generatorName = GenConfigManager.Instance.GetNameFromType(
                    generatorConfig.GetType());
            }
        }

        // Callback invoked when user changes generation method name in editor
        private void OnChangeGeneratorName()
        {
            // Make sure gen. method type is updated accordingly
            System.Type genConfigType =
                GenConfigManager.Instance.GetTypeFromName(generatorName);
            generatorConfig = AbstractGenConfig.GetInstance(genConfigType);
        }

        [Button("Generate", enabledMode: EButtonEnableMode.Editor)]
        private void Generate()
        {

            Terrain terrain = GetComponent<Terrain>();
            int width = terrain.terrainData.heightmapResolution;
            int height = terrain.terrainData.heightmapResolution;

            float[,] heights = new float[width, height];

            float min = 0, max = float.NegativeInfinity;

            // Apply the generation
            generatorConfig.Generate(heights);

            // Post-processing / normalizing
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (heights[i, j] < min) min = heights[i, j];
                    else if (heights[i, j] > max) max = heights[i, j];
                }
            }

            if (min < 0 || max > maxAltitude)
            {
                //float modif = (max - min) / maxAltitude;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        heights[i, j] =
                            maxAltitude * (heights[i, j] - min) / (max - min);
                    }
                }
            }

            // Apply terrain heights
            terrain.terrainData.SetHeights(0, 0, heights);
        }

        [Button("Clear", enabledMode: EButtonEnableMode.Editor)]
        private void Clear()
        {
            Terrain terrain = GetComponent<Terrain>();
            int width = terrain.terrainData.heightmapResolution;
            int height = terrain.terrainData.heightmapResolution;

            float[,] heights = new float[width, height];

            // Apply terrain heights
            terrain.terrainData.SetHeights(0, 0, heights);

        }

        // Update is called once per frame
        // We use to orbit the camera around the cube grid
        private void Update()
        {
            Terrain terrain = GetComponent<Terrain>();
            float width = terrain.terrainData.size.x;
            float height = terrain.terrainData.size.z;

            // Determine the radius which the camera will orbit around the
            // center of the grid
            float radius = (width + height) / 3f;

            // Camera rotation speed
            float cameraRotationSpeed = 0.25f;


            // Determine camera position in its orbit around the center of the grid
            Camera.main.transform.position = new Vector3(width / 2, 1700 * maxAltitude, height / 2)
                + radius * new Vector3(
                    Mathf.Sin(cameraRotationSpeed * Time.time),
                    0,
                    Mathf.Cos(cameraRotationSpeed * Time.time));

            // Make camera look at center of the grid
            Camera.main.transform.LookAt(new Vector3(width / 2, 0, height / 2));

        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.DrawLine(Camera.main.transform.position, new Vector3(500, 0, 500));
        //     Gizmos.DrawSphere(new Vector3(500, 1000 * maxAltitude, 500), 20);
        // }

        // private void OnGUI()
        // {
        //     GUI.Label(new Rect(10, 10, 100, 20), $"Dist: {(Camera.main.transform.position - new Vector3(500, 1500 * maxAltitude, 500)).magnitude}");
        // }

    }
}