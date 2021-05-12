/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Text;
using System.Collections.Generic;

namespace LibGameAI.GAs
{
    public class GeneticAlgorithm<I, F>
    {
        private readonly int popCount;

        private readonly Func<I> generate;
        private readonly Comparison<F> compare;
        private readonly Action<IList<IndFit<I, F>>, IList<IndFit<I, F>>> select;
        private readonly Action<IList<IndFit<I, F>>, IList<IndFit<I, F>>> mate;
        private readonly Action<IList<IndFit<I, F>>> mutate;
        private readonly Func<I, F> evaluate;

        private IndFit<I, F>[] popCurrent;
        private IndFit<I, F>[] popNext;

        private int step;
        private IndFit<I, F> best;

        public GeneticAlgorithm(
            int popCount,
            Func<I> generate,
            Comparison<F> compare,
            Action<IList<IndFit<I, F>>, IList<IndFit<I, F>>> select,
            Action<IList<IndFit<I, F>>, IList<IndFit<I, F>>> mate,
            Action<IList<IndFit<I, F>>> mutate,
            Func<I, F> evaluate)
        {
            this.popCount = popCount;

            popCurrent = new IndFit<I, F>[popCount];
            popNext = new IndFit<I, F>[popCount];

            this.generate = generate;
            this.compare = compare;
            this.select = select;
            this.mate = mate;
            this.mutate = mutate;
            this.evaluate = evaluate;

            // for (int i = 0; i < popCount; i++)
            // {
            //     popCurrent[i] = (new Reaction[TileUtil.numRules], float.NegativeInfinity);
            //     popNext[i] = (new Reaction[TileUtil.numRules], float.NegativeInfinity);
            // }

            // Reset();
        }

        public void Init()
        {
            step = 0;

            // Generate a new population and determine each individual's fitness
            for (int i = 0; i < popCount; i++)
            {
                I ind = generate();
                F fit = evaluate(ind);
                popCurrent[i] = new IndFit<I, F>(ind, fit);

                if (i == 0 || compare(fit, best.Fit) > 0)
                {
                    best = popCurrent[i];
                }
            }
        }

        public I Run(int steps, F goalFit)
        {

            IndFit<I, F>[] aux;

            // Run the genetic algorithm until a maximum number of steps or
            // a goal fitness is reached
            for (int i = 0; i < steps && compare(best.Fit, goalFit) < 0; step++, i++)
            {
                // Selection
                select(popCurrent, popNext);

                // Crossover
                mate(popCurrent, popNext);

                // Mutation
                mutate(popNext);

                // Update population
                aux = popCurrent;
                popCurrent = popNext;
                popNext = aux;

                // Evaluate new generation

            }

            // Return best solution found so far
            return best.Ind;
        }


        // public void Reset()
        // {
        //     step = 0;
        //     for (int i = 0; i < popCount; i++)
        //     {
        //         world.GenerateRandomRules(popCurrent[i].rules);
        //         popCurrent[i].fitness = float.NegativeInfinity;
        //     }
        // }

        // public void Step()
        // {
        //     EvalCurrentPop();
        // }

        // private void EvalCurrentPop()
        // {

        // }

    }
}