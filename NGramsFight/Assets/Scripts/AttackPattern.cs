using System;
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

    }
}