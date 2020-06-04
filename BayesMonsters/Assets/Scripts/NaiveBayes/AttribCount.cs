/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using System.Collections.Generic;

namespace NaiveBayes
{
    // Represents counts for the individual attribute values
    // This class is for internal consumption only
    internal class AttribCount
    {
        // Contains the counts for each attribute value
        private IDictionary<string, int> counts;

        // Constructor, initializes all counts to zero
        public AttribCount(Attrib attribute)
        {
            counts = new Dictionary<string, int>();
            foreach (string value in attribute.Values)
            {
                counts[value] = 0;
            }
        }

        // Get count for a given value
        public int GetCount(string value)
        {
            return counts[value];
        }

        // Increment count for a given value
        public void AddCount(string value, int count)
        {
            counts[value] += count;
        }

    }

}