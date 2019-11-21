/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace LibGameAI.DecisionTrees
{
    public class RandomDecisionBehaviour
    {
        // Last frame this decision was queried
        private int lastFrame = -1;
        // The frame where this decision will timeout
        private int timeoutFrame = -1;
        // The last decision taken
        private bool lastDecision = false;

        // How many frames before this decision times out
        private int timeout;
        // Probability of a true decision
        private float trueProb;
        // Delegate representing a function which returns a random value
        // between 0 and 1
        private Func<float> nextRandValFunc;
        // Delegate representing a function which returns the current frame
        private Func<int> getFrameFunc;

        /// <summary>
        /// Creates a new random decision behaviour.
        /// </summary>
        /// <param name="nextRandValFunc">
        /// A function which returns a random value between 0 and 1.
        /// </param>
        /// <param name="getFrameFunc">
        /// A function which returns the current frame.
        /// </param>
        /// <param name="timeOut">
        /// How many frames before this decision times out.
        /// </param>
        /// <param name="trueProb">
        /// Probability of a true decision (0.5f by default).
        /// </param>
        public RandomDecisionBehaviour(
            Func<float> nextRandValFunc, Func<int> getFrameFunc,
            int timeOut, float trueProb = 0.5f)
        {
            this.timeout = timeOut;
            this.nextRandValFunc = nextRandValFunc;
            this.getFrameFunc = getFrameFunc;
            this.trueProb = trueProb;
        }

        /// <summary>
        /// Makes a true or false decision, or keeps the previous decision if
        /// the world state hasn't changed and the decision hasn't timed out.
        /// </summary>
        /// <returns>True or false.</returns>
        public bool RandomDecision()
        {

            // Check if our stored decision is too old, or if we've timed out
            if (getFrameFunc() > lastFrame + 1
                || getFrameFunc() > timeoutFrame)
            {
                // Make a new decision and store it
                lastDecision = nextRandValFunc() < trueProb;

                // Schedule the next new decision
                timeoutFrame = getFrameFunc() + timeout;
            }

            // Either way we need to store when we were last called
            lastFrame = getFrameFunc();

            // We return the stored value
            return lastDecision;
        }
    }
}
