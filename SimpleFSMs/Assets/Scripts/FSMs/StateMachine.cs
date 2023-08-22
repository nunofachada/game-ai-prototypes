/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;

namespace LibGameAI.FSMs
{
    /// <summary>
    /// A finite state machine.
    /// </summary>
    public class StateMachine
    {
        // Current state
        private State currentState;

        /// <summary>
        /// Create a new FSM.
        /// </summary>
        /// <param name="initialState">Initial state.</param>
        public StateMachine(State initialState)
        {
            currentState = initialState;
        }

        /// <summary>
        /// Update the FSM and return the actions to perform.
        /// </summary>
        /// <returns>Actions to perform.</returns>
        public Action Update()
        {
            // Assume no transition is triggered
            Transition triggeredTransition = null;

            // Check through each transition and store the first one that
            // triggers
            foreach (Transition transition in currentState.Transitions)
            {
                if (transition.IsTriggered())
                {
                    triggeredTransition = transition;
                    break;
                }
            }

            // Check if we have a transition to fire
            if (triggeredTransition != null)
            {
                // Actions to perform when transitioning between states
                Action actions = null;

                // Find the target state
                State targetState = triggeredTransition.TargetState;

                // Add the exit action of the old state, the transition action
                // and the entry for the new state
                actions += currentState.ExitActions;
                actions += triggeredTransition.Actions;
                actions += targetState.EntryActions;

                // Complete the transition and return the action list
                currentState = targetState;
                return actions;
            }

            // If no transition was triggered, return the actions for the
            // current state
            return currentState.StateActions;
        }
    }
}
