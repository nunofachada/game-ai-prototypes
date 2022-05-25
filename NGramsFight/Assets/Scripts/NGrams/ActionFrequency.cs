// Copyright (c) 2022 Nuno Fachada
// Distributed under the MIT License (See accompanying file LICENSE or copy
// at http://opensource.org/licenses/MIT)

using System.Collections.Generic;

namespace LibGameAI.NGrams
{
    /// <summary>
    /// Record containing actions and their frequencies.
    /// </summary>
    /// <typeparam name="T">The type of the actions.</typeparam>
    public class ActionFrequency<T>
    {
        // This dictionary relates actions and their frequencies
        private readonly IDictionary<T, int> actionFrequencies;

        /// <summary>
        /// Number of times the sequence associated to these actions has been
        /// executed.
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// The most likely/frequent action.
        /// </summary>
        public T BestAction
        {
            get
            {
                // The highest frequency of an action
                int highestFreq = 0;

                // The most likely action
                T likelyAction = default;

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

        /// <summary>
        /// Creates new action frequency data record.
        /// </summary>
        public ActionFrequency()
        {
            actionFrequencies = new Dictionary<T, int>();
            Total = 0;
        }

        /// <summary>
        /// Increment the frequency of the specified action.
        /// </summary>
        /// <param name="action">
        /// The action for which the frequency will be incremented.
        /// </param>
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
