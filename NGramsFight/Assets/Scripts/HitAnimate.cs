// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using System.Collections;
using UnityEngine;

namespace GameAIPrototypes.NGramsFight
{
    /// <summary>
    /// This script creates a simple scaling animation for when the object it's
    /// attached to takes a hit and/or takes some damage.
    /// </summary>
    public class HitAnimate : MonoBehaviour
    {
        // Maximum scaling when game object is hit and/or takes damage
        [SerializeField]
        private float maxScaleWhenHit = 1.2f;

        // Duration of the animation in seconds
        [SerializeField]
        private float hitAnimSeconds = 0.5f;

        // Base scale of the game object
        private Vector3 baseScale;

        // Called when the script instance is being loaded
        private void Awake()
        {
            baseScale = transform.localScale;
        }

        // Co-routine which performs the scaling animation
        private IEnumerator Hit()
        {
            // Time at which the animation started
            float timeStart = Time.time;

            // Time elapsed since the animation started
            float timeElapsed;

            // First part of the animation: increase scaling linearly
            do
            {
                timeElapsed = Time.time - timeStart;
                float scaleMult = Mathf.Lerp(1, maxScaleWhenHit, 2 * timeElapsed / hitAnimSeconds);
                transform.localScale = baseScale * scaleMult;
                yield return null;
            } while (timeElapsed < hitAnimSeconds / 2);

            // Second part of the animation: decrease scaling linearly
            do
            {
                timeElapsed = Time.time - timeStart;
                float scaleMult = Mathf.Lerp(1, maxScaleWhenHit, 2 - 2 * timeElapsed / hitAnimSeconds);
                transform.localScale = baseScale * scaleMult;
                yield return null;
            } while (timeElapsed < hitAnimSeconds);

            // Make sure the game object's scale is back to the base scale
            // after the animation finishes
            transform.localScale = baseScale;
        }

        /// <summary>
        /// Start the animation on the game object this script is attached to.
        /// </summary>
        public void Animate()
        {
            // The animation is handled by a co-routine
            StartCoroutine(Hit());
        }

    }
}