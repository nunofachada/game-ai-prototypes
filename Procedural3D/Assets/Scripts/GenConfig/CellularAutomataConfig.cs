/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using LibGameAI.PCG;
using System.Runtime.InteropServices.WindowsRuntime;
using NaughtyAttributes;

namespace GameAIPrototypes.ProceduralLandscape.GenConfig
{
    public class CellularAutomataConfig : StochasticGenConfig
    {

        [SerializeField]
        private CA2D.Rule rule = CA2D.Rule.Smooth44;
        [SerializeField]
        [Range(0, 1)]
        private float initialFill = 0.5f;

        [SerializeField]
        private bool addFirst = false;
        [SerializeField]
        private int steps = 5;
        [SerializeField]
        private bool toroidal = true;
        [SerializeField]
        [HideIf(nameof(toroidal))]
        private bool offGridBorderCellsAlive = false;

        public override float[,] Generate(float[,] prev_heights)
        {
            int xdim = prev_heights.GetLength(0);
            int ydim = prev_heights.GetLength(1);

            int[] ca1 = new int[xdim * ydim];
            int[] ca2 = new int[xdim * ydim];
            int[] aux;

            float[,] ca_heights = new float[xdim, ydim];

            CA2D.RandomFill(
                ca1,
                new int[] { 0, 1 },
                new float[] { 1 - initialFill, initialFill },
                () => (float)PRNG.NextDouble());

            for (int t = 0; t < steps; t++)
            {
                if (addFirst || t > 0)
                {
                    for (int i = 0; i < ydim; i++)
                    {
                        for (int j = 0; j < xdim; j++)
                        {
                            ca_heights[j, i] += ca1[i * xdim + j];
                        }
                    }
                }

                CA2D.DoStep(ca1, ca2, xdim, ydim, toroidal, offGridBorderCellsAlive ? 1 : 0, rule);

                aux = ca1;
                ca1 = ca2;
                ca2 = aux;
            }

            return ca_heights;
        }
    }
}