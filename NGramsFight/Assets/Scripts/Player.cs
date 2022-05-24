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

        protected override void Start()
        {
            base.Start();
        }

        public void PerformAttack(AttackType attack)
        {
            Shot shotToTake = attack <= AttackType.High ? normalShot : specialShot;

            AttackDefenseDamage attDefDam = GetAttackDefenseDamage(attack);

            shotToTake.Fire(attDefDam);
        }

        public void TakeKeyPressDamage() => TakeDamage(damageByKeyPress);
    }
}