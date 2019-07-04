/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NGrams;

public class TestNGrams : MonoBehaviour
{

    [Tooltip("The N value in the N-Gram")]
    [Range(0, 12)]
    [SerializeField] private int nValue = 4;

    [Tooltip("Show accuracy after how many clicks?")]
    [Range(2, 20)]
    [SerializeField] private int showAccuracyInterval = 10;


    [Tooltip("Use hierarchical N-Gram?")]
    [SerializeField] private bool useHierarchicalNGram = true;

    [Tooltip("How many times a sequence has to be seen in order to " +
        "make a prediciton (hierarchical N-Grams only)")]
    [Range(1, 12)]
    [SerializeField] private int threshold = 3;

    // Reference to the N-Gram predictor
    private INGram<char> predictor;

    // List of mouse presses (L or R)
    private List<char> listOfChars;

    // Right and wrong predictions
    private int right, wrong;

    // Last prediction
    private char prediction;

    // Use this for initialization
    void Start()
    {

        // Initialize N-Gram
        predictor = useHierarchicalNGram
            ? new HierarchNGram<char>(nValue, threshold) as INGram<char>
            : new NGram<char>(nValue) as INGram<char>;

        // Initialize list of chars
        listOfChars = new List<char>();

        // Initialize right and wrong predictions
        right = 0;
        wrong = 0;

        // Initialize prediction to none
        prediction = default(char);
    }

    // Update is called once per frame
    void Update()
    {
        // Whether the user clicked the mouse buttons in this frame
        bool clicked = false;

        // Capture mouse button presses
        if (Input.GetMouseButtonDown(0))
        {
            listOfChars.Add('L');
            clicked = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            listOfChars.Add('R');
            clicked = true;
        }

        // Did the user clicked the mouse buttons in this frame?
        if (clicked)
        {
            // Show input and what we had predicted with the N-Gram
            Debug.Log($"Input='{listOfChars[listOfChars.Count - 1]}' "
                + $"| Prediction='{prediction}'");

            // Determine if prediction was accurate and update right or wrong
            // accordingly
            if (listOfChars[listOfChars.Count - 1] == prediction) right++;
            else wrong++;

            // Time to show accuracy?
            if ((right + wrong) % showAccuracyInterval == 0)
                Debug.LogWarning(
                    $"Current accuracy: {((float)right) / (right + wrong):p}");

            // Register sequence
            predictor.RegisterSequence(listOfChars.ToArray());

            // Remove first if list equal or larger than nValue
            if (listOfChars.Count >= nValue)
                listOfChars.RemoveAt(0);

            // Make prediction for next input
            prediction = predictor.GetMostLikely(listOfChars.ToArray());
        }
    }

    // Show the final accuracy when the application terminates
    void OnApplicationQuit()
    {
        if (right + wrong > 0)
            Debug.LogWarning(
                $"FINAL ACCURACY: {((float)right) / (right + wrong):p}");
    }

}
