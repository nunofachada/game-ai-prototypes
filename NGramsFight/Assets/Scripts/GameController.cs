using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{

    public class GameController : MonoBehaviour
    {
        private Agent player, enemy;

        public int Level { get; private set; }

        private void Awake()
        {
            player = GetComponentInChildren<Player>();
            enemy = GetComponentInChildren<Enemy>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            Level = 1;
            OnNextLevel?.Invoke();
        }

        private void OnEnable()
        {
            player.OnDie += GameOver;
            enemy.OnDie += NextLevel;
        }

        private void OnDisable()
        {
            player.OnDie -= GameOver;
            enemy.OnDie -= NextLevel;
        }

        private void GameOver()
        {
            // Disable input


            // Notify listeners

        }

        private void NextLevel()
        {
            // Disable input


            // Notify listeners

        }

        public event Action OnNextLevel;

        public event Action OnGameOver;
    }
}