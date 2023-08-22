/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System.Collections.Generic;

namespace LibGameAI.NaiveBayes
{
    // Represents counts for the individual attribute values
    // This class is for internal consumption only
    internal class AttribCount
    {
        // Contains the counts for each attribute value
        private readonly IDictionary<string, int> counts;

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
