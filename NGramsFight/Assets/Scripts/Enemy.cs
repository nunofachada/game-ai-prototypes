using System.Collections;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Enemy : Agent
    {
        [SerializeField]
        private float defenseDuration = 1f;

        [SerializeField]
        private GameObject shield;

        private YieldInstruction defenseUpWait;

        private Vector3 shieldBasePos;

        public DefenseType? Defense { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            defenseUpWait = new WaitForSeconds(defenseDuration);
            shieldBasePos = shield.transform.localPosition;
            shield.SetActive(false);
        }

        public void SendPrediction(AttackType attack)
        {
            if (!Defense.HasValue)
            {
                AttackDefenseDamage attDefDam = GetAttackDefenseDamage(attack);
                StartCoroutine(DefenseUp(attDefDam.ProperDefense));
            }
        }

        private IEnumerator DefenseUp(DefenseType defense)
        {
            shield.SetActive(true);
            shield.transform.localPosition = shieldBasePos + new Vector3(0, 0.181f * (int)defense , 0);
            Defense = defense;
            yield return defenseUpWait;
            Defense = null;
            shield.SetActive(false);
        }
    }
}