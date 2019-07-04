/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
namespace NGrams
{
    public interface INGram<T>
    {
        // The N in N-Gram (window size + 1)
        int NValue { get; }

        // Register a sequence of actions
        void RegisterSequence(T[] actions);

        // Get the most likely action given a sequence of actions
        T GetMostLikely(T[] actions);
    }
}
