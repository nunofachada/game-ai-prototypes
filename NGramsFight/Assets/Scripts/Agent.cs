using System;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public abstract class Agent : MonoBehaviour
    {
        [SerializeField]
        private float initHealth = 100;

        public float Health { get; private set; }

        private void Start()
        {
            ResetHealth();
        }

        public void ResetHealth()
        {
            Health = initHealth;
            OnHealthChange?.Invoke();
        }

        protected void TakeDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Health = 0;
                OnDie?.Invoke();
            }
            OnHealthChange?.Invoke();
        }

        public event Action OnDie;

        public event Action OnHealthChange;
    }
}