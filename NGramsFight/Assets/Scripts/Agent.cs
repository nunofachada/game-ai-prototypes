using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Agent : MonoBehaviour
    {
        [SerializeField]
        private float initHealth = 100;

        public float Health { get; private set; }

        private void OnEnable()
        {
            Health = initHealth;
        }

        private bool TakeDamage(float damage)
        {
            Health -= damage;
            if (Health < 0)
            {
                Health = 0;
                return true;
            }
            return false;
        }
    }
}