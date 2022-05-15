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

        private Queue<string> patQueue;

        public string Next => patQueue.Count > 0 ? patQueue.Dequeue() : null;

        public string Pattern => pattern;

        public AttackType Attack => attack;

        public IReadOnlyCollection<string> Preprocess()
        {
            patQueue = new Queue<string>();
            foreach (string s in pattern.Split(','))
            {
                patQueue.Enqueue(s.Trim());
            }

            return patQueue;
        }

        public AttackPattern(string pattern, AttackType attack)
        {
            this.pattern = pattern;
            this.attack = attack;
        }
    }
}