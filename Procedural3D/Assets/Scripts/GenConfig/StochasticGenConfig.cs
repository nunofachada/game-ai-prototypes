/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using NaughtyAttributes;
using Random = System.Random;

namespace GameAIPrototypes.ProceduralLandscape.GenConfig
{
    public abstract class StochasticGenConfig : AbstractGenConfig
    {
        [SerializeField]
        private bool useSeed = false;

        [SerializeField]
        [ShowIf(nameof(useSeed))]
        private int seed = 0;

        private Random random;

        protected Random PRNG => random;

        protected void InitPRNG()
        {
            random = useSeed ? new Random(seed) : new Random();
        }
    }
}