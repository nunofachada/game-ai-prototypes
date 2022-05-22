using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIUnityExample.NGramsFight.UI;

namespace AIUnityExample.NGramsFight
{

    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private DialogManager dialogManager;

        private Agent player, enemy;

        private InputFrontend inputFrontend;

        public int Level { get; private set; }

        private void Awake()
        {
            player = GetComponentInChildren<Player>();
            enemy = GetComponentInChildren<Enemy>();
            inputFrontend = GetComponent<InputFrontend>();
            inputFrontend.enabled = false;
        }

        // Start is called before the first frame update
        private void Start()
        {
            Level = 1;
            OnNextLevel?.Invoke();
            dialogManager.Dialog("NGrams Fight!", "Start game?", StartLevel);
        }

        private void OnEnable()
        {
            player.OnDie += GameOver;
            enemy.OnDie += WinLevel;
        }

        private void OnDisable()
        {
            player.OnDie -= GameOver;
            enemy.OnDie -= WinLevel;
        }

        private void StartLevel()
        {
            player.ResetHealth();
            enemy.ResetHealth();
            inputFrontend.enabled = true;
        }

        private void GameOver()
        {
            inputFrontend.enabled = false;
            dialogManager.Dialog("Game Over!", "Start new game?", StartLevel);
        }

        private void WinLevel()
        {
            inputFrontend.enabled = false;
            dialogManager.Dialog("Enemy defeated!", "Continue to next level?", NextLevel);
        }

        private void NextLevel()
        {
            // Increment level
            Level++;

            // Notify listeners that the level was incremented
            OnNextLevel?.Invoke();

            // Start level
            StartLevel();
        }

        public void Quit()
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

        public event Action OnNextLevel;
    }
}