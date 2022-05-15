using System;
using System.Collections.Generic;

namespace AIUnityExample.NGramsFight
{
    public class InputActionNode
    {
        private readonly IDictionary<KnownInput, InputActionNode> children;

        public Action FightAction { get; }

        public bool IsLeaf => FightAction != null;

        public InputActionNode(Action fightAction = null)
        {
            if (fightAction is null)
            {
                children = new Dictionary<KnownInput, InputActionNode>();
                FightAction = null;
            }
            else
            {
                FightAction = fightAction;
            }
        }

        public void AddChild(KnownInput input, InputActionNode node)
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

        public InputActionNode Match(LinkedList<KnownInput> inputQueue)
        {
            InputActionNode matchedActionNode = null;

            if (IsLeaf || inputQueue.Count == 0)
            {
                matchedActionNode = this;
            }
            else if (children.ContainsKey(inputQueue.First.Value))
            {
                LinkedListNode<KnownInput> matchedInput = inputQueue.First;
                inputQueue.RemoveFirst();
                matchedActionNode = children[matchedInput.Value].Match(inputQueue);
                inputQueue.AddFirst(matchedInput);
            }

            return matchedActionNode;

        }
    }
}