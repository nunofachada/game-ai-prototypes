/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada 
 * */

using UnityEngine;

// Controls when to spawn a static target
public class StaticTargetController : MonoBehaviour
{

    // Holds an instance of a static target
    public GameObject target;

    // How long between target destruction and target spawn
    public float delay;

    // The game area
    private GameArea gameArea;

    // Use this for initialization
    void Start()
    {
        gameArea = new GameArea();
        SpawnTarget();
    }

    // Called every frame
    private void Update()
    {
        // Find current target
        GameObject target = GameObject.FindWithTag("Target");

        // If no target exists and if we didn't yet schedule target creation...
        if ((target == null) && !IsInvoking("SpawnTarget"))
        {
            //...then schedule target creation
            Invoke("SpawnTarget", delay);
        }
    }

    // Spawn a new target at a random location
    void SpawnTarget()
    {
        Vector2 pos = gameArea.RandomPosition(0.9f);
        Instantiate(target, pos, Quaternion.identity);
    }

}
