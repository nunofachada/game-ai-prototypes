using System.Collections;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Enemy : Agent
    {
        [SerializeField]
        private float defenseDuration = 0.5f;

        private YieldInstruction defenseUp;

        public DefenseType? Defense { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            defenseUp = new WaitForSeconds(defenseDuration);
        }

        public void SendPrediction(AttackType attack)
        {
            if (!Defense.HasValue)
            {
                AttackDefenseDamage attDefDam = GetAttackDefenseDamage(attack);
                StartCoroutine(DefenseUp(attDefDam.ProperDefense));
                Debug.Log($"[ENEMY] Activate defense {attDefDam.ProperDefense}");
            }
        }

        public bool TakeHit(AttackType attack)
        {
            AttackDefenseDamage attDefDam = GetAttackDefenseDamage(attack);
            if (Defense.HasValue && Defense.Value == attDefDam.ProperDefense)
            {
                // Defense was successful, attack failed
                Debug.Log($"[ENEMY] Successfully defended {attack} attack with {Defense.Value} defense!");
                return false;
            }
            else
            {
                // Defense unsuccessful, attack succeeded
                TakeDamage(attDefDam.DamageToEnemyIfSuccess);
                Debug.Log($"[ENEMY] Suffered an {attack} attack and my health is now {Health}");
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