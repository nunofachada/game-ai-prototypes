/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using System.Collections.Generic;

namespace NaiveBayes
{
    // This class represents an attribute with two or more different values
    public class Attrib
    {
        // Name of the attribute
        public string Name { get; }
        // Possible values of the attribute (read-only)
        public ICollection<string> Values => values;

        // List that contains the actual values of the attribute
        private List<string> values;

        // Constructor, accepts a name and a set of possible values
        public Attrib(string name, IEnumerable<string> values)
        {
            Name = name;
            this.values = new List<string>(values);
        }
    }
}