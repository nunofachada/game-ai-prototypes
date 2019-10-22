/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;

public class PathFollowingBehaviour : SeekBehaviour
{
    // Path offset: how far to look ahead for target from current position in
    // path
    [SerializeField] private float pathOffset = 5f;

    // The path object we'll use
    private IPath path = null;

    // Current distance along path
    private float currentParam = 0;

    // Get the path object
    protected override void Start()
    {
        base.Start();
        path = GetComponent<IPath>();
    }

    // Path following behaviour (non-predictive)
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        SteeringOutput steeringOutput;

        // Do we have a path object to follow?
        if (path != null)
        {
            // Target position in path
            Vector2 targetPosition;

            // Get nearest point in path from current agent position
            currentParam =
                path.GetParam(transform.position, currentParam);

            // Update parameter (distance along path) and position
            currentParam += pathOffset;
            targetPosition = path.GetPosition(currentParam);

            // Create temporary target with determined position
            target = CreateTarget(targetPosition, 0f);

            // Use seek to obtain desired steering
            steeringOutput = base.GetSteering(target);

            // Destroy temporary game object
            Destroy(target);
        }
        else
        {
            // Something not right
            Debug.LogWarning("No path object found!");
            steeringOutput = new SteeringOutput(Vector2.zero, 0f);
        }

        // Return steering
        return steeringOutput;

    }

}
