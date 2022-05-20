using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    public class Damages : MonoBehaviour
    {
        [SerializeField]
        [ReorderableList]
        private List<AttackDefenseDamage> damages;

        private void Awake()
        {
            var set = new HashSet<AttackDefenseDamage>(damages);
            var toRemove = new List<AttackDefenseDamage>(damages.Count);

            int expectedSetSize = 0;

            for (int i = 0; i < damages.Count; i++)
            {
                set.Add(damages[i]);
                expectedSetSize++;
                if (set.Count < expectedSetSize)
                {
                    toRemove.Add(damages[i]);
                    expectedSetSize--;
                }
            }
            foreach (AttackDefenseDamage attDefDam in toRemove)
            {
                int lastIndex = damages.LastIndexOf(attDefDam);
                damages.RemoveAt(lastIndex);
                Debug.LogWarning(
                    $"Ignoring repeated attack '{attDefDam}' from damage configuration.");
            }
        }

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

    }
}