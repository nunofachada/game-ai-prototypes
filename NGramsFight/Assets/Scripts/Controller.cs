using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Controller : MonoBehaviour
    {

        [SerializeField]
        private GameObject viewGameObject;

        private KnownInput[] buffer;
        private int index;
        private IView view;

        private const int BUFFER_SIZE = 5;

        // Start is called before the first frame update
        private void Awake()
        {
            buffer = new KnownInput[BUFFER_SIZE];
            index = 0;
            view = viewGameObject.GetComponent<IView>();
        }

        private void OnEnable()
        {
            view.PressedInput += HandleInput;
        }

        private void OnDisable()
        {
            view.PressedInput -= HandleInput;
        }

        // Update is called once per frame
        private void Update()
        {

        }

        private void HandleInput(KnownInput input)
        {
            index++;
            if (index >= BUFFER_SIZE)
            {
                Array.Clear(buffer, 0, BUFFER_SIZE);
                index = 0;
            }


        }
    }
}