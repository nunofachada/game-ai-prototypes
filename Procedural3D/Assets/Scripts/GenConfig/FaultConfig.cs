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
    public class FaultConfig : StochasticGenConfig
    {
        [SerializeField]
        private int numFaults = 10;

        [SerializeField]
        [Range(0, 1)]
        private float meanDepth = 0.01f;

        [SerializeField]
        private float decreaseDistance = 0f;

        public override float[,] Generate(float[,] prev_heights)
        {
            int xdim = prev_heights.GetLength(0);
            int ydim = prev_heights.GetLength(1);
            float[,] flt_heights = new float[xdim, ydim];

            // Apply faults
            for (int i = 0; i < numFaults; i++)
            {
                Landscape.FaultModifier(
                    flt_heights, meanDepth, () => (float)PRNG.NextDouble(),
                    decreaseDistance);
            }

            return flt_heights;
        }
    }
}