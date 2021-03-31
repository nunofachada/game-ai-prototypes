/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using AIUnityExamples.Movement.Core;

namespace AIUnityExamples.Movement.Dynamic.Behaviours
{
    public class PlayerBehaviour : SteeringBehaviour
    {
        // Player behaviour
        public override SteeringOutput GetSteering(GameObject target)
        {
            // Declare 2D acceleration vector
            Vector2 linear;

            // Get player input
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            // Normalize direction
            linear = (new Vector2(x, y)).normalized;

            // Give full acceleration along this direction
            linear = linear.normalized * MaxAccel;

            // Output the steering
            return new SteeringOutput(linear, 0);
        }
    }
}