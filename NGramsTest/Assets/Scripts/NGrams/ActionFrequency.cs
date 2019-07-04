/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using System.Collections;
using System.Collections.Generic;

namespace NGrams
{
    // Data type for holding the actions and their frequencies
    public class ActionFrequency<T>
    {
        // This dictionary relates actions and their frequencies
        private Dictionary<T, int> actionFrequencies;

        // Number of times the sequence associated to these actions has been
        // executed
        public int Total { get; private set; }

        // Property which represents the most likely/frequent action
        public T BestAction
        {
            get
            {
                // The highest frequency of an action
                int highestFreq = 0;
                // The most likely action
                T likelyAction = default(T);

                // Cycle through action-frequency pairs
                foreach (KeyValuePair<T, int> kvp in actionFrequencies)
                {
                    // If current action is the most frequent so far, keep it
                    if (kvp.Value > highestFreq)
                    {
                        likelyAction = kvp.Key;
                        highestFreq = kvp.Value;
                    }
                }

                // Return the more likely/frequent action
                return likelyAction;
            }
        }

        // Create new action frequency data record
        public ActionFrequency()
        {
            actionFrequencies = new Dictionary<T, int>();
            Total = 0;
        }

        // Increment the frequency of the specified action
        public void IncrementFrequency(T action)
        {
            // If the current action is not in the action-frequency dictionary
            // add it with frequency equal to zero
            if (!actionFrequencies.ContainsKey(action))
                actionFrequencies[action] = 0;

            // Increment frequency associated with this action
            actionFrequencies[action]++;

            // Increment the total frequency for this set of actions
            Total++;
        }
    }
}
