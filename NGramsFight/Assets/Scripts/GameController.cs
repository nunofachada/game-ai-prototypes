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

        private Predictor predictor;

        private InputFrontend inputFrontend;

        private int level;

        public int Level
        {
            get => level;
            private set
            {
                level = value;
                OnChangeLevel?.Invoke();
            }
        }

        private void Awake()
        {
            player = GetComponentInChildren<Player>();
            enemy = GetComponentInChildren<Enemy>();
            predictor = GetComponentInChildren<Predictor>();
            inputFrontend = GetComponent<InputFrontend>();
            inputFrontend.enabled = false;
        }

        // Start is called before the first frame update
        private void Start()
        {
            dialogManager.Dialog("NGrams Fight!", "Start game?", BeginGame);
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

        private void NextLevel()
        {
            Level++;
            player.ResetHealth();
            enemy.ResetHealth();
            inputFrontend.enabled = true;
        }

        private void BeginGame()
        {
            Level = 0;
            predictor.Start();
            NextLevel();
        }

        private void GameOver()
        {
            inputFrontend.enabled = false;
            dialogManager.Dialog("Game Over!", "Start new game?", BeginGame);
        }

        private void WinLevel()
        {
            inputFrontend.enabled = false;
            dialogManager.Dialog("Enemy defeated!", "Continue to next level?", NextLevel);
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

        public event Action OnChangeLevel;
    }
}