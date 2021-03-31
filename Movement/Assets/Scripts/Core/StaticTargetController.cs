/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace AIUnityExamples.Movement.Core
{
    // Controls when to spawn a static target
    public class StaticTargetController : MonoBehaviour
    {
        // The static target prefab
        [SerializeField] private GameObject target = null;

        // How long between target destruction and target spawn
        [SerializeField] private float delay = 0.5f;

        // The game area
        private GameArea gameArea;

        // Use this for initialization
        private void Start()
        {
            gameArea = new GameArea();
            SpawnTarget();
        }

        // Called when the previously created target is destroyed
        private void TargetDestroyed()
        {
            // Schedule the creation of a new target
            Invoke("SpawnTarget", delay);
        }

        // Spawn a new target at a random location
        private void SpawnTarget()
        {
            // Get a random position in the game area
            Vector2 pos = gameArea.RandomPosition(0.9f);

            // Create a new target game object in that random position
            GameObject targetObj = Instantiate(target, pos, Quaternion.identity);

            // Get the script associated with the target game object
            StaticTarget targetScript = targetObj.GetComponent<StaticTarget>();

            // Attach a listener method to be called when the target is destroyed
            targetScript.Destroyed.AddListener(TargetDestroyed);
        }
    }
}