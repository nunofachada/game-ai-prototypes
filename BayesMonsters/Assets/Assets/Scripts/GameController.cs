/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{


    // User interface references
    private Text uiMessages;
    private Text uiDamage;

    private Button newHumanGameButton;
    private Button newAIGameButton;
    private Button quitButton;

    // Contains references to all the buttons
    private Button[] allButtons;

    // References to the player and to the enemy controller game objects
    private GameObject player, enemyController;

    // Method invoked when the application starts
    private void Awake()
    {
        // Get references to UI elements
        uiMessages = GameObject.Find("TextMessages").GetComponent<Text>();
        uiDamage = GameObject.Find("TextDamage").GetComponent<Text>();

        newHumanGameButton =
            GameObject.Find("ButtonNewPlayer").GetComponent<Button>();
        newAIGameButton =
            GameObject.Find("ButtonNewAI").GetComponent<Button>();
        quitButton =
            GameObject.Find("ButtonQuit").GetComponent<Button>();

        // Keep all buttons in an array
        allButtons = new Button[]
            { newHumanGameButton, newAIGameButton, quitButton };

        // Clear UI text fields
        uiMessages.text = "";
        uiDamage.text = "";

        // Add methods to listen to button clicks
        newHumanGameButton.onClick.AddListener(StartHumanGame);
        newAIGameButton.onClick.AddListener(StartAIGame);
        quitButton.onClick.AddListener(Quit);

        // Get reference to the player and enemy controller game objects
        player = GameObject.Find("Player");
        enemyController = GameObject.Find("EnemyController");

        // Disable the player and enemy controller game object
        player.SetActive(false);
        enemyController.SetActive(false);

        // Register the player's NotifyEnemySpawned() method to listen
        // for enemies spawning
        enemyController.GetComponent<EnemyController>().spawnEnemy +=
            player.GetComponent<Player>().NotifyEnemySpawned;

    }

    // Method called when user clicks in new human game button
    private void StartHumanGame()
    {
        StartGame();
        player.GetComponent<Player>().IsAI = false;
    }

    // Method called when user clicks in new AI game button
    private void StartAIGame()
    {
        StartGame();
        player.GetComponent<Player>().IsAI = true;
    }

    // Method called when user clicks the quit button
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

    // Start a new game
    private void StartGame()
    {
        // Disable UI buttons
        foreach (Button b in allButtons) b.gameObject.SetActive(false);
        // Clear UI text
        uiMessages.text = "";
        // Activate the player and enemy controller game objects
        player.SetActive(true);
        enemyController.SetActive(true);
        enemyController.GetComponent<EnemyController>().ScheduleEnemySpawn();
    }

    // End current game
    public void EndCurrentGame()
    {
        // Disable the player and enemy controller game objects
        player.SetActive(false);
        enemyController.SetActive(false);
        // Enable UI buttons
        foreach (Button b in allButtons) b.gameObject.SetActive(true);
        // Update UI text with game over message
        uiMessages.text = "Game Over!";
    }

}
