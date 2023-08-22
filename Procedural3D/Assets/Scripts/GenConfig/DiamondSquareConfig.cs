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

    public class DiamondSquareConfig : StochasticGenConfig
    {
        [SerializeField]
        private float maxInitHeight = 1f;
        [SerializeField]
        private float roughness = 0.5f;

        public override void Generate(float[,] heights)
        {
            Landscape.DiamondSquare(heights, maxInitHeight, roughness, () => (float)PRNG.NextDouble());
        }
    }
}