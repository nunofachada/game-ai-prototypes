/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

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
