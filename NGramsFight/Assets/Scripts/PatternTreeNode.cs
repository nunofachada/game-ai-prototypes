using System;
using System.Collections.Generic;
using UnityEngine;
using LibGameAI.Util;

namespace AIUnityExample.NGramsFight
{
    public class PatternTreeNode
    {
        private IDictionary<KeyCode, PatternTreeNode> children;

        public AttackType? Attack { get; private set; }

        public bool IsLeaf => Attack != null;

        public void AddPattern(IEnumerator<KeyCode> revPatEnumerator, AttackType attack)
        {
            if (revPatEnumerator.MoveNext())
            {
                KeyCode input = revPatEnumerator.Current;

                PatternTreeNode node;

                if (children is null)
                {
                    children = new Dictionary<KeyCode, PatternTreeNode>();
                }

                if (children.ContainsKey(input))
                {
                    node = children[input];
                }
                else
                {
                    node = new PatternTreeNode();
                    children.Add(input, node);
                }

                node.AddPattern(revPatEnumerator, attack);
            }
            else
            {
                if (Attack.HasValue)
                {
                    throw new InvalidOperationException(
                        $"Repeated pattern leading to '{Attack}' attack");
                }
                Attack = attack;
            }
        }

        public PatternTreeNode Match(IReadOnlyList<KeyCode> inputList)
        {
            PatternTreeNode matchedActionNode = null;
            int n = inputList.Count;

            if (IsLeaf || n == 0)
            {
                matchedActionNode = this;
            }
            else if (children.ContainsKey(inputList[n - 1]))
            {
                var subList = new ReadOnlyListSegment<KeyCode>(inputList, 0, n - 1);
                matchedActionNode = children[inputList[n - 1]].Match(subList);
            }

            return matchedActionNode;
        }
    }
}