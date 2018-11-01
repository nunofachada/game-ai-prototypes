/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada 
 * */
using UnityEngine;

public class PathFollowingBehaviour : SeekBehaviour
{
    public string pathObjectTag;

    private IPath path = null;
    
    protected override void Start() {
        base.Start();
        path = GameObject.FindWithTag(pathObjectTag);
    }

    // Path following behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        Vector2 linear = Vector2.zero;
        float angular = 0f;

        // Do we have a path object to follow?
        if (path != null)
        {

        }
        else
        {
            Debug.Debug.LogWarning("No path object found!");
        }

        // Output the steering
        return new SteeringOutput(linear, angular);
    }

}
