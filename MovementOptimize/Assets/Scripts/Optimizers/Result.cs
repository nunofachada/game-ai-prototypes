/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;

namespace LibGameAI.Optimizers
{
    public struct Result
    {
        public IList<float> Solution { get; }
        public float Fitness { get; }
        public int Evaluations { get; }

        public Result(IList<float> solution, float fitness, int evaluations)
        {
            Solution = solution;
            Fitness = fitness;
            Evaluations = evaluations;
        }
    }
}
