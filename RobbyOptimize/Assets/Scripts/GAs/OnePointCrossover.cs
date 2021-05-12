/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace LibGameAI.GAs
{
    public class OnePointCrossover<G>
    {
        private readonly Random random;

        public OnePointCrossover(Random random = null)
        {
            if (random is null)
                this.random = new Random();
            else
                this.random = random;
        }

        public void Mate(Ind<G> ind1, Ind<G> ind2)
        {
            int split = random.Next(ind1.GeneCount);

            for (int i = 0; i < split; i++)
            {
                G aux = ind1[i];
                ind1[i] = ind2[i];
                ind2[i] = aux;
            }

            for (int i = split; i < ind1.GeneCount; i++)
            {
                G aux = ind2[i];
                ind2[i] = ind1[i];
                ind1[i] = aux;
            }
        }
    }
}