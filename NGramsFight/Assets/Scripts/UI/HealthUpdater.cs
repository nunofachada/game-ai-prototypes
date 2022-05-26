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
    /// Updates the agent's health labels.
    /// </summary>
    public class HealthUpdater : MonoBehaviour
    {
        // Reference to the agent.
        [SerializeField]
        private Agent agent;

        // Reference to the text label.
        private TextMeshProUGUI text;

        // Called on the frame when a script is enabled before any of the Update
        // methods are called the first time
        private void Start()
        {
            // Get a reference to the text label component
            text = GetComponent<TextMeshProUGUI>();

            // Update health label
            UpdateHealth();
        }

        // Called when the object becomes enabled and active
        private void OnEnable()
        {
            // Register listener for when the agent's health changes
            agent.OnHealthChange += UpdateHealth;
        }

        // Called when the behaviour becomes disabled
        private void OnDisable()
        {
            // Unregister listener, so it won't be invoked while the component
            // is disabled
            agent.OnHealthChange -= UpdateHealth;
        }

        // Update the health label
        private void UpdateHealth()
        {
            text?.SetText($"{agent.Health:f2}");
        }
    }
}