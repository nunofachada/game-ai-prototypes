/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System.Collections.Generic;

namespace LibGameAI.NaiveBayes
{
    // This class represents an attribute with two or more different values
    public class Attrib
    {
        // Name of the attribute
        public string Name { get; }
        // Possible values of the attribute (read-only)
        public ICollection<string> Values => values;

        // List that contains the actual values of the attribute
        private readonly List<string> values;

        // Constructor, accepts a name and a set of possible values
        public Attrib(string name, IEnumerable<string> values)
        {
            Name = name;
            this.values = new List<string>(values);
        }
    }
}
