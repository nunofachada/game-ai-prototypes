using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Player : Agent
    {
        [SerializeField]
        private float damageByKeyPress = 0.1f;

        [SerializeField]
        private Shot normalShot;

        [SerializeField]
        private Shot specialShot;

        private Enemy enemy;

        protected override void Awake()
        {
            base.Awake();
            enemy = transform.parent.GetComponentInChildren<Enemy>();
        }

        protected override void Start()
        {
            base.Start();
        }

        public void PerformAttack(AttackType attack)
        {
            Shot shotToTake = attack <= AttackType.High ? normalShot : specialShot;

            AttackDefenseDamage attDefDam = GetAttackDefenseDamage(attack);

            shotToTake.Fire(attDefDam);

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