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
    public static class PRNGHelper
    {
        public static string[] KnownPRNGs
        {
            get
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

                // Return them
                return knownPRNGs;
            }
        }

        public static Random PRNGInstance(string prngName, int? seed = null)
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