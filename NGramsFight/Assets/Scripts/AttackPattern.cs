using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    [Serializable]
    public class AttackPattern
    {
        private class RevPatEnumer : IEnumerator<KeyCode>
        {
            private int index;
            private readonly IReadOnlyList<KeyCode> pattern;

            public KeyCode Current => pattern[index];

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                index--;
                return index >= 0;
            }

            public void Reset()
            {
                index = pattern.Count;
            }

            public void Dispose() { }

            public RevPatEnumer(IReadOnlyList<KeyCode> pattern)
            {
                index = pattern.Count;
                this.pattern = pattern;
            }
        }

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
            return new RevPatEnumer(pattern);
        }

        public override string ToString()
        {
            return patternStr;
        }
    }
}