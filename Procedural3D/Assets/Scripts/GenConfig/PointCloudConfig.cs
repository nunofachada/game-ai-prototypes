/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;
using NaughtyAttributes;

namespace GameAIPrototypes.ProceduralLandscape.GenConfig
{
    public class PointCloudConfig : AbstractGenConfig
    {
        [SerializeField]
        [OnPathChanged(nameof(PathChanged))]
        private FileSelector dataFilePath;

        [SerializeField]
        [ResizableTextArea]
        [ReadOnly]
        private string dataInfo = "";

        [SerializeField]
        [HideInInspector]
        private float[] dataInArray;

        private void PathChanged()
        {
            Debug.Log($"selected file: " + dataFilePath);

            if (dataFilePath?.Filename.Length != 0)
            {
                int? numDims = null;
                string line;
                Queue<float[]> dataInQueue = new Queue<float[]>();

                using StreamReader dataStream = new StreamReader(dataFilePath.Filename);
                while ((line = dataStream.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0 || line[0] == '#') continue;

                    string[] tokens = line.Split(',');
                    if (!numDims.HasValue)
                    {
                        if (tokens.Length < 2)
                        {
                            throw new InvalidDataException(
                                "Data must be bidimensional at least.");
                        }
                        numDims = tokens.Length;
                    }
                    else if (numDims.Value != tokens.Length)
                    {
                        throw new InvalidDataException("Data size mismatch.");

                    }

                    float[] dataRow = new float[numDims.Value];

                    for (int i = 0; i < numDims.Value; i++)
                    {
                        if (!float.TryParse(tokens[i], NumberStyles.Any,
                            CultureInfo.InvariantCulture, out dataRow[i]))
                        {
                            throw new InvalidDataException(
                                $"'{tokens[i]}' is not a real value.");
                        };
                    }
                    dataInQueue.Enqueue(dataRow);
                }

                int numPoints = dataInQueue.Count;
                dataInArray = new float[numDims.Value * numPoints];
                for (int i = 0; i < numPoints; i++)
                {
                    Array.Copy(
                        dataInQueue.Dequeue(), 0,
                        dataInArray, i * numDims.Value,
                        numDims.Value);
                }

                dataInfo = $"Dimensions: {numDims.Value}\nPoints: {numPoints}";
            }

        }

        private readonly float tileSize = 10f;

        public override float[,] Generate(float[,] prev_heights)
        {
            int width = prev_heights.GetLength(0);
            int height = prev_heights.GetLength(1);
            float[,] perlin_heights = new float[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlin_heights[i, j] += Mathf.PerlinNoise(
                        tileSize * i / width,
                        tileSize * j / height);
                }
            }

            return perlin_heights;
        }
    }
}