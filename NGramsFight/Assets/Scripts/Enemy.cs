using System.Collections;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Enemy : Agent
    {
        [SerializeField]
        private float defenseDuration = 0.5f;

        private YieldInstruction defenseUp;

        private Damages damages;

        public DefenseType? Defense { get; private set; }

        private void Awake()
        {
            damages = GetComponentInParent<Damages>();
            defenseUp = new WaitForSeconds(defenseDuration);
        }

        public void SendPrediction(AttackType attack)
        {
            if (!Defense.HasValue)
            {
                AttackDefenseDamage attDefDam = damages.GetAttackDefenseDamage(attack);
                StartCoroutine(DefenseUp(attDefDam.ProperDefense));
                Debug.Log($"Activate defense: {attDefDam.ProperDefense}");
            }
        }

        public bool TakeHit(AttackType attack)
        {
            AttackDefenseDamage attDefDam = damages.GetAttackDefenseDamage(attack);
            if (Defense.HasValue && Defense.Value == attDefDam.ProperDefense)
            {
                // Defense was successful, attack failed
                return false;
            }
            else
            {
                // Defense unsuccessful, attack succeeded
                TakeDamage(attDefDam.DamageToEnemyIfSuccess);
                return true;
            }
        }

        private IEnumerator DefenseUp(DefenseType defense)
        {
            Defense = defense;
            yield return defenseUp;
            Defense = null;
        }
    }
}