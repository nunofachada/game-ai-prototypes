/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Reflection;
using UnityEngine;
using LibGameAI.Util;
using Random = System.Random;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    public class PRNGHelper
    {
        // Unique instance of this class, instantiated lazily
        private static readonly Lazy<PRNGHelper> instance =
            new Lazy<PRNGHelper>(() => new PRNGHelper());

        // Array of known PRNGs.
        private readonly string[] knownPRNGs;

        // Private singleton constructor.
        private PRNGHelper()
        {
            // Spin up the scenario class manager with custom
            // scenario naming and get the scenario names
            string[] knownPRNGs = ClassManager<Random>
                .Instance
                .FilterTypes(t =>
                    t.FullName.Contains("System") ||
                    t.FullName.Contains("LibGameAI.PRNG"))
                .ReplaceNames(n => n.Remove(0, n.LastIndexOf(".") + 1))
                .ClassNames;

            // Sort them
            Array.Sort(knownPRNGs);

            // Keep reference
            this.knownPRNGs = knownPRNGs;
        }

        /// <summary>
        /// Provides public access to known PRNGs.
        /// </summary>
        public string[] KnownPRNGs => knownPRNGs;

        /// <summary>
        /// Returns the singleton instance of this class.
        /// </summary>
        /// <value>The singleton instance of this class.</value>
        public static PRNGHelper Instance => instance.Value;

        /// <summary>
        /// Empty method so that this singleton can be instantiated on demand.
        /// </summary>
        public void Init() { }

        /// <summary>
        /// Factory method which creates a PRNG instance.
        /// </summary>
        /// <param name="prngName">Name of the PRNG to create.</param>
        /// <param name="seed">Optional seed to initialize the PRNG.</param>
        /// <returns>A new PRNG of the specified type.</returns>
        public Random CreatePRNG(string prngName, int? seed = null)
        {
            Type rndType = ClassManager<Random>
                .Instance
                .GetTypeFromName(prngName);

            Type[] constrParams = seed.HasValue
                ? new Type[] { typeof(int) }
                : new Type[0];

            ConstructorInfo rndConstr = rndType.GetConstructor(constrParams);

            if (rndConstr is null)
            {
                Debug.LogWarning(
                    $"The {rndType} PRNG does not have the required constructor.");
                return null;
            }

            return seed.HasValue
                ? rndConstr.Invoke(new object[] { seed.Value }) as Random
                : rndConstr.Invoke(new object[0]) as Random;
        }
    }
}