using System;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{

    public class InputFrontend : MonoBehaviour
    {
        [SerializeField]
        private float keyValidDuration = 1.5f;

        private float lastKeyTime;

        private bool lastInputKeyNone;

        private ISet<KeyCode> knownInputs;

        public float KeyValidDuration => keyValidDuration;

        private void Start()
        {
            lastKeyTime = Time.time;
            lastInputKeyNone = false;
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (KeyCode input in knownInputs)
            {
                if (Input.GetKeyUp(input))
                {
                    OnPressedInput?.Invoke(input);
                    lastKeyTime = Time.time;
                    lastInputKeyNone = false;
                }
            }

            if (!lastInputKeyNone && Time.time > lastKeyTime + keyValidDuration)
            {
                OnPressedInput?.Invoke(KeyCode.None);
                lastInputKeyNone = true;
            }
        }

        public void SetKnownInputs(ISet<KeyCode> knownInputs)
        {
            if (this.knownInputs != null)
            {
                throw new InvalidOperationException(
                    "Valid inputs can only be set once in the view.");
            }
            this.knownInputs = knownInputs;
        }

        public event Action<KeyCode> OnPressedInput;
    }
}