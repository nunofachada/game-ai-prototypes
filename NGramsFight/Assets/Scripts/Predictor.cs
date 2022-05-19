using UnityEngine;
using LibGameAI.NGrams;
using LibGameAI.Util;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    public class Predictor : MonoBehaviour
    {
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

        // Reference to the N-Gram predictor
        private INGram<KeyCode> predictor;

        // List of key presses
        private RingList<KeyCode> keyPresses;

        // Last prediction
        private KeyCode prediction;

        // Input handler
        private InputFrontend inputFrontend;

        // Move configuration
        private Patterns patterns;

        private void Awake()
        {
            inputFrontend = GetComponentInParent<InputFrontend>();
            patterns = transform.parent.GetComponentInChildren<Patterns>();
        }

        // Use this for initialization
        private void Start()
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

        private void OnEnable()
        {
            inputFrontend.OnPressedInput += HandleInput;
        }

        private void OnDisable()
        {
            inputFrontend.OnPressedInput -= HandleInput;
        }

        // Handle input
        private void HandleInput(KeyCode keyCode)
        {
            // Add key code to sequence (older ones are automatically discarded)
            keyPresses.Add(keyCode);

            // Register sequence
            predictor.RegisterSequence(keyPresses);

            // Make prediction for next input
            prediction = predictor.GetMostLikely(keyPresses);

            //Debug.Log($"PREDICTION: {prediction}");

            // Compose the collection with sequence plus prediction
            var seqPlusPred = new ListPlusOneWrapper<KeyCode>(keyPresses, prediction);

            // Does the key prediction predict an attack?
            AttackType? attack = patterns.Match(seqPlusPred);

            if (attack.HasValue)
            {
                Debug.Log($"Attack Prediction: {attack}");
            }
        }
    }
}