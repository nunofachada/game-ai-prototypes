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

        private IDictionary<AttackType, AttackDefenseDamage> damagePlaybook;

        private void Awake()
        {
            damagePlaybook = new Dictionary<AttackType, AttackDefenseDamage>();

            foreach (AttackDefenseDamage attDefDam in damages)
            {
                if (damagePlaybook.ContainsKey(attDefDam.Attack))
                {
                    Debug.LogWarning(
                        $"Ignoring repeated attack '{attDefDam}' from damage configuration.");
                }
                else
                {
                    damagePlaybook.Add(attDefDam.Attack, attDefDam);
                }
            }
        }

        public AttackDefenseDamage GetAttackDefenseDamage(AttackType attack)
        {
            if (damagePlaybook.TryGetValue(attack, out AttackDefenseDamage attDefDam))
            {
                return attDefDam;
            }
            return null;
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
            damages.Add(new AttackDefenseDamage(AttackType.High, DefenseType.High, 2f, 1.2f));
            // Med attack
            damages.Add(new AttackDefenseDamage(AttackType.Med, DefenseType.Med, 1.5f, 0.8f));
            // Low attack
            damages.Add(new AttackDefenseDamage(AttackType.Low, DefenseType.Low, 1.2f, 0.6f));
            // MegaTop attack
            damages.Add(new AttackDefenseDamage(AttackType.MegaMean, DefenseType.Med, 3f, 1.5f));
            // SuperMean attack
            damages.Add(new AttackDefenseDamage(AttackType.SuperUnder, DefenseType.Low, 3.5f, 1.8f));
            // HyperUnder attack
            damages.Add(new AttackDefenseDamage(AttackType.HyperTop, DefenseType.High, 4f, 2f));

        }

    }
}