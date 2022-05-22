using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    public class Patterns : MonoBehaviour
    {
        [SerializeField]
        [ReorderableList]
        private List<AttackPattern> patterns;

        [Button]
        private void Clear()
        {
            if (patterns is null)
            {
                patterns = new List<AttackPattern>();
            }
            else
            {
                patterns.Clear();
            }
        }

        [Button]
        private void Reset()
        {
            Clear();

            // Low attack
            patterns.Add(new AttackPattern("d,s,s,s", AttackType.Low));
            // Med attack
            patterns.Add(new AttackPattern("d,a,d,d", AttackType.Med));
            // High attack
            patterns.Add(new AttackPattern("d,w,w,w", AttackType.High));
            // Mega attack
            patterns.Add(new AttackPattern("d,w,w,d,d", AttackType.MegaMean));
            // Super attack
            patterns.Add(new AttackPattern("d,a,d,w,w", AttackType.SuperUnder));
            // Hyper attack
            patterns.Add(new AttackPattern("d,s,s,w,s", AttackType.HyperTop));

        }

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

            foreach (AttackPattern pattern in patterns)
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