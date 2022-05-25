// Copyright (c) 2022 Nuno Fachada
// Distributed under the MIT License (See accompanying file LICENSE or copy
// at http://opensource.org/licenses/MIT)

using System.Collections.Generic;
using System.Text;
using LibGameAI.Util;

namespace LibGameAI.NGrams
{
    /// <summary>
    /// A regular N-Gram.
    /// </summary>
    /// <typeparam name="T">The type of the actions.</typeparam>
    public class NGram<T> : INGram<T>
    {
        /// <inheritdoc/>
        public int NValue { get; }

        // Dictionary which relates a sequence to a set of actions and
        // probabilities
        private readonly IDictionary<string, ActionFrequency<T>> data;

        // String builder object, for converting sequences of objects to strings
        private static StringBuilder stringBuilder;

        // Initialize static string builder
        static NGram()
        {
            stringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Creates a new N-Gram.
        /// </summary>
        /// <param name="nValue">The N in N-Gram (window size + 1).</param>
        public NGram(int nValue)
        {
            NValue = nValue;
            data = new Dictionary<string, ActionFrequency<T>>();
        }

        /// <summary>
        /// Converts a sequence of objects to a string.
        /// </summary>
        /// <param name="actions">Sequence of objects.</param>
        /// <returns>String representing sequence of objects.</returns>
        public static string SequenceToString(IEnumerable<T> actions)
        {
            stringBuilder.Clear();
            foreach (T a in actions)
            {
                stringBuilder.Append(a.ToString());
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Register a sequence of actions.
        /// </summary>
        /// <param name="actions">
        /// The actions list, which should be at least of size N.
        /// </param>
        public void RegisterSequence(IReadOnlyList<T> actions)
        {
            // Can only register sequence if its size N
            if (actions.Count == NValue)
            {
                // Previous actions
                ReadOnlyListSegment<T> prevActions =
                    new ReadOnlyListSegment<T>(actions, 0, NValue - 1);

                // Previous actions in key form
                string prevActionsKey = SequenceToString(prevActions);

                // Action performed
                T actionPerformed = actions[NValue - 1];

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

        /// <summary>
        /// Get the most likely action given a sequence of actions.
        /// </summary>
        /// <param name="actions">
        /// The actions list, which should be at least of size N-1.
        /// </param>
        /// <returns>
        /// The most likely action for the given a sequence of actions.
        /// </returns>
        public T GetMostLikely(IReadOnlyList<T> actions)
        {
            // The most likely action, initially set to its default value
            T bestAction = default;

            // The actions array must have the window size
            if (actions.Count == NValue - 1)
            {
                // First, convert sequence of actions to string (i.e. the key)
                string key = SequenceToString(actions);

                // Try to get the best action for the given sequence of actions
                if (data.TryGetValue(key, out ActionFrequency<T> actionFrequency))
                {
                    bestAction = actionFrequency.BestAction;
                }
            }

            // Return the most likely/most frequent action
            return bestAction;
        }

        /// <summary>
        /// Return the number of times the given sequence of actions has been
        /// seen.
        /// </summary>
        /// <param name="actions">
        /// Sequence of actions, which must of size N-1.
        /// </param>
        /// <returns>
        /// Number of times the given sequence of actions has been seen.
        /// </returns>
        public int GetActionsFrequency(IReadOnlyCollection<T> actions)
        {
            // Number of times this sequence of actions has been seen
            int actionCount = 0;

            if (actions.Count == NValue - 1)
            {
                // First, convert sequence of actions to string (i.e. the key)
                string key = SequenceToString(actions);

                // If there is data for this sequence, get the number of times
                // this sequence has been seen
                if (data.ContainsKey(key)) actionCount = data[key].Total;
            }
            return actionCount;
        }
    }
}
