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
    public class FaultConfig : StochasticGenConfig
    {
        [SerializeField]
        private int numFaults = 0;

        [SerializeField]
        [Range(0, 1)]
        private float meanDepth = 0.01f;

        [SerializeField]
        private float decreaseDistance = 0f;

        public override void Generate(float[,] heights)
        {
            // Apply faults
            for (int i = 0; i < numFaults; i++)
            {
                Landscape.FaultModifier(
                    heights, meanDepth, () => (float)PRNG.NextDouble(),
                    decreaseDistance);
            }
        }
    }
}