using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Player : Agent
    {
        [SerializeField]
        private float damageByKeyPress = 0.1f;

        private Damages damages;
        private Enemy enemy;

        private void Awake()
        {
            damages = GetComponentInParent<Damages>();
            enemy = transform.parent.GetComponentInChildren<Enemy>();
        }

        public void PerformAttack(AttackType attack)
        {
            Debug.Log($"[PLAYER] Performing {attack} attack");
            if (!enemy.TakeHit(attack))
            {
                TakeDamage(damages.GetAttackDefenseDamage(attack).DamageToPlayerIfFail);
                Debug.Log($"[PLAYER] Enemy predicted attack, my health is now {Health}");
            }
        }

        public void TakeKeyPressDamage() => TakeDamage(damageByKeyPress);
    }
}