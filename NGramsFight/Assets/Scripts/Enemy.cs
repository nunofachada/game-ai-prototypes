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
            damages = transform.parent.GetComponentInChildren<Damages>();
            defenseUp = new WaitForSeconds(defenseDuration);
        }

        public void ReceivePrediction(AttackType attack)
        {
            if (!Defense.HasValue)
            {
                AttackDefenseDamage attDefDam = damages.GetAttackDefenseDamage(attack);
                StartCoroutine(DefenseUp(attDefDam.ProperDefense));
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