using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    [Serializable]
    public class AttackPattern
    {
        [SerializeField]
        private string patternStr;

        [SerializeField]
        private AttackType attack;

        [SerializeField]
        [HideInInspector]
        private List<KeyCode> pattern;

        public AttackType Attack => attack;

        public int Size => pattern.Count;

        public IEnumerable<KeyCode> Pattern => pattern;

        public AttackPattern(string patternStr, AttackType attack)
        {
            this.patternStr = patternStr;
            this.attack = attack;

            pattern = new List<KeyCode>();

            foreach (string patStr in patternStr.Split(','))
            {
                if (!string.IsNullOrEmpty(patStr))
                {
                    pattern.Add(Event.KeyboardEvent(patStr.Trim()).keyCode);
                }
            }
        }

        public IEnumerator<KeyCode> GetReverseEnumerator()
        {
            for (int i = pattern.Count - 1; i >= 0; i--)
            {
                yield return pattern[i];
            }
        }

        public override string ToString()
        {
            return patternStr;
        }
    }
}