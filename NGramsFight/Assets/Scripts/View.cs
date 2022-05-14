using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{

    public class View : MonoBehaviour, IView
    {

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {

            if (Input.GetKeyUp(KeyCode.W))
            {
                Debug.Log("up!");
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                Debug.Log("down!");
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                Debug.Log("right!");
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                Debug.Log("left!");
            }

        }

        public event Action<KnownInput> PressedInput;
    }
}