using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    [CreateAssetMenu(fileName = "patterns_config", menuName = "Patterns Configuration", order = 1)]
    public class PatternsConfig : ScriptableObject, IEnumerable<AttackPattern>
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
            patterns.Add(new AttackPattern("d,w,w,d,d", AttackType.MegaTop));
            // Super attack
            patterns.Add(new AttackPattern("d,a,d,w,w", AttackType.SuperMean));
            // Hyper attack
            patterns.Add(new AttackPattern("d,s,s,w,s", AttackType.HyperUnder));

        }

        public IEnumerator<AttackPattern> GetEnumerator() => patterns.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
