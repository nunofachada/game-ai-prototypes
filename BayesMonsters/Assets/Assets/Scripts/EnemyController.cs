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
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{

    // This should have the enemy prefab
    [SerializeField]
    private GameObject enemyPrototype;

    // Maximum enemy speed
    [SerializeField]
    [Range(MINSPEED, MAXSPEED)]
    private float maxSpeed = (MINSPEED + MAXSPEED) / 2f;

    // Maximum spawn interval
    [SerializeField]
    [Range(0f, 10f)]
    private float maxSpawnInterval = 3f;

    // Number of enemies per game
    [SerializeField]
    [Range(1, 30)]
    private int enemiesPerGame = 10;

    // Constants
    public const float MAXSPEED = 40f;
    public const float MINSPEED = MAXSPEED / 10f;

    // Property that exposes the maximum speed to outside objects
    public float MaxSpeed => maxSpeed;

    // Reference to the game controller
    private GameController gameController;

    // Number of enemies spawned so far in the current game
    private int spawnedEnemies;

    // Method called when the application loads
    private void Awake()
    {
        // Make sure the enemy prefab is specified
        Assert.IsNotNull(enemyPrototype);
        // Get reference to the game controller
        gameController =
            GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Method called when this object is activated
    private void OnEnable()
    {
        spawnedEnemies = 0;
    }

    // Method called when it's necessary to schedule the spawn of a new enemy
    public void ScheduleEnemySpawn()
    {
        // Do we still have enemies to spawn?
        if (spawnedEnemies < enemiesPerGame)
            // If so, spawn new enemy
            Invoke("SpawnEnemy", Random.Range(0, maxSpawnInterval));
        else
            // Otherwise end current game
            gameController.EndCurrentGame();

        // Increment the number of spawned enemies
        spawnedEnemies++;
    }

    // Spawn a new enemy
    private void SpawnEnemy()
    {
        // The new enemy to spawn
        Enemy enemyScript;

        // Randomize enemy properties
        EnemyType enemyType =
            Random.value < 0.5 ? EnemyType.Demon : EnemyType.Dragon;
        float speed = Random.Range(maxSpeed / 10f, maxSpeed);

        // Spawn enemy
        GameObject enemy = Instantiate(enemyPrototype, transform, false);
        enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.Initialize(enemyType, speed, this);

        // Notify listeners that an enemy was spawned
        OnSpawnEnemy(enemyScript);
    }

    // Notify listeners that an enemy was spawned
    private void OnSpawnEnemy(Enemy enemy)
    {
        // Only invoke event if there are listeners
        spawnEnemy?.Invoke(enemy);
    }

    // Event which outside objects can register in order to be notified when
    // an enemy is spawned
    public event Action<Enemy> spawnEnemy;

}
