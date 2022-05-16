using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    [CreateAssetMenu(fileName = "attack_pattern", menuName = "AttackPatternSet", order = 1)]
    public class AttackPatternSet : ScriptableObject
    {
        [SerializeField]
        [ReorderableList]
        private List<AttackPattern> patterns;

        private PatternTreeNode patternMatchTreeRoot;

        private ISet<KeyCode> knownInputs;

        public ISet<KeyCode> KnownInputs => knownInputs;

        public IEnumerable<AttackPattern> Patterns => patterns;

        public int MinLength { get; private set; }
        public int MaxLength { get; private set; }

        public AttackType? Match(LinkedList<TimedInput> inputQueue)
        {
            return patternMatchTreeRoot.Match(inputQueue)?.Attack;
        }

        private void OnEnable()
        {
            patternMatchTreeRoot = new PatternTreeNode();
            knownInputs = new HashSet<KeyCode>();
            MinLength = int.MaxValue;
            MaxLength = 0;

            foreach (AttackPattern pattern in patterns)
            {
                Debug.Log(pattern + ", size=" + pattern.Size);
                if (pattern.Size < MinLength) MinLength = pattern.Size;
                if (pattern.Size > MaxLength) MaxLength = pattern.Size;
                knownInputs.UnionWith(pattern.Pattern);
                patternMatchTreeRoot.AddPattern(
                    pattern.ReversePatternEnumerator, pattern.Attack);
            }
        }

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
            patterns.Add(new AttackPattern("d,w,w,d,d", AttackType.Mega));
            // Super attack
            patterns.Add(new AttackPattern("d,a,d,w,w", AttackType.Super));
            // Hyper attack
            patterns.Add(new AttackPattern("d,s,s,w,s", AttackType.Hyper));

        }

    }
}
