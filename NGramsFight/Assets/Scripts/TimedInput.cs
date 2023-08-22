// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Author: Nuno Fachada

using UnityEngine;

namespace GameAIPrototypes.NGramsFight
{
    /// <summary>
    /// Associates a key code with a specific input time.
    /// </summary>
    public struct TimedInput
    {
        /// <summary>
        /// The time at which the key was pressed.
        /// </summary>
        public float Time { get; }

        /// <summary>
        /// The pressed key code.
        /// </summary>
        public KeyCode Input { get; }

        /// <summary>
        /// Create a new key code - input time association.
        /// </summary>
        /// <param name="time">The time at which the key was pressed.</param>
        /// <param name="input">The pressed key code.</param>
        public TimedInput(float time, KeyCode input)
        {
            Time = time;
            Input = input;
        }
    }
}