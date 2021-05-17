/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using System.Linq;

namespace AIUnityExamples.Procedural2D.Scenarios
{
    /// <summary>
    /// Singleton class used for finding and keeping a record of existing
    /// scenarios.
    /// </summary>
    public class ScenarioManager
    {
        // Unique instance of this class, instantiated lazily
        private static readonly Lazy<ScenarioManager> instance =
            new Lazy<ScenarioManager>(() => new ScenarioManager());

        // Known scenarios
        private readonly IDictionary<string, Type> genCfgTable;

        /// <summary>
        /// Returns the singleton instance of this class.
        /// </summary>
        /// <value>The singleton instance of this class.</value>
        public static ScenarioManager Instance => instance.Value;

        /// <summary>
        /// Array of scenario names.
        /// </summary>
        /// <value>Names of known scenarios.</value>
        public string[] ScenarioNames => genCfgTable.Keys.ToArray();

        /// <summary>
        /// Get scenario type from simplified name.
        /// </summary>
        /// <param name="name">
        /// Simple name of scenario class.
        /// </param>
        /// <returns>
        /// The scenario's type.
        /// </returns>
        public Type GetTypeFromName(string name) => genCfgTable[name];

        /// <summary>
        /// Get simplified name from type.
        /// </summary>
        /// <param name="type">Type of scenario.</param>
        /// <returns>Simplified name of scenario.</returns>
        public string GetNameFromType(Type type)
        {
            foreach (KeyValuePair<string, Type> kvp in genCfgTable)
            {
                if (kvp.Value.Equals(type))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        // Private constructor
        private ScenarioManager()
        {
            // Get a reference to the scenario type
            Type typeGenConfig = typeof(AbstractScenario);

            // Get known methods, i.e. classes which extends AbstractScenario,
            // and are not abstract
            genCfgTable = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeGenConfig) && !t.IsAbstract)
                .ToDictionary(t => SimpleName(t.FullName), t => t);
        }

        /// <summary>
        /// Simplify the name of a scenario by removing the namespace
        /// and the "Scenario" substring in the end.
        /// </summary>
        /// <param name="fqName">
        /// The fully qualified name of the scenario.
        /// </param>
        /// <returns>
        /// The simplified name of the scenario.
        /// </returns>
        public static string SimpleName(string fqName)
        {
            string simpleName = fqName;

            // Strip namespace
            if (simpleName.Contains("."))
            {
                simpleName = fqName.Substring(fqName.LastIndexOf(".") + 1);
            }

            // Strip "Config"
            if (simpleName.EndsWith("Scenario"))
            {
                simpleName = simpleName.Substring(
                    0, simpleName.Length - "Scenario".Length);
            }

            // Return simple name
            return simpleName;
        }
    }
}
