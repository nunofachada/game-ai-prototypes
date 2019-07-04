/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Enemy : MonoBehaviour
{

    // Enemy sprites
    [SerializeField] private Sprite demonSprite;
    [SerializeField] private Sprite dragonSprite;

    // The enemy type
    private EnemyType type;
    // Enemy speed
    private float speed;
    // Reference to the enemy controller, which is the parent game object
    private EnemyController parent;

    // Property that exposes the enemy type to outside objects
    public EnemyType Type => type;
    // Property that exposes the enemy speed to outside objects
    public float Speed => speed;

    // Properties that exposes the enemy speed category to outside objects
    public SpeedClassification SpeedCategory
    {
        get
        {
            // How many speed categories exist?
            float numSpeedCategs =
                Enum.GetNames(typeof(SpeedClassification)).Length;

            // What is the speed increment that determines a new speed category?
            float speedIncrement =
                (parent.MaxSpeed - parent.MaxSpeed / 10f)
                / numSpeedCategs;

            // We'll increment this variable to determine the speed category
            float speedCheck = parent.MaxSpeed / 10f;

            // The variable which will contain the speed category
            SpeedClassification speedCategory = (SpeedClassification)0;

            // Determine the speed category
            do
            {
                // Increment speed check variable
                speedCheck += speedIncrement;

                // Are we over the current enemy speed? If so, leave the loop
                if (speedCheck > speed) break;

                // Increase the speed category
                speedCategory = (SpeedClassification)(speedCategory + 1);

            } while (speedCheck < parent.MaxSpeed);

            // Return the speed category
            return speedCategory;
        }
    }

    // Initialize a newly created enemy
    public void Initialize(EnemyType type, float speed, EnemyController parent)
    {
        // Keep parameters
        this.type = type;
        this.speed = speed;
        this.parent = parent;

        // Determine sprite to use
        switch (type)
        {
            case EnemyType.Demon:
                GetComponent<SpriteRenderer>().sprite = demonSprite;
                break;
            case EnemyType.Dragon:
                GetComponent<SpriteRenderer>().sprite = dragonSprite;
                break;
        }
    }

    // Enemy moves towards player each frame
    private void Update()
    {
        transform.position = new Vector2(
            transform.position.x - Speed * Time.deltaTime,
            transform.position.y);
    }

    // Method called when enemy hits player
    private void OnTriggerEnter2D(Collider2D otherGuy)
    {
        // Deal damage to player
        otherGuy.GetComponent<Player>().DealDamage(this);
        // Notify my parent (EnemyController) that I'm going to be destroyed
        // and he should spawn a new enemy
        SendMessageUpwards("ScheduleEnemySpawn");
        // Destroy myself
        Destroy(gameObject);
    }
}
