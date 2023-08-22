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
    /// An agent in the fighting game. Can be player or enemy.
    /// </summary>
    public abstract class Agent : MonoBehaviour
    {
        // Initial health
        [SerializeField]
        private float initHealth = 20;

        // Relation between attacks, proper defenses and damages caused
        private Damages damages;

        // The agent's sprite
        private SpriteRenderer spriteRenderer;

        // This component animates the agent when hit
        private HitAnimate hitAnimate;

        // An agent with health lower than this value is considered dead
        private const float EPSILON = 0.001f;

        /// <summary>
        /// The agent's current health.
        /// </summary>
        public float Health { get; private set; }

        /// <summary>
        /// Set the agent visibility (write-only property).
        /// </summary>
        public bool Visible
        {
            set
            {
                spriteRenderer.enabled = value;
            }
        }

        // Called when the script instance is being loaded.
        protected virtual void Awake()
        {
            // Obtain references to the required components
            damages = GetComponentInParent<Damages>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            hitAnimate = GetComponent<HitAnimate>();
        }

        // Called on the frame when a script is enabled before any of the Update
        // methods are called the first time
        protected virtual void Start()
        {
            ResetHealth();
        }

        /// <summary>
        /// Set the agent's health to its initial value and notify listeners.
        /// </summary>
        public void ResetHealth()
        {
            Health = initHealth;
            OnHealthChange?.Invoke();
        }

        /// <summary>
        /// The agent takes some damage.
        /// </summary>
        /// <param name="damage">Damage to take.</param>
        /// <param name="animate">
        /// Whether to animate the agent when taking this
        /// <paramref name="damage"/>.
        /// </param>
        public void TakeDamage(float damage, bool animate = true)
        {
            Health -= damage;
            if (Health < EPSILON)
            {
                Health = 0;
                OnDie?.Invoke();
            }
            OnHealthChange?.Invoke();

            if (animate)
            {
                hitAnimate?.Animate();
            }
        }

        // Gets the attack-proper defense-damage relation for the given attack
        protected AttackDefenseDamage GetAttackDefenseDamage(AttackType attack)
            => damages.GetAttackDefenseDamage(attack);

        /// <summary>
        /// Event raised when the agent dies (i.e., its health reaches zero).
        /// </summary>
        public event Action OnDie;

        /// <summary>
        /// Event raised when the agent health changes.
        /// </summary>
        public event Action OnHealthChange;
    }
}