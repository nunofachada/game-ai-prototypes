using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    [CreateAssetMenu(fileName = "damages_config", menuName = "Damages Configuration", order = 1)]
    public class DamagesConfig : ScriptableObject
    {
        [SerializeField]
        [ReorderableList]
        private List<AttackDefenseDamage> damages;

        [Button]
        private void Clear()
        {
            if (damages is null)
            {
                damages = new List<AttackDefenseDamage>();
            }
            else
            {
                damages.Clear();
            }
        }

        [Button]
        private void Reset()
        {
            Clear();

            // High attack
            damages.Add(new AttackDefenseDamage(AttackType.High, DefenseType.High, 2f, 1.8f));
            // Med attack
            damages.Add(new AttackDefenseDamage(AttackType.Med, DefenseType.Med, 1.5f, 1.1f));
            // Low attack
            damages.Add(new AttackDefenseDamage(AttackType.Low, DefenseType.Low, 1.2f, 0.6f));
            // MegaTop attack
            damages.Add(new AttackDefenseDamage(AttackType.MegaTop, DefenseType.High, 5f, 6f));
            // SuperMean attack
            damages.Add(new AttackDefenseDamage(AttackType.SuperMean, DefenseType.Med, 5.5f, 7f));
            // HyperUnder attack
            damages.Add(new AttackDefenseDamage(AttackType.HyperUnder, DefenseType.Low, 6f, 8f));

        }

        // public IEnumerator<AttackPattern> GetEnumerator() => patterns.GetEnumerator();

        // IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
