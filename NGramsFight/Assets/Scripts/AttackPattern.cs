using System;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    [Serializable]
    public class AttackPattern
    {
        [SerializeField]
        private string pattern;

        [SerializeField]
        private AttackType attack;

        private Stack<KeyCode> patStack;

        public KeyCode Next => patStack.Count > 0 ? patStack.Pop() : KeyCode.None;

        public string Pattern => pattern;

        public AttackType Attack => attack;

        public IReadOnlyCollection<KeyCode> Preprocess()
        {
            patStack = new Stack<KeyCode>();
            foreach (string keyStr in pattern.Split(','))
            {
                if (!string.IsNullOrEmpty(keyStr))
                {
                    patStack.Push(Event.KeyboardEvent(keyStr.Trim()).keyCode);
                }
            }

            return patStack;
        }

        public AttackPattern(string pattern, AttackType attack)
        {
            this.pattern = pattern;
            this.attack = attack;
        }
    }
}