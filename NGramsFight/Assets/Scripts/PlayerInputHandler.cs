// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    /// <summary>
    /// This script handles input for the player agent.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        // Reference to the configured attack patterns
        private Patterns patterns;

        // Time (seconds) a valid keypress is considered relevant for performing
        // an attack
        private float keyValidDuration;

        // The player's input buffer
        private LinkedList<TimedInput> buffer;

        // Reference to the input frontend script
        private InputFrontend inputFrontend;

        // Reference to the player script
        private Player player;

        // Minimum and maximum buffer size for being matched with a pattern
        private (int min, int max) bufferSize;

        // Auxiliary list, key codes in buffer will be copied into this list
        // before being matched with a pattern
        private List<KeyCode> auxList;

        // Called when the script instance is being loaded
        private void Awake()
        {
            patterns = GetComponentInParent<Patterns>();

            inputFrontend = GetComponentInParent<InputFrontend>();

            player = GetComponent<Player>();

            keyValidDuration = inputFrontend.KeyValidDuration;

            buffer = new LinkedList<TimedInput>();
        }

        // Called on the frame when a script is enabled before any of the Update
        // methods are called the first time
        private void Start()
        {
            bufferSize = (patterns.MinLength, patterns.MaxLength);

            auxList = new List<KeyCode>(patterns.MaxLength + 1);
        }

        // Called when the object becomes enabled and active
        // We use it to attach listeners to events
        private void OnEnable()
        {
            inputFrontend.OnPressedInput += HandleInput;
        }

        // Called when the behaviour becomes disabled
        // We use it to remove listeners from events
        private void OnDisable()
        {
            inputFrontend.OnPressedInput -= HandleInput;
        }

        // Update is called once per frame
        private void Update()
        {
            // Remove key codes that are too old (older than keyValidDuration)
            while (buffer.Count > 0 && Time.time > buffer.First.Value.Time + keyValidDuration)
            {
                buffer.RemoveFirst();
            }
        }


        // This method is called in a frame-rate independent fashion for
        // performing physics calculations
        // We use it to match the current buffer contents with an attack pattern
        // This could probably be in Update() too
        private void FixedUpdate()
        {
            // Is the buffer the right size to be matched with an attack pattern?
            if (buffer.Count >= bufferSize.min && buffer.Count <= bufferSize.max)
            {
                // Copy key codes in buffer to the auxiliary list
                auxList.Clear();
                foreach (TimedInput timedInput in buffer)
                {
                    auxList.Add(timedInput.Input);
                }

                // Match the key code sequence with an attack pattern
                AttackType? attack = patterns.Match(auxList);
                if (attack.HasValue)
                {
                    // If an attack patter was found, perform it
                    buffer.Clear();
                    player.PerformAttack(attack.Value);
                }
            }
        }

        // Callback to be invoked when a key is pressed
        private void HandleInput(KeyCode input)
        {
            // If an actual key was pressed apply key press damage to player
            if (input != KeyCode.None) player.TakeKeyPressDamage();

            // Add key code and time key was pressed to the player's input buffer
            buffer.AddLast(new TimedInput(Time.time, input));

            // If the buffer is over it's maximum size, remove oldest key code
            if (buffer.Count > bufferSize.max)
            {
                buffer.RemoveFirst();
            }
        }
    }
}