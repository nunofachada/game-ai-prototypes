/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using UnityEngine.InputSystem;
using GameAIPrototypes.Movement.Core;

namespace GameAIPrototypes.Movement.Dynamic.Behaviours
{
    /// <summary>
    /// Controls an agent using standard player input.
    /// </summary>
    public class PlayerBehaviour : SteeringBehaviour
    {
        // Player input (movement) object
        private InputAction moveAction;

        // We override Start so we can get input from the player
        protected override void Start()
        {
            // Invoke Start() in the base class
            base.Start();

            // Get input from the player
            moveAction = InputSystem.actions.FindAction("Move");
        }

        // Player behaviour
        public override SteeringOutput GetSteering(GameObject target)
        {
            // Declare 2D acceleration vector
            Vector2 linear;

            // Get player input
            linear = moveAction.ReadValue<Vector2>();

            // Give full acceleration along this direction
            linear = linear.normalized * MaxAccel;

            // Output the steering
            return new SteeringOutput(linear, 0);
        }
    }
}