/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using System.Linq;

namespace LibGameAI.Util
{
    /// <summary>
    /// Singleton class used for finding and keeping a record of concrete
    /// classes of a given generic type.
    /// </summary>
    public class ClassManager<T>
    {
        // Unique instance of this class, instantiated lazily
        private static readonly Lazy<ClassManager<T>> instance =
            new Lazy<ClassManager<T>>(() => new ClassManager<T>());

        // Known classes of type T
        private IDictionary<string, Type> knownTypes;

        /// <summary>
        /// Returns the singleton instance of this class.
        /// </summary>
        /// <value>The singleton instance of this class.</value>
        public static ClassManager<T> Instance => instance.Value;

        /// <summary>
        /// Array of class names.
        /// </summary>
        /// <value>Names of known classes of type T.</value>
        public string[] ClassNames => knownTypes.Keys.ToArray();

        /// <summary>
        /// Get class type from class name.
        /// </summary>
        /// <param name="name">
        /// Name of class.
        /// </param>
        /// <returns>
        /// The class's type.
        /// </returns>
        public Type GetTypeFromName(string name) => knownTypes[name];

        /// <summary>
        /// Get name from type.
        /// </summary>
        /// <param name="type">Type of class.</param>
        /// <returns>Name of class.</returns>
        public string GetNameFromType(Type type)
        {
            foreach (KeyValuePair<string, Type> kvp in knownTypes)
            {
                if (kvp.Value.Equals(type))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Replace default names with which the classes are known.
        /// </summary>
        /// <param name="replacer">Delegate to perform name replacement.</param>
        /// <returns>This instance, for use in fluent syntax.</returns>
        public ClassManager<T> ReplaceNames(Func<string, string> replacer)
        {
            knownTypes = knownTypes
                .ToDictionary(kvp => replacer(kvp.Key), kvp => kvp.Value);
            return this;
        }

        /// <summary>
        /// Filter out types that are not of interest to the client.
        /// </summary>
        /// <param name="filter">
        /// Filter to apply, returns <c>true</c> if class is to keep, or
        /// <c>false</c> otherwise.
        /// </param>
        /// <returns>This instance, for use in fluent syntax.</returns>
        public ClassManager<T> FilterTypes(Func<Type, bool> filter)
        {
            knownTypes = knownTypes
                .Where(kvp => filter(kvp.Value))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return this;
        }

        // Private constructor
        private ClassManager()
        {
            // Get a reference to the class type
            Type classType = typeof(T);

            // Get classes which extend or implement T and are not abstract
            knownTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract &&
                    (t.IsSubclassOf(classType) || classType.IsAssignableFrom(t)))
                .ToDictionary(t => t.FullName, t => t);
        }
    }
}
