using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIUnityExample.NGramsFight
{
    public class InputActionNode
    {
        private IDictionary<KnownInput, InputActionNode> children;

        private Action FightAction;


        public bool IsLeaf => (children?.Count ?? 0) == 0 && FightAction is null;

        public InputActionNode(Action fightAction = null)
        {
            if (fightAction is null)
            {
                children = new Dictionary<KnownInput, InputActionNode>();
            }
        }

        public void AddChild(KnownInput input, InputActionNode node)
        {
            children.Add(input, node);
        }

        public InputActionNode Match(KnownInput input)
        {

        }
    }
}