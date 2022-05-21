using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{

    public class GameController : MonoBehaviour
    {
        public int Level { get; private set; }

        // Start is called before the first frame update
        private void Start()
        {
            Level = 1;
            OnLevelChange?.Invoke();
        }

        // Update is called once per frame
        private void Update()
        {

        }

        public event Action OnLevelChange;
    }
}