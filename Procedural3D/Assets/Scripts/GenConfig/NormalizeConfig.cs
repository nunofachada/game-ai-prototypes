/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using LibGameAI.PCG;

namespace GameAIPrototypes.ProceduralLandscape.GenConfig
{
    public class NormalizeConfig : AbstractGenConfig
    {
        [SerializeField]
        [Range(0, 1)]
        private float maxAltitude = 0.1f;

        public override void Generate(float[,] heights)
        {
            int width = heights.GetLength(0);
            int height = heights.GetLength(1);
            float min = 0, max = float.NegativeInfinity;

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
        }
    }
}