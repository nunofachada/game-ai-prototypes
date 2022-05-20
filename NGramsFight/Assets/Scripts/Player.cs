using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIUnityExample.NGramsFight
{
    public class Player : Agent
    {
        private Damages damages;
        private Enemy enemy;

        private void Awake()
        {
            damages = transform.parent.GetComponentInChildren<Damages>();
            enemy = transform.parent.GetComponentInChildren<Enemy>();
        }

        public void PerformAttack(AttackType attack)
        {
            if (!enemy.TakeHit(attack))
            {
                TakeDamage(damages.GetAttackDefenseDamage(attack).DamageToPlayerIfFail);
            }
        }
    }
}