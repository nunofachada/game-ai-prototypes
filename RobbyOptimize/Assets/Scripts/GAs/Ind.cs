/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using System.Linq;

namespace LibGameAI.GAs
{
    public class Ind<G>
    {
        private List<G> genes;

        public G this[int i]
        {
            get => genes[i];
            set => genes[i] = value;
        }

        public float? Fit { get; set; }

        public IList<G> GenesView => genes.AsReadOnly();

        public int GeneCount => genes.Count;

        public Ind(int numGenes)
        {
            genes = new List<G>(numGenes);
            Fit = null;
        }

        public Ind(IEnumerable<G> genes)
        {
            this.genes = genes.ToList();
            Fit = null;
        }

        public Ind<G> Copy()
        {
            return new Ind<G>(genes);
        }
    }
}