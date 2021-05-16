/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using LibGameAI.ProcGen;

namespace AIUnityExamples.ProceduralLandscape.GenConfig
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
        private float grainDropDensity = 10;

        [SerializeField]
        private bool staticDrop = true;

        [SerializeField]
        private bool stochastic = true;

        public override void Generate(float[,] heights)
        {
            Landscape.Sandpile(heights, threshold, increment, decrement,
                grainDropDensity, staticDrop, stochastic, PRNG.Next,
                PRNG.NextDouble);
        }
    }
}