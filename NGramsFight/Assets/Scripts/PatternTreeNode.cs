// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System;
using System.Collections.Generic;
using UnityEngine;
using LibGameAI.Util;

namespace AIUnityExample.NGramsFight
{
    /// <summary>
    /// A node of a tree data structure used for matching patterns with specific
    /// attacks.
    /// </summary>
    public class PatternTreeNode
    {
        // Children of this node are nodes associated with specific key codes
        private IDictionary<KeyCode, PatternTreeNode> children;

        /// <summary>
        /// The attack associated with this node. It's `null` unless the node
        /// is a leaf node. Intermediate nodes don't have associated attacks.
        /// </summary>
        public AttackType? Attack { get; private set; }

        /// <summary>
        /// Is this node a leaf in the tree?
        /// </summary>
        public bool IsLeaf => Attack != null;

        /// <summary>
        /// Add a pattern to this node.
        /// </summary>
        /// <param name="revPatEnumerator">
        /// An enumerator which iterates through the remaining pattern in
        /// reverse order.
        /// </param>
        /// <param name="attack">
        /// The attack associated with this pattern.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the pattern is so far a repetition of an already existing
        /// pattern which leads to an attack (making the given pattern
        /// impossible to be detected).
        /// </exception>
        public void AddPattern(IEnumerator<KeyCode> revPatEnumerator, AttackType attack)
        {
            // Does the enumerator have any more elements?
            if (revPatEnumerator.MoveNext())
            {
                // Obtain next key code in pattern
                KeyCode input = revPatEnumerator.Current;

                // Child node to add to children
                PatternTreeNode childNode;

                // Instantiate the children dictionary if it's the first time
                // a pattern passes through the current node
                if (children is null)
                {
                    children = new Dictionary<KeyCode, PatternTreeNode>();
                }

                // Is there a child node associated with the current key code?
                if (children.ContainsKey(input))
                {
                    // If so, fetch it
                    childNode = children[input];
                }
                else
                {
                    // Otherwise create a new child node and associate it with
                    // the current key code
                    childNode = new PatternTreeNode();
                    children.Add(input, childNode);
                }

                // Add the remaining pattern and respective attack to the child
                // node
                childNode.AddPattern(revPatEnumerator, attack);
            }
            else
            {
                // If the pattern doesn't have any more key codes, it means this
                // is a leaf node, to which an attack must be associated
                if (Attack.HasValue)
                {
                    // However, if an attack has been previously associated with
                    // this node, it means the current pattern is repeated, in
                    // which case we'll thrown an exception
                    throw new InvalidOperationException(
                        $"Repeated pattern leading to '{Attack}' attack");
                }

                // Associate attack with this leaf node
                Attack = attack;
            }
        }

        /// <summary>
        /// Match a given input with a registered pattern.
        /// </summary>
        /// <param name="inputList">A sequence of key codes.</param>
        /// <returns>
        /// The leaf node associated with the given sequence of key codes, or
        /// `null` if no match is found.
        /// </returns>
        public PatternTreeNode Match(IReadOnlyList<KeyCode> inputList)
        {
            // The leaf node associated with the given pattern; assume it's
            // null, which will be the case if no match is found
            PatternTreeNode matchedActionNode = null;

            // The number of remaining key codes to match
            int n = inputList.Count;

            if (IsLeaf || n == 0)
            {
                // If this node is a leaf or there aren't any more key codes to
                // match, return it has the match found
                matchedActionNode = this;
            }
            else if (children.ContainsKey(inputList[n - 1]))
            {
                // Otherwise, if a child node is associated with the next key
                // code, keep searching recursively for a match
                var subList = new ReadOnlyListSegment<KeyCode>(inputList, 0, n - 1);
                matchedActionNode = children[inputList[n - 1]].Match(subList);
            }

            return matchedActionNode;
        }
    }
}