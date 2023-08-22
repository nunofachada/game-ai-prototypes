// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using UnityEngine;

namespace GameAIPrototypes.NGramsFight
{
    /// <summary>
    /// A script for handling the player agent.
    /// </summary>
    public class Player : Agent
    {
        [Tooltip("Damage the player takes each time it presses a key")]
        [SerializeField]
        private float damageByKeyPress = 0.1f;

        // Reference to the regular shot script
        [SerializeField]
        private Shot normalShot;

        // Reference to the special shot script
        [SerializeField]
        private Shot specialShot;

        /// <summary>
        /// Perform an attack
        /// </summary>
        /// <param name="attack">Attack to perform.</param>
        public void PerformAttack(AttackType attack)
        {
            // Determine shot to take based on the attack to perform
            Shot shotToTake = attack <= AttackType.High ? normalShot : specialShot;

            // Obtain the relation between this attack, the proper defense for
            // it and the associated damages
            AttackDefenseDamage attDefDam = GetAttackDefenseDamage(attack);

            // Take shot / perform attack
            shotToTake.Fire(attDefDam);
        }

        /// <summary>
        /// Apply the configured key press damage to the player.
        /// </summary>
        public void TakeKeyPressDamage() => TakeDamage(damageByKeyPress, false);
    }
}