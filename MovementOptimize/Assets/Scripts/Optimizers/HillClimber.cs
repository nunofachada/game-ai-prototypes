/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections;
using System.Collections.Generic;

namespace LibGameAI.Optimizers
{
    public class HillClimber
    {
        private Func<ISolution, ISolution> findNeighbor;
        private Func<ISolution, float> evaluate;
        private Func<float, float, bool> compare;
        private Func<float, float, bool> select;

        public float BestEvaluation { get; set; }
        public ISolution BestSolution { get; set; }
        public float BestEvaluationInRun { get; set; }
        public ISolution BestSolutionInRun { get; set; }
        public float CurrentEvaluation { get; set; }
        public ISolution CurrentSolution { get; set; }

        public HillClimber(
            Func<ISolution, ISolution> findNeighbor,
            Func<ISolution, float> evaluate,
            Func<float, float, bool> compare,
            Func<float, float, bool> select)
        {
            this.findNeighbor = findNeighbor;
            this.evaluate = evaluate;
            this.compare = compare;
            this.select = select;
        }

        public Result Optimize(
            int maxSteps,
            float criteria,
            Func<ISolution> initialSolution,
            int runs = 1,
            int evalsPerSolution = 1)
        {
            int evaluations = 0;
            BestEvaluation = float.NaN;
            BestSolution = null;

            // Perform algorithm runs
            for (int i = 0; i < runs; i++)
            {
                BestSolutionInRun = null;

                // For current run, get an initial solution
                CurrentSolution = initialSolution();
                CurrentEvaluation = evaluate(CurrentSolution);
                evaluations++;
                BestSolutionInRun = CurrentSolution;
                BestEvaluationInRun = CurrentEvaluation;

                // Perform a run of the algorithm
                for (int j = 0; j < maxSteps; j++)
                {
                    // Find random neighbor
                    ISolution neighborSolution = findNeighbor(CurrentSolution);
                    float neighborEvaluation = 0;

                    // Evaluate neighbor
                    for (int k = 0; k < evalsPerSolution; k++)
                    {
                        neighborEvaluation += evaluate(neighborSolution);
                        evaluations++;
                    }
                    neighborEvaluation = neighborEvaluation / evalsPerSolution;

                    // Select solution (either keep current solution or
                    // select neighbor solution)
                    if (select(neighborEvaluation, CurrentEvaluation))
                    {
                        CurrentSolution = neighborSolution;
                        CurrentEvaluation = neighborEvaluation;

                        if (compare(CurrentEvaluation, BestEvaluationInRun))
                        {
                            BestSolutionInRun = CurrentSolution;
                            BestEvaluationInRun = CurrentEvaluation;
                        }
                    }

                    // If we reached the criteria, break loop
                    if (compare(CurrentEvaluation, criteria)) break;
                }

                // Is last run's best solution better than the best solution
                // found so far in all runs?
                if (compare(BestEvaluationInRun, BestEvaluation)
                    || BestSolution == null)
                {
                    BestSolution = BestSolutionInRun;
                    BestEvaluation = BestEvaluationInRun;
                }
            }
            return new Result(BestSolution, BestEvaluation, evaluations);
        }
    }
}