/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;

namespace LibGameAI.FSMs
{
    /// <summary>
    /// A transition between states.
    /// </summary>
    public class Transition
    {
        /// <summary>
        /// Actions associated with this transition.
        /// </summary>
        public Action Actions { get; }

        /// <summary>
        /// Target state for this transition.
        /// </summary>
        public State TargetState { get; }

        // The condition for triggering this transition
        private Func<bool> condition;

        /// <summary>
        /// Is this transition triggered?
        /// </summary>
        /// <returns>
        /// <c>true</c> if transition is triggered, <c>false</c> otherwise.
        /// </returns>
        public bool IsTriggered()
        {
            return condition();
        }

        /// <summary>
        /// Create a new transition.
        /// </summary>
        /// <param name="condition">
        /// Condition for triggering this transition.
        /// </param>
        /// <param name="actions">
        /// Actions associated with this transition.
        /// </param>
        /// <param name="targetState">
        /// Target state for this transition.
        /// </param>
        public Transition(
            Func<bool> condition, Action actions, State targetState)
        {
            this.condition = condition;
            Actions = actions;
            TargetState = targetState;
        }
    }
}
