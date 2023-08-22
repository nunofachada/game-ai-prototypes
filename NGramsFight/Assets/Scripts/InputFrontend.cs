// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAIPrototypes.NGramsFight
{
    /// <summary>
    /// The input frontend, captures valid keystrokes and notifies registered
    /// listeners.
    /// </summary>
    public class InputFrontend : MonoBehaviour
    {
        [Tooltip("Time (seconds) a valid keypress is considered relevant for performing an attack")]
        [SerializeField]
        private float keyValidDuration = 1.5f;

        // Time a valid key was last pressed
        private float lastKeyTime;

        // Was the last "valid" input a "no input"?
        private bool lastInputKeyNone;

        // Set of known inputs
        private ISet<KeyCode> knownInputs;

        /// <summary>
        /// Time (seconds) a valid keypress is considered relevant (read-only).
        /// </summary>
        public float KeyValidDuration => keyValidDuration;

        // Called on the frame when a script is enabled before any of the Update
        // methods are called the first time
        private void Start()
        {
            lastKeyTime = Time.time;
            lastInputKeyNone = false;
        }

        // Update is called once per frame
        private void Update()
        {
            // Capture valid keypresses in the current frame
            foreach (KeyCode input in knownInputs)
            {
                if (Input.GetKeyUp(input))
                {
                    OnPressedInput?.Invoke(input);
                    lastKeyTime = Time.time;
                    lastInputKeyNone = false;
                }
            }

            // If the last input wasn't a "no input" and no key has been pressed
            // for a while, insert a "no input" in the buffer
            if (!lastInputKeyNone && Time.time > lastKeyTime + keyValidDuration)
            {
                OnPressedInput?.Invoke(KeyCode.None);
                lastInputKeyNone = true;
            }
        }

        /// <summary>
        /// Set the known / valid inputs. Can only be called once.
        /// </summary>
        /// <param name="knownInputs">Set of known inputs.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the valid inputs have already been set.
        /// </exception>
        public void SetKnownInputs(ISet<KeyCode> knownInputs)
        {
            if (this.knownInputs != null)
            {
                throw new InvalidOperationException(
                    "Valid inputs can only be set once in the view.");
            }
            this.knownInputs = knownInputs;
        }

        /// <summary>
        /// Event raised when a valid key is pressed.
        /// </summary>
        public event Action<KeyCode> OnPressedInput;
    }
}