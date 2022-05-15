using System;
using System.Collections.Generic;

namespace AIUnityExample.NGramsFight
{
    public class InputActionNode
    {
        private readonly IDictionary<InputType, InputActionNode> children;

        public AttackType? Attack { get; }

        public bool IsLeaf => Attack != null;

        public InputActionNode(AttackType? attack = null)
        {
            if (!attack.HasValue)
            {
                children = new Dictionary<InputType, InputActionNode>();
                Attack = null;
            }
            else
            {
                Attack = attack;
            }
        }

        public void AddChild(InputType input, InputActionNode node)
        {
            if (IsLeaf)
            {
                throw new InvalidOperationException(
                    "Cannot add children to leaf nodes");
            }
            else
            {
                children.Add(input, node);
            }
        }

        public InputActionNode Match(LinkedList<InputType> inputQueue)
        {
            InputActionNode matchedActionNode = null;

            if (IsLeaf || inputQueue.Count == 0)
            {
                matchedActionNode = this;
            }
            else if (children.ContainsKey(inputQueue.First.Value))
            {
                LinkedListNode<InputType> matchedInput = inputQueue.First;
                inputQueue.RemoveFirst();
                matchedActionNode = children[matchedInput.Value].Match(inputQueue);
                inputQueue.AddFirst(matchedInput);
            }

            return matchedActionNode;

        }
    }
}