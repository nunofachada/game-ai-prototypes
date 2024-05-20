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

    public class ThermalConfig : AbstractGenConfig
    {
        [SerializeField]
        private float threshold = 0.01f;
        [SerializeField]
        private int iterations = 10;
        [SerializeField]
        private bool toroidal = true;

        public override bool IsModifier => true;

        public override float[,] Generate(float[,] heights)
        {
            // Apply thermal erosion
            Landscape.ThermalErosion(heights, threshold, iterations, toroidal);

            return heights;
        }
    }
}