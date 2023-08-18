/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace AIUnityExamples.Movement.Paths
{
    // Interface that defines how paths work
    public interface IPath
    {
        float GetParam(Vector2 position, float lastParam);

        Vector2 GetPosition(float param);
    }
}