/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

 using System;

namespace LibGameAI.DecisionTrees
{
    /// <summary>
    /// A decision node in a decision tree.
    /// </summary>
    public class DecisionNode : IDecisionTreeNode
    {
        // True and false nodes
        private IDecisionTreeNode trueNode, falseNode;

        // Delegate of function which will make the true or false decision
        private Func<bool> decisionFunc;

        /// <summary>
        /// Creates a new decision node.
        /// </summary>
        /// <param name="test">
        /// Function which will make the true or false decision.
        /// </param>
        /// <param name="trueNode">
        /// DT node to return when the decision is true.
        /// </param>
        /// <param name="falseNode">
        /// DT node to return when the decision is false.
        /// </param>
        public DecisionNode(
            Func<bool> test,
            IDecisionTreeNode trueNode, IDecisionTreeNode falseNode)
        {
            this.decisionFunc = test;
            this.trueNode = trueNode;
            this.falseNode = falseNode;
        }

        /// <summary>
        /// Make a decision.
        /// </summary>
        /// <returns>
        /// A DT node, which will depend wether the decision was true or false.
        /// </returns>
        public IDecisionTreeNode MakeDecision()
        {
            // Determine the branch to take depending on the decision function
            IDecisionTreeNode branch = decisionFunc() ? trueNode : falseNode;

            // Recursively return the branch returned by the selected branch
            return branch.MakeDecision();
        }
    }
}
