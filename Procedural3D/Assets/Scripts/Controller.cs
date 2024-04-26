/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using GameAIPrototypes.ProceduralLandscape.GenConfig;
using NaughtyAttributes;

namespace GameAIPrototypes.ProceduralLandscape
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        [OnValueChanged(nameof(ResetConfiguration))]
        private Terrain terrain;

        [SerializeField]
        [HideInInspector]
        private float[,] heights;

        [SerializeField]
        [HideInInspector]
        private float maxHeight;

        [SerializeField]
        [HideInInspector]
        private int tSide;

        private float[,] Heights
        {
            get
            {
                if (heights is null)
                {
                    tSide = terrain.terrainData.heightmapResolution;
                    heights = heights = new float[tSide, tSide];
                    maxHeight = 0;
                }
                return heights;
            }
        }

        [Button("Generate", enabledMode: EButtonEnableMode.Editor)]
        private void Generate()
        {
            Flatten();

            int xdim = Heights.GetLength(0);
            int ydim = Heights.GetLength(1);

            // Apply the generation
            foreach (Generator g in GetComponents<Generator>())
            {
                float[,] partial_heights = g.Generate(heights);

                if (g.PostNormalize)
                {
                    float min = float.PositiveInfinity;
                    float max = float.NegativeInfinity;

                    // Normalizing
                    for (int i = 0; i < xdim; i++)
                    {
                        for (int j = 0; j < ydim; j++)
                        {
                            if (partial_heights[i, j] < min) min = partial_heights[i, j];
                            else if (partial_heights[i, j] > max) max = partial_heights[i, j];
                        }
                    }

                    for (int i = 0; i < xdim; i++)
                    {
                        for (int j = 0; j < ydim; j++)
                        {
                            partial_heights[i, j] =
                                g.MaxHeight * (partial_heights[i, j] - min) / (max - min);
                        }
                    }
                }
                else if (g.PostMultiply)
                {
                    // Apply multiplier
                    for (int i = 0; i < xdim; i++)
                    {
                        for (int j = 0; j < ydim; j++)
                        {
                            partial_heights[i, j] *= g.Multiplier;
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

            // Determine minimum and maximum heights
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
        }

        [Button("Flatten", enabledMode: EButtonEnableMode.Editor)]
        private void Flatten()
        {
            for (int i = 0; i < Heights.GetLength(0); i++)
            {
                for (int j = 0; j < heights.GetLength(1); j++)
                {
                    heights[i, j] = 0;
                }
            }

            maxHeight = 0;

            // Apply terrain heights
            terrain.terrainData.SetHeights(0, 0, heights);
        }

        [Button("Reset Configuration", enabledMode: EButtonEnableMode.Editor)]
        private void ResetConfiguration()
        {
            terrain.transform.position = new Vector3(0, 0, 0);
            terrain.transform.rotation = Quaternion.identity;
            terrain.transform.localScale = new Vector3(1, 1, 1);

            foreach (Generator g in GetComponents<Generator>())
            {
                DestroyImmediate(g);
            }

            gameObject.AddComponent<Generator>();
            gameObject.AddComponent<Generator>();
            gameObject.AddComponent<Generator>().SetAsNormalizer();

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
            float radius = (side1Len + side2Len) / 3f;

            // Camera rotation speed
            float cameraRotationSpeed = 0.15f;

            // Camera height
            float camHeight = Mathf.Max(1500 * maxHeight, 100);

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