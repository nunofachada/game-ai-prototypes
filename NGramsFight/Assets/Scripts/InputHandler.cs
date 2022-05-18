using System;
using System.Collections.Generic;
using UnityEngine;
namespace AIUnityExample.NGramsFight
{

    public class InputHandler : MonoBehaviour
    {
        private ISet<KeyCode> knownInputs;

        // Start is called before the first frame update
        private void Awake()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            foreach (KeyCode input in knownInputs)
            {
                if (Input.GetKeyUp(input))
                {
                    OnPressedInput?.Invoke(input);
                }
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