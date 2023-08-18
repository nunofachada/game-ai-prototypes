/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

namespace LibGameAI.GAs
{
    /// <summary>
    /// Basic tournament selection, choses the best between pairs of randomly
    /// selected individuals.
    /// </summary>
    /// <typeparam name="G"></typeparam>
    public class TournamentSelection<G>
    {
        private Random random;

        public TournamentSelection(Random random = null)
        {
            if (random is null)
                this.random = new Random();
            else
                this.random = random;

        }

        public void Select(IList<Ind<G>> curr, IList<Ind<G>> next)
        {

            for (int i = 0; i < next.Count; i++)
            {
                int ind1 = random.Next(curr.Count);
                int ind2 = random.Next(curr.Count);

                next[i] = curr[ind1].Fit > curr[ind2].Fit
                    ? curr[ind1]
                    : curr[ind2];
            }
        }
    }
}