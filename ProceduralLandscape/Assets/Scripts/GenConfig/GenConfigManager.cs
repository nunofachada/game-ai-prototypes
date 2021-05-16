/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using System.Linq;

namespace AIUnityExamples.ProceduralLandscape.GenConfig
{
    /// <summary>
    /// Singleton class used for finding and keeping a record of existing
    /// generator configurators.
    /// </summary>
    public class GenConfigManager
    {
        // Unique instance of this class, instantiated lazily
        private static readonly Lazy<GenConfigManager> instance =
            new Lazy<GenConfigManager>(() => new GenConfigManager());

        // Known generation methods
        private readonly IDictionary<string, Type> genCfgTable;

        /// <summary>
        /// Returns the singleton instance of this class.
        /// </summary>
        /// <value>The singleton instance of this class.</value>
        public static GenConfigManager Instance => instance.Value;

        /// <summary>
        /// Array of generator names.
        /// </summary>
        /// <value>Names of known generators.</value>
        public string[] GeneratorNames => genCfgTable.Keys.ToArray();

        /// <summary>
        /// Get generator configurator type from simplified name.
        /// </summary>
        /// <param name="name">
        /// Simple name of generator configurator class.
        /// </param>
        /// <returns>
        /// The generator configurator's type.
        /// </returns>
        public Type GetTypeFromName(string name) => genCfgTable[name];

        /// <summary>
        /// Get simplified name from type.
        /// </summary>
        /// <param name="type">Type of generator configurator.</param>
        /// <returns>Simplified name of generator configurator.</returns>
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
        private GenConfigManager()
        {
            // Get a reference to the generator configurator type
            Type typeGenConfig = typeof(AbstractGenConfig);

            // Get known methods, i.e. classes which extends AbstractGenConfig,
            // and are not abstract
            genCfgTable = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeGenConfig) && !t.IsAbstract)
                .ToDictionary(t => SimpleName(t.FullName), t => t);
        }

        /// <summary>
        /// Simplify the name of a generator by removing the namespace
        /// and the "Config" substring in the end.
        /// </summary>
        /// <param name="fqName">
        /// The fully qualified name of the generator.
        /// </param>
        /// <returns>
        /// The simplified name of the generator.
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
            if (simpleName.EndsWith("Config"))
            {
                simpleName = simpleName.Substring(
                    0, simpleName.Length - "Config".Length);
            }

            // Return simple name
            return simpleName;
        }
    }
}