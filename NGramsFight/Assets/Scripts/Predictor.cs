using System.Collections.Generic;
using UnityEngine;
using LibGameAI.NGrams;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    public class Predictor : MonoBehaviour
    {
        // Reference to the view game object
        [HideInInspector]
        [SerializeField]

        private GameObject viewGameObject;
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

        // Reference to the view script
        private IView view;

        // Reference to the N-Gram predictor
        private INGram<KeyCode> predictor;

        // List of key presses
        private List<KeyCode> keyPresses;

        // Last prediction
        private KeyCode prediction;

        private void Awake()
        {
            view = viewGameObject.GetComponent<IView>();
        }

        // Use this for initialization
        private void Start()
        {
            // Initialize N-Gram
            predictor = hierarchical
                ? new HierarchNGram<KeyCode>(nValue, threshold) as INGram<KeyCode>
                : new NGram<KeyCode>(nValue) as INGram<KeyCode>;

            // Initialize list of key presses
            keyPresses = new List<KeyCode>();

            // Initialize prediction to none
            prediction = default;
        }

        private void OnEnable()
        {
            view.OnPressedInput += HandleInput;
        }

        private void OnDisable()
        {
            view.OnPressedInput -= HandleInput;
        }

        // Handle input
        private void HandleInput(KeyCode keyCode)
        {
            // Add key code to sequence
            keyPresses.Add(keyCode);

            // Register sequence
            predictor.RegisterSequence(keyPresses);

            // Remove first if list equal or larger than nValue
            if (keyPresses.Count >= nValue)
                keyPresses.RemoveAt(0);

            // Make prediction for next input
            prediction = predictor.GetMostLikely(keyPresses);
        }
    }
}