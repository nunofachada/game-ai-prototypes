/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace AIUnityExamples.Movement.Core
{
    public struct SteeringOutput
    {
        // Linear force
        public Vector2 Linear { get; }

        // Torque (angular force)
        public float Angular { get; }

        // Builds a new instance of a steering output
        public SteeringOutput(Vector2 linear, float angular)
        {
            Linear = linear;
            Angular = angular;
        }
    }
}