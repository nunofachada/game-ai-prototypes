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
    public class GeneticAlgorithm<G>
    {
        private readonly int popSize;
        private readonly float crossoverProb;
        private readonly float mutateProb;

        private readonly Func<Ind<G>> generate;
        private readonly Action<IList<Ind<G>>, IList<Ind<G>>> select;
        private readonly Action<Ind<G>, Ind<G>> mate;
        private readonly Action<Ind<G>> mutate;
        private readonly Action<Ind<G>> evaluate;
        private readonly Random random;

        private Ind<G>[] popCurrent;
        private Ind<G>[] popNext;

        private int step;
        private Ind<G> best;

        public GeneticAlgorithm(
            int popSize,
            float crossoverProb,
            float mutateProb,
            Func<Ind<G>> generate,
            Action<IList<Ind<G>>, IList<Ind<G>>> select,
            Action<Ind<G>, Ind<G>> mate,
            Action<Ind<G>> mutate,
            Action<Ind<G>> evaluate,
            Random random = null)
        {
            this.popSize = popSize;
            this.crossoverProb = crossoverProb;
            this.mutateProb = mutateProb;

            popCurrent = new Ind<G>[popSize];
            popNext = new Ind<G>[popSize];

            this.generate = generate;
            this.select = select;
            this.mate = mate;
            this.mutate = mutate;
            this.evaluate = evaluate;

            if (random is null)
                this.random = new Random();
            else
                this.random = random;
        }

        public void Init()
        {
            step = 0;

            // Generate a new population and determine each individual's fitness
            for (int i = 0; i < popSize; i++)
            {
                Ind<G> ind = generate.Invoke();
                evaluate.Invoke(ind);
                popCurrent[i] = ind;

                if (i == 0 || ind.Fit > best.Fit)
                {
                    best = popCurrent[i];
                }
            }
        }

        public Ind<G> Run(int steps, float goalFit)
        {
            // Aux. variable
            Ind<G>[] aux;

            // Run the genetic algorithm until a maximum number of steps or
            // a goal fitness is reached
            for (int i = 0; i < steps && best.Fit < goalFit; step++, i++)
            {
                // Select the next generation of individuals
                select.Invoke(popCurrent, popNext);

                // Clone the selected individuals
                for (int j = 0; j < popSize; j++)
                {
                    popNext[j] = popNext[j].Copy();
                }

                // Crossover (mate)
                for (int j = 0; j < popSize; j += 2)
                {
                    if (random.NextDouble() < crossoverProb)
                    {
                        mate.Invoke(popNext[j], popNext[j + 1]);
                        popNext[j].Fit = null;
                    }
                }

                // Mutation
                foreach (Ind<G> ind in popNext)
                {
                    if (random.NextDouble() < mutateProb)
                    {
                        mutate.Invoke(ind);
                        ind.Fit = null;
                    }
                }

                // Evaluate new generation
                foreach (Ind<G> ind in popNext)
                {
                    if (!ind.Fit.HasValue)
                    {
                        evaluate.Invoke(ind);
                        if (ind.Fit > best.Fit) best = ind;
                    }
                }

                // Update population
                aux = popCurrent;
                popCurrent = popNext;
                popNext = aux;
            }

            // Return best solution found so far
            return best;
        }
    }
}