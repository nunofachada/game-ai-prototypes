/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameAIPrototypes.Movement.Core
{
    /// <summary>
    /// Base class for all agents, provides methods to find targets quickly and
    /// without resorting to `Find` methods.
    /// </summary>
    /// <remarks>
    /// Tags are still used to find agents: consider that this is not a good
    /// practice in larger projects.
    /// </remarks>
    public class Agent : MonoBehaviour
    {

        /// <summary>
        /// Finding agents only works properly if they're all under a game
        /// object literally named `Agents`.
        /// </summary>
        protected void Awake()
        {
            if (transform.parent.name != "Agents")
            {
                Debug.LogError(
                    $"An instance of {GetType()} is not under the `Agents` "
                    + "game object and might not work properly!");
            }
        }

        /// <summary>
        /// Get first target agent with the given tag.
        /// </summary>
        /// <param name="tag">The tag the target must have.</param>
        /// <returns>The first found target with the given tag.</returns>
        public GameObject GetFirstTarget(string tag)
        {
            return string.IsNullOrEmpty(tag)
                ? null
                : GetAllTargets(tag, true).FirstOrDefault();
        }

        /// <summary>
        /// Return all target agents with the specified tag.
        /// </summary>
        /// <param name="tag">Tag that the agents must have.</param>
        /// <param name="firstOnly">Return only the first found agent?</param>
        /// <returns>Agents with the specified tag.</returns>
        public IEnumerable<GameObject> GetAllTargets(string tag, bool firstOnly = false)
        {
            foreach (Transform child in transform.parent)
            {
                if (child.CompareTag(tag) && child != transform)
                {
                    yield return child.gameObject;
                    if (firstOnly) break;
                }
            }
        }
    }
}