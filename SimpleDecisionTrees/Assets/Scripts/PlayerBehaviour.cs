/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    // Player speed
    [SerializeField]
    [Range(1, 100)]
    private float speed = 50f;

    // Simple player controller
    private void Update()
    {
        // Get controller input
        Vector3 direction = new Vector3(
            Input.GetAxis("Horizontal"),
            0f,
            Input.GetAxis("Vertical"));

        // Move player
        transform.position += direction * speed * Time.deltaTime;
    }
}
