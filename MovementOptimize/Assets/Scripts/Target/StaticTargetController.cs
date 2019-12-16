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
    [SerializeField] private GameObject target = default;

    // How long between target destruction and target spawn
    [SerializeField] private float delay = 1f;

    // The game area
    private GameArea gameArea;

    private TextMesh textMesh;

    public float Points { get; private set; } = 0;

    // Use this for initialization
    private void Start()
    {
        textMesh = GameObject.Find("Points").GetComponent<TextMesh>();
        UpdatePoints(0);
        gameArea = new GameArea();
        SpawnTarget();
    }

    public void NotifyDestructionBy(Component destroyer)
    {
        UpdatePoints(1);
        Invoke("SpawnTarget", delay);
    }

    private void UpdatePoints(float morePoints)
    {
        Points += morePoints;
        textMesh.text = $"Points: {Points}";
    }

    // Spawn a new target at a random location
    private void SpawnTarget()
    {
        Vector2 pos = gameArea.RandomPosition(0.9f);
        Instantiate(target, pos, Quaternion.identity, transform);
    }
}
