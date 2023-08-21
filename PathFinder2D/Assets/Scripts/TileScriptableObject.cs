/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;

namespace GameAIPrototypes.PathFinder2D
{
    [CreateAssetMenu(
        fileName = "Data",
        menuName = "ScriptableObjects/TileScriptableObject",
        order = 1)]
    public class TileScriptableObject : ScriptableObject
    {
        [SerializeField] private Sprite blocked = null;
        [SerializeField] private Sprite empty = null;

        public Sprite Blocked => blocked;
        public Sprite Empty => empty;
    }
}