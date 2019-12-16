/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

namespace LibGameAI.Optimizers
{
    public struct Result
    {
        public ISolution Solution { get; }
        public float Fitness { get; }
        public int Evaluations { get; }

        public Result(ISolution solution, float fitness, int evaluations)
        {
            Solution = solution;
            Fitness = fitness;
            Evaluations = evaluations;
        }

        public override string ToString() =>
            $"Best fitness is {Fitness} at {Solution} " +
            $"(took me {Evaluations} evaluations to get there)";
    }
}