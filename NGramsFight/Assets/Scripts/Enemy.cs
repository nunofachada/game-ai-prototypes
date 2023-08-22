// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System.Collections;
using UnityEngine;

namespace GameAIPrototypes.NGramsFight
{
    /// <summary>
    /// A script for handling the enemy agent.
    /// </summary>
    public class Enemy : Agent
    {
        [Tooltip("How long should the enemy keep the shield raised after predicting an attack?")]
        [SerializeField]
        private float defenseDuration = 1f;

        // Reference to the shield game object
        [SerializeField]
        private GameObject shield;

        // Used in the co-routine which keeps the shield raised
        private YieldInstruction defenseUpWait;

        // The base position of the shield
        private Vector3 shieldBasePos;

        // The current raised defense, if any
        private DefenseType? defense;

        // Called when the script instance is being loaded
        protected override void Awake()
        {
            base.Awake();
            defenseUpWait = new WaitForSeconds(defenseDuration);
            shieldBasePos = shield.transform.localPosition;
            shield.SetActive(false);
        }

        /// <summary>
        /// Informs the enemy of the prediction for the next attack.
        /// </summary>
        /// <param name="attack">The predicted attack.</param>
        public void ReceivePrediction(AttackType attack)
        {
            // Mount a defense, but only if no defense is currently mounted
            if (!defense.HasValue)
            {
                // Determine proper defense
                AttackDefenseDamage attDefDam = GetAttackDefenseDamage(attack);

                // Initiate proper defense
                StartCoroutine(DefenseUp(attDefDam.ProperDefense));
            }
        }

        // Co-routine which keeps a defense raised during a predefined duration
        private IEnumerator DefenseUp(DefenseType defense)
        {
            // Activate shield and put it into place
            shield.SetActive(true);
            shield.transform.localPosition = shieldBasePos + new Vector3(0, 0.181f * (int)defense , 0);

            // Keep a note of the defense currently raised
            this.defense = defense;

            // Wait for a predefined duration before unmounting defense
            yield return defenseUpWait;

            // Deactivate shield / unmount defense
            this.defense = null;
            shield.SetActive(false);
        }
    }
}