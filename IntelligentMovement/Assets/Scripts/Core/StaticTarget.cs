/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using UnityEngine.Events;

namespace AIUnityExamples.Movement.Core
{
    // Defines a static target
    public class StaticTarget : MonoBehaviour
    {
        // Event raised when this target has been destroyed
        public UnityEvent Destroyed { get; private set; }

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            Destroyed = new UnityEvent();
        }

        // Target is destroyed if someone collides with it
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Destroy this target
            Destroy(this.gameObject);

            // Notify listeners this target has been destroyed
            Destroyed.Invoke();
        }
    }
}