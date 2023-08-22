// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameAIPrototypes.NGramsFight.UI
{
    /// <summary>
    /// Manages the dialog box that appears to the user.
    /// </summary>
    public class DialogManager : MonoBehaviour
    {
        // Reference to the dialog box title label
        [SerializeField]
        private TextMeshProUGUI titleLabel;

        // Reference to the dialog box text label
        [SerializeField]
        private TextMeshProUGUI textLabel;

        // Reference to the dialog box OK button
        [SerializeField]
        private Button buttonOK;

        // Current callback attached to the dialog box OK button
        private Action currentCallback;

        /// <summary>
        /// Show the dialog box to the user.
        /// </summary>
        /// <param name="title">Dialog box title.</param>
        /// <param name="text">Dialog box text.</param>
        /// <param name="callback">
        /// Callback to be invoked when the OK button is pressed.
        /// </param>
        public void Dialog(string title, string text, Action callback)
        {
            currentCallback = callback;
            titleLabel.SetText(title);
            textLabel.SetText(text);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// This method is invoked when the OK button is pressed.
        /// </summary>
        public void ClickOK()
        {
            // Invoke the callback and then set it to null
            currentCallback?.Invoke();
            currentCallback = null;

            // Disable/hide the dialog
            gameObject.SetActive(false);
        }
    }
}