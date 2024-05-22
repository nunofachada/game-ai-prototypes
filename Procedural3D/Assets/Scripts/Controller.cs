/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;
using GameAIPrototypes.ProceduralLandscape.GenConfig;

namespace GameAIPrototypes.ProceduralLandscape
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        [OnValueChanged(nameof(ResetGenerators))]
        private Terrain terrain;

        [SerializeField]
        [ShowAssetPreview]
        [ReadOnly]
	    private Texture2D heightmapTexture;

        [SerializeField]
        [Dropdown(nameof(validHeighmapResolutions))]
        [OnValueChanged(nameof(UpdateHeighmapResolution))]
        private int heightmapResolution = 513;

        private readonly DropdownList<int> validHeighmapResolutions = new()
        {
            {"33 x 33", 33},
            {"65 x 65", 65},
            {"129 x 129", 129},
            {"257 x 257", 257},
            {"513 x 513", 513},
            {"1025 x 1025", 1025},
            {"2049 x 2049", 2049},
            {"4097 x 4097", 4097},
        };

        [SerializeField]
        [HideInInspector]
        private float[,] heights;

        [SerializeField]
        [HideInInspector]
        private float maxHeight;

        private float[,] Heights
        {
            get
            {
                if (heights is null)
                {
                    heights = new float[heightmapResolution, heightmapResolution];
                    maxHeight = 0;
                }
                return heights;
            }
        }

        private void UpdateHeighmapResolution()
        {
            terrain.terrainData.heightmapResolution = heightmapResolution;
            terrain.terrainData.size = heightmapResolution * Vector3.one;
            heights = null;
            SceneView.lastActiveSceneView.Frame(terrain.terrainData.bounds);
        }

        // Normalizing
        private void Normalize(float[,] heightmap, float maxHeight = 1f)
        {
            int xdim = heightmap.GetLength(0);
            int ydim = heightmap.GetLength(1);

            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;


            for (int i = 0; i < xdim; i++)
            {
                for (int j = 0; j < ydim; j++)
                {
                    if (heightmap[i, j] < min) min = heightmap[i, j];
                    else if (heightmap[i, j] > max) max = heightmap[i, j];
                }
            }

            if (max - min != 0) // Avoid divide by zero
            {
                for (int i = 0; i < xdim; i++)
                {
                    for (int j = 0; j < ydim; j++)
                    {
                        heightmap[i, j] =
                            maxHeight * (heightmap[i, j] - min) / (max - min);
                    }
                }
            }

        }

        private void UpdateHeightMapTexture()
        {
            int xdim = Heights.GetLength(0);
            int ydim = Heights.GetLength(1);

            float[,] heights4texture = new float[xdim, ydim];
            Array.Copy(heights, heights4texture, heights.Length);
            Normalize(heights4texture);

            heightmapTexture = new(xdim, ydim);
            for (int x = 0; x < xdim; x++)
            {
                for (int y = 0; y < ydim; y++)
                {
                    float greyLevel = heights4texture[x, y];
                    heightmapTexture.SetPixel(x, y, new Color(greyLevel, greyLevel, greyLevel));
                }
            }
            heightmapTexture.Apply();
        }

        private void Zeros(float[,] heights2zero)
        {
            int xdim = heights2zero.GetLength(0);
            int ydim = heights2zero.GetLength(1);

            for (int i = 0; i < xdim; i++)
            {
                for (int j = 0; j < ydim; j++)
                {
                    heights2zero[i, j] = 0;
                }
            }
        }

        [Button("Generate", enabledMode: EButtonEnableMode.Editor)]
        private void Generate()
        {
            static float Avg(float[,] local_heights)
            {
                float total = 0;
                foreach (float f in local_heights) total += f;
                return total / local_heights.Length;
            }

            UpdateHeighmapResolution();
            Zeros(Heights);

            int xdim = heights.GetLength(0);
            int ydim = Heights.GetLength(1);

            // Apply the generation
            foreach (Generator g in GetComponents<Generator>())
            {
                float[,] partial_heights = g.Generate(heights);

                Debug.Log($"Generator: {g} [Avg={Avg(partial_heights)}]");

                if (g.PostNormalize)
                {
                    Normalize(partial_heights, g.MaxHeight);
                }
                else if (g.PostScale)
                {
                    // Apply scaling
                    for (int i = 0; i < xdim; i++)
                    {
                        for (int j = 0; j < ydim; j++)
                        {
                            partial_heights[i, j] *= g.ScaleFactor;
                        }
                    }
                }

                if (!g.IsModifier)
                {
                    // If this generator is not a modifier of the previous
                    // landscape, then we must add the newly generated landscape
                    // to the previous one
                    for (int i = 0; i < xdim; i++)
                    {
                        for (int j = 0; j < ydim; j++)
                        {
                            heights[i, j] += partial_heights[i, j];
                        }
                    }

                }
            }

            // Determine maximum height
            maxHeight = float.NegativeInfinity;

            for (int i = 0; i < xdim; i++)
            {
                for (int j = 0; j < ydim; j++)
                {
                    if (heights[i, j] > maxHeight) maxHeight = heights[i, j];
                }
            }

            // Apply terrain heights
            terrain.terrainData.SetHeights(0, 0, heights);

            // Update preview texture
            UpdateHeightMapTexture();
        }

        [Button("Save Heightmap to PNG", enabledMode: EButtonEnableMode.Editor)]
        private void SaveHeightmapToPNG()
        {
            if (heightmapTexture == null)
            {
                Debug.LogWarning(
                    "No heightmap texture defined. Run generate and try again.");
                return;
            }

            byte[] bytes = heightmapTexture.EncodeToPNG();

            // Ask user for file name to save
            string filename = EditorUtility.SaveFilePanel(
                "Save image as PNG",
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "heightmap.png",
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

        [Button("Flatten", enabledMode: EButtonEnableMode.Editor)]
        private void Flatten()
        {
            // Set heights to zero
            Zeros(Heights);

            maxHeight = 0;

            // Apply terrain heights
            terrain.terrainData.SetHeights(0, 0, heights);

            UpdateHeightMapTexture();
        }

        [Button("Add Generator", enabledMode: EButtonEnableMode.Editor)]
        private void AddGenerators()
        {
            gameObject.AddComponent<Generator>();
        }

        [Button("Reset Generators", enabledMode: EButtonEnableMode.Editor)]
        private void ResetGenerators()
        {
            terrain.transform.SetPositionAndRotation(
                new Vector3(0, 0, 0),
                Quaternion.identity);
            terrain.transform.localScale = new Vector3(1, 1, 1);

            foreach (Generator g in GetComponents<Generator>())
            {
                DestroyImmediate(g);
            }

            gameObject.AddComponent<Generator>();

            AbstractGenConfig.ClearUnusedInstances();

            terrain.terrainData.SetHeights(0, 0, Heights);
        }

        // Update is called once per frame
        // We use to orbit the camera around the cube grid
        private void Update()
        {
            float side1Len = terrain.terrainData.size.x;
            float side2Len = terrain.terrainData.size.z;

            // Determine the radius which the camera will orbit around the
            // center of the grid
            float radius = (side1Len + side2Len) / 4f;

            // Camera rotation speed
            float cameraRotationSpeed = 0.15f;

            // Camera height
            float camHeight = 4f * maxHeight * heightmapResolution;

            // Determine camera position in its orbit around the center of the grid
            Camera.main.transform.position = new Vector3(side1Len / 2, camHeight, side2Len / 2)
                + radius * new Vector3(
                    Mathf.Sin(cameraRotationSpeed * Time.time),
                    0,
                    Mathf.Cos(cameraRotationSpeed * Time.time));

            // Make camera look at center of the grid
            Camera.main.transform.LookAt(new Vector3(side1Len / 2, 0, side2Len / 2));
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