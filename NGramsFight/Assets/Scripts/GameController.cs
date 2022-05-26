// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System;
using UnityEngine;
using AIUnityExample.NGramsFight.UI;

namespace AIUnityExample.NGramsFight
{
    /// <summary>
    /// The game controller.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        // Reference to the dialog manager
        [SerializeField]
        private DialogManager dialogManager;

        // Agents fighting in the game
        private Agent player, enemy;

        // Reference to the predictor script, which interfaces with the N-Grams
        // library
        private Predictor predictor;

        // Reference to the input frontend
        private InputFrontend inputFrontend;

        // Current level
        private int level;

        /// <summary>
        /// The current level.
        /// </summary>
        public int Level
        {
            get => level;
            private set
            {
                level = value;
                OnChangeLevel?.Invoke();
            }
        }

        // Called when the script instance is being loaded
        private void Awake()
        {
            player = GetComponentInChildren<Player>();
            enemy = GetComponentInChildren<Enemy>();
            predictor = GetComponentInChildren<Predictor>();
            inputFrontend = GetComponent<InputFrontend>();
        }

        // Called on the frame when a script is enabled before any of the Update
        // methods are called the first time
        private void Start()
        {
            // Disable game elements and show start menu / dialog
            DisableGameElements();
            dialogManager.Dialog("NGrams Fight!", "Start game?", BeginGame);
        }

        // Called when the object becomes enabled and active
        // We use it to attach listeners to events
        private void OnEnable()
        {
            player.OnDie += GameOver;
            enemy.OnDie += WinLevel;
        }

        // Called when the behaviour becomes disabled
        // We use it to remove listeners from events
        private void OnDisable()
        {
            player.OnDie -= GameOver;
            enemy.OnDie -= WinLevel;
        }

        // Enable game elements, for when the game is actually taking place
        private void EnableGameElements()
        {
            player.Visible = true;
            enemy.Visible = true;
            inputFrontend.enabled = true;
        }

        // Disable game elements, when the menu dialog is being shown
        private void DisableGameElements()
        {
            inputFrontend.enabled = false;
            player.Visible = false;
            enemy.Visible = false;
        }

        // Start the next level
        private void NextLevel()
        {
            Level++;
            EnableGameElements();
            player.ResetHealth();
            enemy.ResetHealth();
        }

        // Start a new game
        private void BeginGame()
        {
            Level = 0;
            predictor.Start();
            NextLevel();
        }

        // Callback invoked when the game is over
        private void GameOver()
        {
            DisableGameElements();
            dialogManager.Dialog("Game Over!", "Start new game?", BeginGame);
        }

        // Callback invoked when the player wins a level
        private void WinLevel()
        {
            DisableGameElements();
            dialogManager.Dialog("Enemy defeated!", "Continue to next level?", NextLevel);
        }

        // Quit the game
        private void Quit()
        {
#if UNITY_STANDALONE
            // Quit application if running standalone
            Application.Quit();
#endif
#if UNITY_EDITOR
            // Stop game if running in editor
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        /// <summary>
        /// Event raised when the player moves to the next level.
        /// </summary>
        public event Action OnChangeLevel;
    }
}