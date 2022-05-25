/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

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
