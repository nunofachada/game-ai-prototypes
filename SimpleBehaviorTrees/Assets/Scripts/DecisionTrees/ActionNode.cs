/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;

namespace LibGameAI.DecisionTrees
{
    /// <summary>
    /// An action node in a decision tree.
    /// </summary>
    public class ActionNode : IDecisionTreeNode
    {
        // Delegate to function which will execute the actual game action
        private Action gameAction;

        /// <summary>
        /// Create a new game action node.
        /// </summary>
        /// <param name="gameAction">
        /// Delegate to function which will execute the actual game action.
        /// </param>
        public ActionNode(Action gameAction)
        {
            this.gameAction = gameAction;
        }

        /// <summary>
        /// Execute the game action.
        /// </summary>
        public void Execute()
        {
            gameAction();
        }

        /// <summary>
        /// An action node is always a leaf on the tree, so it will return
        /// itself.
        /// </summary>
        /// <returns>
        /// Returns itself.
        /// </returns>
        public IDecisionTreeNode MakeDecision()
        {
            return this;
        }
    }
}
