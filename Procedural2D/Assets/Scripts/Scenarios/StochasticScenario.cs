/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using NaughtyAttributes;
using Random = System.Random;

namespace AIUnityExamples.Procedural2D.Scenarios
{
    public abstract class StochasticScenario : AbstractScenario
    {
        // public enum PRNG { System, XorShift128 }

        // [SerializeField]
        // private PRNG randomNumberGenerator = PRNG.System;

        [SerializeField]
        private bool useSeed = false;

        [SerializeField]
        [EnableIf(nameof(useSeed))]
        private int seed = 0;

        private Random random;

        protected Random PRNG => random;

        public override void Generate(Color[] pixels, int width, int height)
        {
            random = useSeed ? new Random(seed) : new Random();
        }
    }
}