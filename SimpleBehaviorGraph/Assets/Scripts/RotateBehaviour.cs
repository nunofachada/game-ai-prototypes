/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace GameAIPrototypes.BehaviorTrees
{
    public class RotateBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float rotateSpeed = 50f;

        // Rotate the object
        private void Update()
        {
            transform.Rotate(new Vector3(0, rotateSpeed, 0) * Time.deltaTime);
        }
    }
}