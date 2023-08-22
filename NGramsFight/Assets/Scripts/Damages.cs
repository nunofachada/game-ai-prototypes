// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace GameAIPrototypes.NGramsFight
{
    /// <summary>
    /// An list of <see cref="AttackDefenseDamage"/> which can be edited in the
    /// editor.
    /// </summary>
    public class Damages : MonoBehaviour
    {
        // The actual list of attack-proper defense-damages relations
        [SerializeField]
        [ReorderableList]
        private List<AttackDefenseDamage> damages;

        // Relates an attack with its respective attack-proper defense-damages
        // relation
        private IDictionary<AttackType, AttackDefenseDamage> damagePlaybook;

        // Called when the script instance is being loaded
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

        /// <summary>
        /// Get the <see cref="AttackDefenseDamage"/> relation associated with
        /// the given <paramref name="attack"/>.
        /// </summary>
        /// <param name="attack">The attack.</param>
        /// <returns>
        /// The <see cref="AttackDefenseDamage"/> relation associated with the
        /// given <paramref name="attack"/>, or `null` if none found.
        /// </returns>
        public AttackDefenseDamage GetAttackDefenseDamage(AttackType attack)
        {
            if (damagePlaybook.TryGetValue(attack, out AttackDefenseDamage attDefDam))
            {
                return attDefDam;
            }
            return null;
        }

        // Clear list of attack-defense-damages relations
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

        // Initialize the list of attack-defense-damage relations with
        // sane/tested defaults
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