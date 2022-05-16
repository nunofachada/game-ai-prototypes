/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;

namespace LibGameAI.NGrams
{
    public interface INGram<T>
    {
        // The N in N-Gram (window size + 1)
        int NValue { get; }

        // Register a sequence of actions
        void RegisterSequence(IReadOnlyList<T> actions);

        // Get the most likely action given a sequence of actions
        T GetMostLikely(IReadOnlyList<T> actions);
    }
}
