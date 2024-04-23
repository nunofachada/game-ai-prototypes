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
    public class CellularAutomataConfig : StochasticGenConfig
    {

        [SerializeField]
        private bool addFirst = false;
        [SerializeField]
        private int steps = 5;

        public override void Generate(float[,] heights)
        {
            int xdim = heights.GetLength(0);
            int ydim = heights.GetLength(1);

            int[] ca1 = new int[xdim * ydim];
            int[] ca2 = new int[xdim * ydim];
            int[] aux;

            CA2D.RandomFill(ca1, new int[] { 0, 1 }, new float[] { 0.5f, 0.5f }, () => (float)PRNG.NextDouble());

            for (int t = 0; t < steps; t++)
            {
                if (addFirst || t > 0)
                {
                    for (int i = 0; i < ydim; i++)
                    {
                        for (int j = 0; j < xdim; j++)
                        {
                            heights[j, i] += ca1[i * xdim + j];
                        }
                    }
                }

                CA2D.DoStep(ca1, ca2, xdim, ydim);

                aux = ca1;
                ca1 = ca2;
                ca2 = aux;
            }
            // for (int i = 0; i < ydim; i++)
            // {
            //     for (int j = 0; j < xdim; j++)
            //     {
            //         heights[j, i] = heights[j, i] * 0.0015f;
            //     }
            // }
        }
    }
}