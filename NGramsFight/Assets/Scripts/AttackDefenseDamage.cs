// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System;
using UnityEngine;

namespace GameAIPrototypes.NGramsFight
{
    /// <summary>
    /// Relates an <see cref="AttackType"/> with a proper
    /// <see cref="DefenseType"/> and the associated damage in case the attack
    /// is successfull or not.
    /// </summary>
    [Serializable]
    public class AttackDefenseDamage
    {
        // The attack
        [SerializeField]
        private AttackType attack;

        // The proper defense for the attack
        [SerializeField]
        private DefenseType properDefense;

        // Damage to enemy if attack is successful
        [SerializeField]
        private float damageToEnemyIfSuccess;

        // Damage to player if attack fails
        [SerializeField]
        private float damageToPlayerIfFail;

        /// <summary>
        /// The attack (read-only).
        /// </summary>
        public AttackType Attack => attack;

        /// <summary>
        /// The proper defense for the given <see cref="Attack"/> (read-only).
        /// </summary>
        public DefenseType ProperDefense => properDefense;

        /// <summary>
        /// Damage to enemy if the attack is successful (read-only).
        /// </summary>
        public float DamageToEnemyIfSuccess => damageToEnemyIfSuccess;

        /// <summary>
        /// Damage to player if the attack fails (read-only).
        /// </summary>
        public float DamageToPlayerIfFail => damageToPlayerIfFail;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="attack">The attack.</param>
        /// <param name="properDefense">
        /// The proper defense for the given <paramref name="attack"/>.
        /// </param>
        /// <param name="damageToEnemyIfSuccess">
        /// Damage to enemy if the attack is successful.
        /// </param>
        /// <param name="damageToPlayerIfFail">
        /// Damage to player if the attack fails.
        /// </param>
        public AttackDefenseDamage(
            AttackType attack, DefenseType properDefense,
            float damageToEnemyIfSuccess, float damageToPlayerIfFail)
        {
            this.attack = attack;
            this.properDefense = properDefense;
            this.damageToEnemyIfSuccess = damageToEnemyIfSuccess;
            this.damageToPlayerIfFail = damageToPlayerIfFail;
        }
    }
}