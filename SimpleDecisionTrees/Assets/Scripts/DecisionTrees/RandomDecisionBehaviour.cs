/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace LibGameAI.DecisionTrees
{
    /// <summary>
    /// Helper class which performs a random decision and maintains it during
    /// a specified duration and while the tree node containing it continues to
    /// be invoked every frame.
    /// </summary>
    public class RandomDecisionBehaviour
    {
        // Last time this decision was queried
        private float lastTime = -1;

        // The time where this decision will timeout
        private float timeoutTime = -1;

        // The last decision taken
        private bool lastDecision = false;

        // How much time before this decision times out
        private float timeoutDuration;

        // Probability of a true decision
        private float trueProb;

        // Delegate representing a function which returns a random value
        // between 0 and 1
        private Func<float> nextRandValFunc;

        // Delegate representing a function which returns the current time
        private Func<float> getTimeFunc;

        /// <summary>
        /// Creates a new random decision behaviour.
        /// </summary>
        /// <param name="nextRandValFunc">
        /// A function which returns a random value between 0 and 1.
        /// </param>
        /// <param name="getTimeFunc">
        /// A function which returns the current time.
        /// </param>
        /// <param name="timeoutDuration">
        /// How many seconds before this decision times out.
        /// </param>
        /// <param name="trueProb">
        /// Probability of a true decision (0.5f by default).
        /// </param>
        public RandomDecisionBehaviour(
            Func<float> nextRandValFunc, Func<float> getTimeFunc,
            float timeoutDuration, float trueProb = 0.5f)
        {
            this.timeoutDuration = timeoutDuration;
            this.nextRandValFunc = nextRandValFunc;
            this.getTimeFunc = getTimeFunc;
            this.trueProb = trueProb;
        }

        /// <summary>
        /// Makes a true or false decision, or keeps the previous decision if
        /// the world state hasn't changed and the decision hasn't timed out.
        /// </summary>
        /// <returns>True or false.</returns>
        public bool RandomDecision()
        {
            // Get the current time
            float currentTime = getTimeFunc();

            // Check if our stored decision is too old, or if we've timed out
            if (currentTime > lastTime + 0.1 || currentTime > timeoutTime)
            {
                // Make a new decision and store it
                lastDecision = nextRandValFunc() < trueProb;

                // Schedule the next new decision
                timeoutTime = currentTime + timeoutDuration;
            }

            // Either way we need to store when we were last called
            lastTime = currentTime;

            // We return the stored value
            return lastDecision;
        }
    }
}
