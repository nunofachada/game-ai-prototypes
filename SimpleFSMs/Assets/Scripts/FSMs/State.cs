/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;

namespace LibGameAI.FSMs
{
    /// <summary>
    /// A FSM state.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Name of the FSM state.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Actions to perform when entering this state.
        /// </summary>
        public Action EntryActions { get; }

        /// <summary>
        /// Actions to perform while in this state.
        /// </summary>
        public Action StateActions { get; }

        /// <summary>
        /// Actions to perform when exiting this state.
        /// </summary>
        public Action ExitActions { get; }

        /// <summary>
        /// Public property exposing the transitions associated with this
        /// state.
        /// </summary>
        public IEnumerable<Transition> Transitions => transitions;

        // Internal list of the transitions associated with this state
        private IList<Transition> transitions;

        /// <summary>
        /// Create a new state.
        /// </summary>
        /// <param name="name">Name of the FSM state.</param>
        /// <param name="entryActions">
        /// Actions to perform when entering this state.
        /// </param>
        /// <param name="stateActions">
        /// Actions to perform while in this state.
        /// </param>
        /// <param name="exitActions">
        /// Actions to perform when exiting this state.
        /// </param>
        public State(string name,
            Action entryActions, Action stateActions, Action exitActions)
        {
            Name = name;
            EntryActions = entryActions;
            StateActions = stateActions;
            ExitActions = exitActions;
            transitions = new List<Transition>();
        }

        /// <summary>
        /// Add a transition from this state to another state
        /// </summary>
        /// <param name="transition">Transition to another state.</param>
        public void AddTransition(Transition transition)
        {
            transitions.Add(transition);
        }
    }
}
