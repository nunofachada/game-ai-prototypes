/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace GameAIPrototypes.ProceduralLandscape.GenConfig
{
    public class MultiplierConfig : AbstractGenConfig
    {
        [SerializeField]
        [Range(0, 1)]
        private float multiplier = 0.1f;

        public override void Generate(float[,] heights)
        {
            int width = heights.GetLength(0);
            int height = heights.GetLength(1);

            // Apply multiplier
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heights[i, j] *= multiplier;
                }
            }

        }
    }
}