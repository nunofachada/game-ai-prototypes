// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using UnityEngine;
using LibGameAI.NGrams;
using LibGameAI.Util;
using NaughtyAttributes;

namespace GameAIPrototypes.NGramsFight
{
    /// <summary>
    /// This script is attached to the Enemy agent, interfacing with the N-Grams
    /// library in order to provide the enemy with player attack predictions.
    /// </summary>
    public class Predictor : MonoBehaviour
    {
        // N-Grams configuration
        [Tooltip("The N value in the N-Gram")]
        [Range(0, 12)]
        [SerializeField]
        private int nValue = 4;

        [Tooltip("Use hierarchical N-Gram?")]
        [SerializeField]
        private bool hierarchical = true;

        [Tooltip("How many times a sequence has to be seen in order to " +
            "make a prediction (hierarchical N-Grams only)")]
        [Range(1, 12)]
        [SerializeField]
        [ShowIf(nameof(hierarchical))]
        private int threshold = 3;

        // Pattern configuration
        private Patterns patterns;

        // Reference to the N-Gram predictor
        private INGram<KeyCode> predictor;

        // List of key presses
        private RingList<KeyCode> keyPresses;

        // Last prediction
        private KeyCode prediction;

        // Reference to the input frontend
        private InputFrontend inputFrontend;

        // Reference to the Enemy script
        private Enemy enemy;

        // Called when the script instance is being loaded
        private void Awake()
        {
            inputFrontend = GetComponentInParent<InputFrontend>();
            patterns = GetComponentInParent<Patterns>();
            enemy = GetComponent<Enemy>();
        }

        /// <summary>
        /// This method is usually called on the frame when a script is enabled
        /// before any of the Update methods are called the first time, but in
        /// this case it's also invoked by the <see cref="GameController"/>
        /// for reinitializing the N-Gram predictor.
        /// </summary>
        public void Start()
        {
            // Initialize N-Gram
            predictor = hierarchical
                ? new HierarchNGram<KeyCode>(nValue, threshold) as INGram<KeyCode>
                : new NGram<KeyCode>(nValue) as INGram<KeyCode>;

            // Initialize list of key presses
            keyPresses = new RingList<KeyCode>(nValue);

            // Initialize prediction to none
            prediction = default;
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

        // Callback to be invoked when a key is pressed
        private void HandleInput(KeyCode keyCode)
        {
            // Add key code to sequence, older ones are automatically discarded
            // since we're using a ring list
            keyPresses.Add(keyCode);

            // Register sequence with the N-Gram predictor
            predictor.RegisterSequence(keyPresses);

            // Make prediction for next input
            prediction = predictor.GetMostLikely(keyPresses);

            // Compose a lightweight collection containing the sequence plus the
            // prediction
            var seqPlusPred = new ListPlusOneWrapper<KeyCode>(keyPresses, prediction);

            // Does the key code sequence + key code prediction corresponds to
            // an attack?
            AttackType? attack = patterns.Match(seqPlusPred);

            // If so, inform enemy of the predicted attack
            if (attack.HasValue)
            {
                enemy.ReceivePrediction(attack.Value);
            }
        }
    }
}