// Copyright (c) 2022 Nuno Fachada
// Distributed under the MIT License (See accompanying file LICENSE or copy
// at http://opensource.org/licenses/MIT)

using System.Collections.Generic;
using LibGameAI.Util;

namespace LibGameAI.NGrams
{
    /// <summary>
    /// Hierarchical N-Gram.
    /// </summary>
    /// <typeparam name="T">The type of the actions.</typeparam>
    public class HierarchNGram<T> : INGram<T>
    {
        // Threshold: minimum frequency for sequence of actions to be considered
        private readonly int threshold;

        // Array of N-Grams
        private readonly NGram<T>[] predictors;

        /// <inheritdoc/>
        public int NValue { get; }

        /// <summary>
        /// Creates a new hierarchical N-Gram.
        /// </summary>
        /// <param name="nValue">The N in N-Gram (window size + 1).</param>
        /// <param name="threshold">
        /// Minimum number of observations for a specific N-Gram to return a
        /// prediction.
        /// </param>
        public HierarchNGram(int nValue, int threshold)
        {
            // Keep the N value
            NValue = nValue;

            // Keep the threshold
            this.threshold = threshold;

            // Instantiate the array of N-Grams
            predictors = new NGram<T>[nValue];

            // Instantiate the individual N-Grams
            for (int i = 0; i < nValue; i++)
                predictors[i] = new NGram<T>(i + 1);
        }

        /// <summary>
        /// Register a sequence of actions.
        /// </summary>
        /// <param name="actions">
        /// The actions list, which should be at least of size 1.
        /// </param>
        public void RegisterSequence(IReadOnlyList<T> actions)
        {
            // Register the sequence of actions in each Ni-Gram
            for (int i = 0; i < NValue; i++)
            {
                // Are there enough actions for the current Ni-Gram?
                if (actions.Count >= i + 1)
                {
                    ReadOnlyListSegment<T> subactions =
                        new ReadOnlyListSegment<T>(
                            actions,
                            actions.Count - i - 1,
                            i + 1);

                    // Register the sequence of actions in the current Ni-Gram
                    predictors[i].RegisterSequence(subactions);
                }
            }
        }

        /// <summary>
        /// Get the most likely action given a sequence of actions.
        /// </summary>
        /// <param name="actions">
        /// The actions list, which should be at least of size 1.
        /// </param>
        /// <returns>
        /// The most likely action for the given a sequence of actions.
        /// </returns>
        public T GetMostLikely(IReadOnlyList<T> actions)
        {
            // Declare variable for best action and set it to its default value
            T bestAction = default;

            // Go through the various Ni-Grams
            for (int i = 0; i < NValue; i++)
            {
                // Are there enough actions for the current Ni-Gram?
                if (actions.Count >= NValue - i - 1)
                {
                    // Get current Ni-Gram
                    NGram<T> p = predictors[NValue - i - 1];

                    // Create a view containing only the actions for the
                    // current Ni-Gram
                    ReadOnlyListSegment<T> subactions =
                        new ReadOnlyListSegment<T>(
                            actions,
                            actions.Count + i + 1 - NValue,
                            NValue - i - 1);

                    // Get frequency of action sequence in the current Ni-Gram
                    int numActions = p.GetActionsFrequency(subactions);

                    // Is that frequency larger than the threshold?
                    if (numActions > threshold)
                    {
                        // Then use this action
                        bestAction = p.GetMostLikely(subactions);
                        break;
                    }
                }
            }

            // Return the best action
            return bestAction;
        }
    }
}
