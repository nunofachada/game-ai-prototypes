/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

namespace LibGameAI.FSMs
{
    /// <summary>
    /// A FSM state.
    /// </summary>
    public class State
    {
        // Name of the FSM state
        public string Name { get; }

        // Actions to perform when entering this state
        public Action EntryActions { get; }
        // Actions to perform while in this state
        public Action StateActions { get; }
        // Actions to perform when exiting this state
        public Action ExitActions { get; }

        // Public property exposing the transactions associated with this state
        public IEnumerable<Transition> Transitions => transitions;

        // Internal list of the transactions associated with this state
        private IList<Transition> transitions;

        // Create a new state
        public State(string name,
            Action entryActions, Action stateActions, Action exitActions)
        {
            Name = name;
            EntryActions = entryActions;
            StateActions = stateActions;
            ExitActions = exitActions;
            transitions = new List<Transition>();
        }

        // Add a transition from this state to another state
        public void AddTransition(Transition transition)
        {
            transitions.Add(transition);
        }
    }
}
