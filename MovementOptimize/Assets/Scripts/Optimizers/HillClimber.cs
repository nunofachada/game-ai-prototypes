/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

namespace LibGameAI.Optimizers
{
    public class HillClimber
    {
        // Function for finding neighbors of a given solution
        private Func<IList<float>, IList<float>> findNeighbor;

        // Function for evaluating a solution
        private Func<IList<float>, float> evaluate;

        // Function for comparing two solutions based on their evaluation
        private Func<float, float, bool> compare;

        // Function for selecting between two solutions based on their
        // evaluation
        private Func<float, float, bool> select;

        /// <summary>Best evaluation so far in all finished runs.</summary>
        public float BestEvaluation { get; private set; }

        /// <summary>Best solution so far in all finished runs.</summary>
        public IList<float> BestSolution { get; private set; }

        /// <summary>Best evaluation so far in current run.</summary>
        public float BestEvaluationInRun { get; private set; }

        /// <summary>Best solution so far in current run.</summary>
        public IList<float> BestSolutionInRun { get; private set; }

        /// <summary>Current (last) evaluation.</summary>
        public float CurrentEvaluation { get; private set; }

        /// <summary>Current (last) solution.</summary>
        public IList<float> CurrentSolution { get; private set; }

        /// <summary>
        /// Create a new hill climber instance.
        /// </summary>
        /// <param name="findNeighbor">
        /// Function for finding neighbors of a given solution.
        /// </param>
        /// <param name="evaluate">
        /// Function for evaluating a solution.
        /// </param>
        /// <param name="compare">
        /// Function for comparing two solutions based on their evaluation.
        /// </param>
        /// <param name="select">
        /// Function for selecting between two solutions based on their
        /// evaluation.
        /// </param>
        public HillClimber(
            Func<IList<float>, IList<float>> findNeighbor,
            Func<IList<float>, float> evaluate,
            Func<float, float, bool> compare,
            Func<float, float, bool> select)
        {
            this.findNeighbor = findNeighbor;
            this.evaluate = evaluate;
            this.compare = compare;
            this.select = select;
        }

        /// <summary>
        /// Optimize / search for a good solution.
        /// </summary>
        /// <param name="maxSteps">Maximum steps.</param>
        /// <param name="criteria">Evaluation stopping criteria.</param>
        /// <param name="initialSolution">
        /// Function which returns an initial solution for starting the
        /// search (should be a different initial solution each time the
        /// function is called).
        /// </param>
        /// <param name="runs">
        /// Number of times to start the algorithm from scratch with another
        /// initial solution.
        /// </param>
        /// <param name="evalsPerSolution">
        /// Number of evaluations per solution, in case evaluations are
        /// non-deterministic.
        /// </param>
        /// <returns>
        /// The best solution found, its evaluation and number of evaluations
        /// required to find it.
        /// </returns>
        public Result Optimize(
            int maxSteps,
            float criteria,
            Func<IList<float>> initialSolution,
            int runs = 1,
            int evalsPerSolution = 1)
        {
            int evaluations = 0;
            BestEvaluation = float.NaN;
            BestSolution = null;

            // Perform algorithm runs
            for (int i = 0; i < runs; i++)
            {
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
                    IList<float> neighborSolution = findNeighbor(CurrentSolution);
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

                        // If this is the best evaluation in run, keep that
                        // record
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

            // Return best solution found, its evaluation and number of
            // evaluations performed
            return new Result(BestSolution, BestEvaluation, evaluations);
        }
    }
}
