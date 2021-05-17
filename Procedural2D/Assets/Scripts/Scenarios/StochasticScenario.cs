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
        [EnableIf(nameof(RandActive))]
        private bool useSeed = false;

        [SerializeField]
        [EnableIf(EConditionOperator.And, nameof(useSeed), nameof(RandActive))]
        private int seed = 0;

        private Random random;

        protected Random PRNG => random;

        protected virtual bool RandActive => true;

        public override void Generate(Color[] pixels, int width, int height)
        {
            random = useSeed ? new Random(seed) : new Random();
        }
    }
}