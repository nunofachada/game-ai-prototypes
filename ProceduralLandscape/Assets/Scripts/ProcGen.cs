/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using UnityEngine;
using LibGameAI.ProcGen;
using NaughtyAttributes;

namespace AIUnityExamples.ProceduralLandscape
{
    public class ProcGen : MonoBehaviour
    {

        [Header("General parameters")]

        [SerializeField]
        private int seed = 1234;

        [SerializeField]
        [Range(0, 1)]
        private float maxAltitude = 0.1f;

        [Header("Perlin noise")]

        [SerializeField]
        private bool perlinNoise = true;

        [SerializeField]
        private float tileSize = 10f;

        [Header("Fault parameters")]

        [SerializeField]
        private int numFaults = 0;

        [SerializeField]
        [Range(0, 1)]
        private float meanDepth = 0.01f;

        [SerializeField]
        private float decreaseDistance = 0f;

        [Header("Sandpile")]

        [SerializeField]
        private bool sandpile = false;

        [SerializeField]
        private float threshold = 4;

        [SerializeField]
        private float increment = 1;

        [SerializeField]
        private float decrement = 4;

        [SerializeField]
        private float grainDropDensity = 10;

        [SerializeField]
        private bool staticDrop = true;
        [SerializeField]
        private bool stochastic = true;

        // [Header("Thermal erosion parameters")]

        // [SerializeField]
        // [Range(0, 1)]
        // private float maxHeight = 0;

        [Button("Generate", enabledMode: EButtonEnableMode.Editor)]
        private void Generate()
        {
            System.Random random = new System.Random(seed);

            Terrain terrain = GetComponent<Terrain>();
            int width = terrain.terrainData.heightmapResolution;
            int height = terrain.terrainData.heightmapResolution;

            float[,] heights = new float[width, height];

            float min = 0, max = float.NegativeInfinity;

            // Apply faults
            for (int i = 0; i < numFaults; i++)
            {
                Landscape.FaultModifier(
                    heights, meanDepth, () => (float)random.NextDouble(),
                    decreaseDistance);
            }

            // Apply Perlin noise
            if (perlinNoise)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        heights[i, j] += Mathf.PerlinNoise(
                            tileSize * i / (float)width,
                            tileSize * (float)j / height);
                    }
                }
            }

            // Apply sandpile
            if (sandpile)
            {
                Landscape.Sandpile(heights, threshold, increment, decrement, grainDropDensity, staticDrop, stochastic, random.Next, random.NextDouble);
            }


            // // Apply thermal erosion (not working atm)
            // if (maxHeight > 0)
            // {
            //     Landscape.ThermalErosion(heights, maxHeight);
            // }

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

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 20), $"Dist: {(Camera.main.transform.position - new Vector3(500, 1500 * maxAltitude, 500)).magnitude}");
        }

    }
}