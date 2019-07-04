/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using NaiveBayes;

public class Player : MonoBehaviour
{

    // The weapon sprites
    [SerializeField] private Sprite swordSprite;
    [SerializeField] private Sprite bowSprite;

    // Property that exposes the total player damage to outside objects
    public int Damage => damage;

    // Total player damage
    private int damage;

    public bool IsAI { get; set; }

    // Sprite renderer for the player weapon
    private SpriteRenderer spriteRenderer;

    // The currently selected player weapon
    private PlayerWeapon weapon;

    // UI widgets
    private Text uiMessages;
    private Text uiDamage;
    private Text uiAi;

    // These variables are related to the AI
    private NaiveBayesClassifier nbClassifier;
    private Attrib enemyTypeAttrib, speedAttrib;
    private int aiObservations = 0;

    private void Awake()
    {
        // Make sure the weapon sprites are set
        Assert.IsNotNull(swordSprite);
        Assert.IsNotNull(bowSprite);

        // Get reference to the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get references to the several UI widgets
        uiMessages = GameObject.Find("TextMessages").GetComponent<Text>();
        uiDamage = GameObject.Find("TextDamage").GetComponent<Text>();
        uiAi = GameObject.Find("TextAI").GetComponent<Text>();

        // Initially the player is not supposed to be controlled by the AI
        IsAI = false;

        // Initialize the AI
        InitAI();
    }

    // Method for initializing the AI
    private void InitAI()
    {
        // Create two attributes and specify their possible values
        enemyTypeAttrib =
            new Attrib("enemyType", Enum.GetNames(typeof(EnemyType)));
        speedAttrib =
            new Attrib("speed", Enum.GetNames(typeof(SpeedClassification)));

        // Create a naive Bayes classifier with a set of labels and a
        // set of attributes
        nbClassifier = new NaiveBayesClassifier(
            Enum.GetNames(typeof(PlayerWeapon)),
            new Attrib[] { enemyTypeAttrib, speedAttrib });
    }

    // Method called when Player game object is activated
    private void OnEnable()
    {
        spriteRenderer.sprite = swordSprite;
        weapon = PlayerWeapon.Sword;
        SetDamage(0);
    }

    // Update total player damage
    private void SetDamage(int dam)
    {
        damage = dam;
        uiDamage.text = $"Damage = {dam}";
    }

    // Update is called once per frame
    private void Update()
    {
        // Only check for inputs if player is not being controlled by the AI
        if (!IsAI)
        {
            // The game is played by a human player
            if (Input.GetKeyDown(KeyCode.A))
            {
                spriteRenderer.sprite = bowSprite;
                weapon = PlayerWeapon.Bow;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                spriteRenderer.sprite = swordSprite;
                weapon = PlayerWeapon.Sword;
            }
        }
    }

    // Deal damage to player
    public void DealDamage(Enemy enemy)
    {
        // The partial damages
        int damageFromEnemy = 0, damageFromSpeed = 0, totalDamage;
        // Get the enemy type
        EnemyType enemyType = enemy.Type;
        // Get the speed category
        SpeedClassification speedCategory = enemy.SpeedCategory;

        // Determine damage if player is holding a sword
        if (weapon == PlayerWeapon.Sword)
        {
            // Determine damage due to enemy type
            if (enemyType == EnemyType.Demon)
                damageFromEnemy = 2;
            else if (enemyType == EnemyType.Dragon)
                damageFromEnemy = 5;

            // Determine damage due to enemy speed
            if (speedCategory == SpeedClassification.Slow)
                damageFromSpeed = 8;
            else if (speedCategory == SpeedClassification.Normal)
                damageFromSpeed = 4;
            else if (speedCategory == SpeedClassification.Fast)
                damageFromSpeed = 2;
        }

        // Determine damage if player is holding a bow
        else if (weapon == PlayerWeapon.Bow)
        {
            // Determine damage due to enemy type
            if (enemyType == EnemyType.Demon)
                damageFromEnemy = 4;
            else if (enemyType == EnemyType.Dragon)
                damageFromEnemy = 3;

            // Determine damage due to enemy speed
            if (speedCategory == SpeedClassification.Slow)
                damageFromSpeed = 1;
            else if (speedCategory == SpeedClassification.Normal)
                damageFromSpeed = 3;
            else if (speedCategory == SpeedClassification.Fast)
                damageFromSpeed = 7;
        }

        // Determine total damage player will take
        totalDamage = damageFromEnemy * damageFromSpeed;

        // Update UI with damage taken by player
        uiMessages.text = $"+{totalDamage} damage from "
            + $"{speedCategory.ToString()} {enemyType.ToString()}";

        // Update total player damage
        SetDamage(damage + totalDamage);

        // Is the player being controlled by the AI?
        if (!IsAI)
        {
            // If the game is not being played by the AI, pass the player's
            // choice to the AI so it can learn
            nbClassifier.Update(
                weapon.ToString(),
                new Dictionary<Attrib, string>()
                {
                    { enemyTypeAttrib, enemyType.ToString() },
                    { speedAttrib, speedCategory.ToString() }
                }
            );

            // Update the number of AI observations and the respective text
            // widget
            aiObservations++;
            uiAi.text = $"AI Observations = {aiObservations}";
        }
    }

    // This method is invoked when a new enemy is spawned
    public void NotifyEnemySpawned(Enemy enemy)
    {
        // Only do something if AI is in charge of player
        if (IsAI)
        {

            // Ask AI to make a prediction and choose weapon
            string weaponStr = nbClassifier.Predict(
                new Dictionary<Attrib, string>()
                {
                    { enemyTypeAttrib, enemy.Type.ToString() },
                    { speedAttrib, enemy.SpeedCategory.ToString() }
                });

            // Convert the string returned by the AI into a weapon
            Enum.TryParse<PlayerWeapon>(weaponStr, out weapon);

            // Check what weapon was selected and act upon it
            switch (weapon)
            {
                case PlayerWeapon.Bow:
                    spriteRenderer.sprite = bowSprite;
                    break;
                case PlayerWeapon.Sword:
                    spriteRenderer.sprite = swordSprite;
                    break;
            }

        }
    }

}
