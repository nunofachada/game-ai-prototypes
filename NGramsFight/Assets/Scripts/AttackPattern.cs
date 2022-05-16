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

        private Stack<string> patStack;

        public string Next => patStack.Count > 0 ? patStack.Pop() : null;

        public string Pattern => pattern;

        public AttackType Attack => attack;

        public IReadOnlyCollection<string> Preprocess()
        {
            patStack = new Stack<string>();
            foreach (string s in pattern.Split(','))
            {
                patStack.Push(s.Trim());
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