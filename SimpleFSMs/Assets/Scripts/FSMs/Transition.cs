/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace LibGameAI.FSMs
{
    /// <summary>
    /// A transition between states.
    /// </summary>
    public class Transition
    {

        // Actions associated with this transition
        public Action Actions { get; }
        // Target state for this transition
        public State TargetState { get; }

        // The condition for triggering this transition
        private Func<bool> condition;

        // Is this transition triggered?
        public bool IsTriggered()
        {
            return condition();
        }

        // Create a new transition
        public Transition(
            Func<bool> condition, Action actions, State targetState)
        {
            this.condition = condition;
            Actions = actions;
            TargetState = targetState;
        }
    }
}
