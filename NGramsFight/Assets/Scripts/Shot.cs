// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System;
using System.Collections;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    /// <summary>
    /// Script to be attached to the shot or bullet fired by the player when
    /// performing an attack.
    /// </summary>
    public class Shot : MonoBehaviour
    {
        // The speed at which the shot moves
        [SerializeField]
        private float speed = 0.2f;

        // Reference to the shot's sprite renderer
        private SpriteRenderer spriteRenderer;

        // Original position of the shot game object
        private Vector3 originalPosition;

        // Co-routine which will displace / move the shot, moving from the
        // player towards the enemy
        private Coroutine displace;

        // Reference to the agent that fired the shot (i.e., the player)
        private Agent shooter;

        // A reference to the attack-proper defense-damages relation associated
        // with the last attack that took place
        private AttackDefenseDamage attDefDam;

        // Called when the script instance is being loaded
        private void Awake()
        {
            shooter = GetComponentInParent<Agent>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;

            // A rigid body is required for triggers to trigger
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        /// <summary>
        /// Fire the shot!
        /// </summary>
        /// <param name="attDefDam">
        /// The attack-proper defense-damages relation object associated
        /// with the attack taking place.
        /// </param>
        public void Fire(AttackDefenseDamage attDefDam)
        {
            // Keep reference to the attack-proper defense-damages relation object
            this.attDefDam = attDefDam;

            // Keep original position of the shot game object
            originalPosition = transform.localPosition;

            // Determine vertical position of the shot, depending on the proper
            // defense for this attack
            float y = attDefDam.ProperDefense switch
            {
                DefenseType.High => 0.172f,
                DefenseType.Med => -0.010f,
                DefenseType.Low => -0.191f,
                _ => 0
            };

            // Move shot to the correct vertical position
            transform.localPosition += new Vector3(0, y, 0);

            // Start co-routine that moves the bullet
            displace = StartCoroutine(Displace());
        }

        // Co-routine that moves the bullet
        private IEnumerator Displace()
        {
            // Show the bullet
            spriteRenderer.enabled = true;

            // Move the shot frame by frame at the specified speed
            while (true)
            {
                transform.localPosition += new Vector3(speed * Time.deltaTime, 0, 0);
                yield return null;
            }
        }

        // Method invoked when another object enters a trigger collider attached
        // to this object (in this case, when the shot hits the enemy or it's
        // shield)
        private void OnTriggerEnter2D(Collider2D collider)
        {
            // Stop bullet-moving co-routine, hide bullet and move it to its
            // original position
            StopCoroutine(displace);
            spriteRenderer.enabled = false;
            transform.localPosition = originalPosition;

            // What did the bullet collide with?
            if (collider.gameObject.name.Equals("Shield"))
            {
                // If the bullet collided with the shield, then it's the player
                // that'll take some damage
                Debug.Log($"{attDefDam.Attack} attack unsuccessful, enemy predicted proper {attDefDam.ProperDefense} defense!");
                shooter.TakeDamage(attDefDam.DamageToPlayerIfFail);
            }
            else if (collider.gameObject.name.Equals("Enemy"))
            {
                // If the bullet collided with the enemy, then the enemy takes
                // some damage
                Debug.Log($"{attDefDam.Attack} attack SUCCESSFUL!");
                collider.gameObject
                    .GetComponent<Agent>()
                    .TakeDamage(attDefDam.DamageToEnemyIfSuccess);
            }
            else
            {
                // This should never happen
                throw new InvalidProgramException("Shot hit something unexpected!");
            }
        }
    }
}