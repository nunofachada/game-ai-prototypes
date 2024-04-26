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
    public class SandpileConfig : StochasticGenConfig
    {

        [SerializeField]
        private float threshold = 4;

        [SerializeField]
        private float increment = 1;

        [SerializeField]
        private float decrement = 4;

        [SerializeField]
        private float grainDropDensity = 0.02f;

        [SerializeField]
        private bool staticDrop = false;

        [SerializeField]
        private bool stochastic = true;

        public override float[,] Generate(float[,] prev_heights)
        {
            InitPRNG();

            int xdim = prev_heights.GetLength(0);
            int ydim = prev_heights.GetLength(1);
            float[,] sandpile_heights = new float[xdim, ydim];

            Landscape.Sandpile(sandpile_heights, threshold, increment, decrement,
                grainDropDensity, staticDrop, stochastic, PRNG.Next,
                PRNG.NextDouble);

            return sandpile_heights;
        }
    }
}