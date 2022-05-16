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

        public IEnumerable<AttackPattern> Patterns => patterns;

        [Button]
        private void ClearPatterns()
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
        private void SetPatternsToDefault()
        {
            ClearPatterns();

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
