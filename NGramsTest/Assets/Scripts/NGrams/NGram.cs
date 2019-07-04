/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

namespace NGrams
{
    public class NGram<T> : INGram<T>
    {
        // The N in N-Gram (window size + 1)
        public int NValue { get; }

        // Dictionary which relates a sequence to a set of actions and
        // probabilities
        private Dictionary<string, ActionFrequency<T>> data;

        // Constructor, accepts the N in N-Gram
        public NGram(int nValue)
        {
            NValue = nValue;
            data = new Dictionary<string, ActionFrequency<T>>();
        }

        // Converts a sequence of actions to a string
        public static string ArrayToStringKey(T[] actions)
        {
            StringBuilder builder = new StringBuilder();
            foreach (T a in actions)
            {
                builder.Append(a.ToString());
            }
            return builder.ToString();
        }

        // Register a sequence of actions
        // The actions array should be of size N
        public void RegisterSequence(T[] actions)
        {

            // Can only register sequence if its size N
            if (actions.Length == NValue)
            {

                // Previous actions
                T[] prevActions = new T[NValue - 1];

                // Previous actions in key form
                string prevActionsKey;

                // Action performed
                T actionPerformed;

                // Split the sequence into a key and value
                Array.ConstrainedCopy(actions, 0, prevActions, 0, NValue - 1);
                actionPerformed = actions[NValue - 1];
                prevActionsKey = ArrayToStringKey(prevActions);

                // Check if our data contains the key (i.e. sequence of actions)
                // If not, create a new record for this sequence of actions
                if (!data.ContainsKey(prevActionsKey))
                    data[prevActionsKey] = new ActionFrequency<T>();

                // Get the data record for this sequence of actions
                ActionFrequency<T> actionFrequency = data[prevActionsKey];

                // Increment the number of times this action was performed
                // after the given sequence of actions
                actionFrequency.IncrementFrequency(actionPerformed);
            }
        }

        // Get the most likely action given a sequence of actions
        // The actions array should be of size N-1
        public T GetMostLikely(T[] actions)
        {
            // The most likely action, initially set to its default value
            T bestAction = default(T);

            // The actions array must have the window size
            if (actions.Length == NValue - 1)
            {
                // First, convert sequence of actions to string (i.e. the key)
                string key = ArrayToStringKey(actions);

                // The data record for this sequence of actions
                ActionFrequency<T> actionFrequency;

                // Try to get the best action for the given sequence of actions
                if (data.TryGetValue(key, out actionFrequency))
                {
                    bestAction = actionFrequency.BestAction;
                }
            }

            // Return the most likely/most frequent action
            return bestAction;
        }

        // Return the number of times this sequence has been seen
        // The actions array should be of size N-1
        public int GetActionsFrequency(T[] actions)
        {
            // Number of times this sequence of actions has been seen
            int actionCount = 0;

            if (actions.Length == NValue - 1)
            {
                // First, convert sequence of actions to string (i.e. the key)
                string key = ArrayToStringKey(actions);

                // If there is data for this sequence, get the number of times
                // this seqence has been seen
                if (data.ContainsKey(key)) actionCount = data[key].Total;
            }
            return actionCount;
        }
    }
}
