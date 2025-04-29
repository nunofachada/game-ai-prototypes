/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using Random = System.Random;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    public abstract class StochasticScenario : AbstractScenario
    {
        [SerializeField]
        [EnableIf(nameof(RandActive))]
        [Dropdown(nameof(RandomNames))]
        private string randGenerator;

        [SerializeField]
        [EnableIf(nameof(RandActive))]
        private bool useSeed = false;

        [SerializeField]
        [EnableIf(EConditionOperator.And, nameof(useSeed), nameof(RandActive))]
        private int seed = 0;

        // Names of known scenarios
        [NonSerialized]
        private string[] randomNames;

        // Get PRNG names
        private string[] RandomNames
        {
            get
            {
                // Did we initialize PRNG names already?
                if (randomNames is null)
                {
                    // Obtain known PRNGs
                    randomNames = PRNGHelper.Instance.KnownPRNGs;
                }

                // Return existing PRNG names
                return randomNames;
            }
        }

        private Random random;

        protected Random PRNG => random;

        protected virtual bool RandActive => true;

        public override IEnumerator Generate(Color[] pixels, int xDim, int yDim)
        {
            random = useSeed
                ? PRNGHelper.Instance.CreatePRNG(randGenerator, seed)
                : PRNGHelper.Instance.CreatePRNG(randGenerator);

            return null;
        }
    }
}