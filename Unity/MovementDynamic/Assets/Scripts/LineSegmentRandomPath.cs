/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada 
 * */
using UnityEngine;

// Ideas:
// 1 - Make paths using a wander-style generator
// 2 - Allow for explicit paths specified in the Unity Editor in another IPath
// class
public class LineSegmentRandomPath : AbstractPath
{
    public override float GetParam(Vector2 position, float lastParam)
    {
        return 0f;
    }

    public override Vector2 GetPosition(float param)
    {
        return Vector2.zero;
    }
}
