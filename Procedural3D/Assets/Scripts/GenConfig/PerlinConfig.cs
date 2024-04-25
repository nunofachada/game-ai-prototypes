/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace GameAIPrototypes.ProceduralLandscape.GenConfig
{
    public class PerlinConfig : AbstractGenConfig
    {
        [SerializeField]
        private float tileSize = 10f;

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