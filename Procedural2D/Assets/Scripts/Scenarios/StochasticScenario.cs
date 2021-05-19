/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Reflection;
using UnityEngine;
using NaughtyAttributes;
using LibGameAI.Util;
using Random = System.Random;

namespace AIUnityExamples.Procedural2D.Scenarios
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
                    randomNames = PRNGHelper.KnownPRNGs;
                }

                // Return existing PRNG names
                return randomNames;
            }
        }

        private Random random;

        protected Random PRNG => random;

        protected virtual bool RandActive => true;

        public override void Generate(Color[] pixels, int width, int height)
        {
            random = useSeed
                ? PRNGHelper.PRNGInstance(randGenerator, seed)
                : PRNGHelper.PRNGInstance(randGenerator);
        }
    }
}