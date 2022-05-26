// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using UnityEngine;
using TMPro;

namespace AIUnityExample.NGramsFight.UI
{
    /// <summary>
    /// Updates the current level label.
    /// </summary>
    public class LevelLabelUpdater : MonoBehaviour
    {
        // Reference to the game controller object
        [SerializeField]
        private GameController gameController;

        // Reference to the text label
        private TextMeshProUGUI text;

        // Called on the frame when a script is enabled before any of the Update
        // methods are called the first time
        private void Start()
        {
            // Get reference to the level text label
            text = GetComponent<TextMeshProUGUI>();

            // Update the level's text label
            UpdateLevelLabel();
        }

        // Called when the object becomes enabled and active
        private void OnEnable()
        {
            // Register listener for when the level changes
            gameController.OnChangeLevel += UpdateLevelLabel;
        }

        // Called when the behaviour becomes disabled
        private void OnDisable()
        {
            // Unregister listener, so it won't be invoked while the component
            // is disabled
            gameController.OnChangeLevel -= UpdateLevelLabel;
        }

        // Update the level label
        private void UpdateLevelLabel()
        {
            text?.SetText($"Level {gameController.Level:d3}");
        }
    }
}