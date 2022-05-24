using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Player : Agent
    {
        [SerializeField]
        private float damageByKeyPress = 0.1f;

        [SerializeField]
        private GameObject normalAttack;

        [SerializeField]
        private GameObject specialAttack;

        private Enemy enemy;

        protected override void Awake()
        {
            base.Awake();
            normalAttack.SetActive(false);
            specialAttack.SetActive(false);
            enemy = transform.parent.GetComponentInChildren<Enemy>();
        }

        public void PerformAttack(AttackType attack)
        {
            Debug.Log($"[PLAYER] Performing {attack} attack");
            if (!enemy.TakeHit(attack))
            {
                TakeDamage(GetAttackDefenseDamage(attack).DamageToPlayerIfFail);
                Debug.Log($"[PLAYER] Enemy predicted attack, my health is now {Health}");
            }
        }

        public void TakeKeyPressDamage() => TakeDamage(damageByKeyPress);
    }
}