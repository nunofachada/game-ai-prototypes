using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
namespace AIUnityExample.NGramsFight
{

    public class View : MonoBehaviour, IView
    {
        private ISet<KeyCode> validInputs;

        // Start is called before the first frame update
        private void Awake()
        {
            validInputs = null;
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (KeyCode input in validInputs)
            {
                if (Input.GetKeyUp(input))
                {
                    OnPressedInput?.Invoke(input);
                }
            }
        }

        public void SetValidInputs(ISet<KeyCode> validInputs)
        {
            if (this.validInputs != null)
            {
                throw new InvalidOperationException(
                    "Valid inputs can only be set once in the view.");
            }
            this.validInputs = validInputs;
        }

        public event Action<KeyCode> OnPressedInput;
    }
}