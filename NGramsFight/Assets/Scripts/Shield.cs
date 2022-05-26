// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    /// <summary>
    /// Script to be attached to the enemy's shield.
    /// </summary>
    public class Shield : MonoBehaviour
    {
        // Reference to the animator script, for when the shield is hit
        private HitAnimate hitAnimate;

        // Called when the script instance is being loaded
        private void Awake()
        {
            hitAnimate = GetComponent<HitAnimate>();
        }

        // Method invoked when another object enters a trigger collider attached
        // to this object (in this case, when a shot hits the shield)
        private void OnTriggerEnter2D(Collider2D collider)
        {
            // Perform animation when a shot hits the shield
            hitAnimate?.Animate();
        }
    }
}