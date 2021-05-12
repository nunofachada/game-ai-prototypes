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
    public class FlipEnumMutation<G>
    {
        private readonly float flipProb;
        private readonly Random random;
        private readonly G[] geneValues;

        public FlipEnumMutation(float flipProb, Random random = null)
        {
            this.flipProb = flipProb;

            if (random is null)
                this.random = new Random();
            else
                this.random = random;

            geneValues = (G[])Enum.GetValues(typeof(G));
        }

        public void Mutate(Ind<G> ind)
        {
            for (int i = 0; i < ind.GeneCount; i++)
            {
                if (random.NextDouble() < flipProb)
                {
                    G changed;
                    do
                    {
                        changed = geneValues[random.Next(geneValues.Length)];

                    } while (changed.Equals(ind[i]));

                    ind[i] = changed;
                }
            }
        }
    }
}