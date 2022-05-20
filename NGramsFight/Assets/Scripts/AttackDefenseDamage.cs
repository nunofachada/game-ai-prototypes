using System;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    [Serializable]
    public class AttackDefenseDamage
    {
        [SerializeField]
        private AttackType attack;
        [SerializeField]
        private DefenseType properDefense;
        [SerializeField]
        private float damageToEnemyIfSuccess;
        [SerializeField]
        private float damageToPlayerIfFail;

        public AttackType Attack => attack;
        public DefenseType ProperDefense => properDefense;
        public float DamageToEnemyIfSuccess => damageToEnemyIfSuccess;
        public float DamageToPlayerIfFail => damageToPlayerIfFail;


        public AttackDefenseDamage(
            AttackType attack, DefenseType properDefense,
            float damageToEnemyIfSuccess, float damageToPlayerIfFail)
        {
            this.attack = attack;
            this.properDefense = properDefense;
            this.damageToEnemyIfSuccess = damageToEnemyIfSuccess;
            this.damageToPlayerIfFail = damageToPlayerIfFail;
        }

        public override string ToString() => attack.ToString();
    }
}