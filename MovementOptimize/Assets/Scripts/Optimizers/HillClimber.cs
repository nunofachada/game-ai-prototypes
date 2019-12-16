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

        public ISolution Optimize(
            int maxSteps,
            float criteria,
            Func<ISolution> initialSolution,
            int runs = 1)
        {

            float bestEvaluation = float.NaN;
            ISolution bestSolution = null;

            // Perform algorithm runs
            for (int i = 0; i < runs; i++)
            {
                float currentEvaluation;
                ISolution currentSolution;
                float bestEvaluationInRun;
                ISolution bestSolutionInRun = null;

                // For current run, get an initial solution
                currentSolution = initialSolution();
                currentEvaluation = evaluate(currentSolution);
                bestSolutionInRun = currentSolution;
                bestEvaluationInRun = currentEvaluation;
                if (bestSolution == null)
                {
                    bestSolution = currentSolution;
                    bestEvaluation = currentEvaluation;
                }

                // Perform a run of the algorithm
                for (int j = 0; j < maxSteps; j++)
                {
                    // Find random neighbor
                    ISolution neighborSolution = findNeighbor(currentSolution);

                    // Evaluate neighbor
                    float neighborEvaluation = evaluate(neighborSolution);

                    // Select solution (either keep current solution or
                    // select neighbor solution)
                    if (select(neighborEvaluation, currentEvaluation))
                    {
                        currentSolution = neighborSolution;
                        currentEvaluation = neighborEvaluation;

                        if (compare(currentEvaluation, bestEvaluationInRun))
                        {
                            bestSolutionInRun = currentSolution;
                            bestEvaluationInRun = currentEvaluation;
                        }
                    }

                    // If we reached the criteria, break loop
                    if (compare(currentEvaluation, criteria)) break;
                }

                // Is last run's best solution better than the best solution
                // found so far in all runs?
                if (compare(bestEvaluationInRun, bestEvaluation))
                {
                    bestSolution = bestSolutionInRun;
                    bestEvaluation = bestEvaluationInRun;
                }
            }
            return bestSolution;
        }
    }
}