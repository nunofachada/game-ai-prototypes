using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
namespace AIUnityExample.NGramsFight
{

    public class View : MonoBehaviour, IView
    {
        private ISet<string> validInputs;

        // Start is called before the first frame update
        private void Awake()
        {
            validInputs = null;
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (string input in validInputs)
            {
                if (Input.GetKeyUp(input))
                {
                    OnPressedInput?.Invoke(input);
                }
            }
        }

        public void SetValidInputs(ISet<string> validInputs)
        {
            if (this.validInputs != null)
            {
                throw new InvalidOperationException(
                    "Valid inputs can only be set once in the view.");
            }
            this.validInputs = validInputs;
        }

        public event Action<string> OnPressedInput;
    }
}