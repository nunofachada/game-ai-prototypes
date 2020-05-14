/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

namespace LibGameAI.DecisionTrees
{
    /// <summary>
    /// Represents a node in a decision tree.
    /// </summary>
    public interface IDecisionTreeNode
    {
        /// <summary>
        /// Make a decision.
        /// </summary>
        /// <returns>
        /// A DT node, which will depend wether the decision was true or false.
        /// </returns>
        IDecisionTreeNode MakeDecision();
    }
}
