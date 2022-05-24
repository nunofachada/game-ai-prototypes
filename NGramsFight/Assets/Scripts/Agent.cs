using System;
using System.Collections;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public abstract class Agent : MonoBehaviour
    {
        [SerializeField]
        private float initHealth = 100;

        private Damages damages;

        private SpriteRenderer spriteRenderer;

        private HitAnimate hitAnimate;

        private const float EPSILON = 0.001f;

        public float Health { get; private set; }

        public bool Visible
        {
            set
            {
                spriteRenderer.enabled = value;
            }
        }

        protected virtual void Awake()
        {
            damages = GetComponentInParent<Damages>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            hitAnimate = GetComponent<HitAnimate>();
        }

        protected virtual void Start()
        {
            ResetHealth();
        }

        public void ResetHealth()
        {
            Health = initHealth;
            OnHealthChange?.Invoke();
        }

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

        protected AttackDefenseDamage GetAttackDefenseDamage(AttackType attack)
            => damages.GetAttackDefenseDamage(attack);

        public event Action OnDie;

        public event Action OnHealthChange;
    }
}