/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

public class RotateBehaviour : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotateSpeed = new Vector3(0f, 3f, 0f);

    // Rotate the object
    private void Update()
    {
        transform.Rotate(rotateSpeed);
    }
}
