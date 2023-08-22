/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System.Collections.Generic;

namespace LibGameAI.NGrams
{
    /// <summary>
    /// Interface for concrete N-Gram implementations.
    /// </summary>
    /// <typeparam name="T">The type of the actions.</typeparam>
    public interface INGram<T>
    {
        /// <summary>
        /// The N in N-Gram (window size + 1).
        /// </summary>
        int NValue { get; }

        /// <summary>
        /// Register a sequence of actions.
        /// </summary>
        /// <param name="actions">
        /// The actions list.
        /// </param>
        void RegisterSequence(IReadOnlyList<T> actions);

        /// <summary>
        /// Get the most likely action given a sequence of actions.
        /// </summary>
        /// <param name="actions">
        /// The actions list.
        /// </param>
        /// <returns>
        /// The most likely action for the given a sequence of actions.
        /// </returns>
        T GetMostLikely(IReadOnlyList<T> actions);
    }
}
