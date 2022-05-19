using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    public class Patterns : MonoBehaviour
    {
        [SerializeField]
        [Expandable]
        private PatternsConfig patternsConfig;

        private InputFrontend inputFrontend;

        private PatternTreeNode patternMatchTreeRoot;

        private ISet<KeyCode> knownInputs;

        public int MinLength { get; private set; }
        public int MaxLength { get; private set; }

        public AttackType? Match(IReadOnlyList<KeyCode> inputList)
        {
            return patternMatchTreeRoot.Match(inputList)?.Attack;
        }

        private void Awake()
        {
            patternMatchTreeRoot = new PatternTreeNode();
            knownInputs = new HashSet<KeyCode>();
            MinLength = int.MaxValue;
            MaxLength = 0;

            foreach (AttackPattern pattern in patternsConfig)
            {
                //Debug.Log(pattern + ", size=" + pattern.Size);
                if (pattern.Size < MinLength) MinLength = pattern.Size;
                if (pattern.Size > MaxLength) MaxLength = pattern.Size;
                knownInputs.UnionWith(pattern.Pattern);
                patternMatchTreeRoot.AddPattern(
                    pattern.GetReverseEnumerator(), pattern.Attack);
            }
        }

        private void Start()
        {
            inputFrontend = GetComponentInParent<InputFrontend>();
            inputFrontend.SetKnownInputs(knownInputs);
        }
    }
}