/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace LibGameAI.DecisionTrees
{
    public class ActionNode : IGameAction, IDecisionTreeNode
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
