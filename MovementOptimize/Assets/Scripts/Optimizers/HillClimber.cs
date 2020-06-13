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
        // Random number generator
        private Random random;

        // Acceleration parameter for adaptive steps
        private float accel;

        // Solution domain
        private (float min, float max)[] solutionDomain;

        // Maximum change in each parameter
        private float[] deltas;

        // Function for evaluating a solution
        private Func<IList<float>, float> evaluate;

        // Function for comparing two solutions based on their evaluation
        private Func<float, float, bool> compare;

        // Function for performing simulated annealing by modifying the
        // value of an evaluation
        private Func<float> annealing;

        // Update annealing temperature
        private Action<int> updateTemperature;

        // Sign multiplier
        private int sign;

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
        /// Number of evaluations performed in current/last optimization.
        /// </summary>
        public int Evaluations { get; private set; }

        public HillClimber(
            IList<(float, float)> solutionDomain,
            IList<float> deltas,
            Func<IList<float>, float> evaluate,
            bool minimize = true,
            float t0 = 0f,
            float r = 0.1f,
            float accel = 1f,
            int? seed = null)
        {
            // Keep accel parameter
            this.accel = accel;

            // Keep solution domain
            this.solutionDomain =
                new (float min, float max)[solutionDomain.Count];
            solutionDomain.CopyTo(this.solutionDomain, 0);

            // Keep deltas
            this.deltas = new float[deltas.Count];
            deltas.CopyTo(this.deltas, 0);

            // Keep evaluation function
            this.evaluate = evaluate;

            // Initialize random number generator
            random = seed.HasValue ? new Random(seed.Value) : new Random();

            // Determine comparison function
            if (minimize)
            {
                compare = (a, b) => a < b;
                sign = 1;
            }
            else
            {
                compare = (a, b) => a > b;
                sign = -1;
            }

            // Perform direct simulated annealing?
            if (t0 > 0)
            {
                // Set current temperature to initial temperature
                float t = t0;

                // Set function for performing simulated annealing
                annealing = () =>
                    t * (float)(random.NextDouble() - random.NextDouble());

                // Set function for updating (decreasing) temperature
                updateTemperature =
                    (int step) => t = t0 * (float)Math.Exp(-r * step);
            }
        }

        /// <summary>
        /// Optimize / search for a good solution.
        /// </summary>
        /// <param name="maxSteps">Maximum steps per run.</param>
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
            // Determine modifications to perform to each parameter
            // Depends whether there is an acceleration factor or not
            float[] modif = accel > 1.0f
                ? new float[] { -accel, -1f / accel, 0, 1f / accel, accel }
                : new float[] { -1, 0, 1 };

            // Set number of evaluations performed to zero
            Evaluations = 0;

            // Best evaluation and solution
            BestEvaluation = sign * float.PositiveInfinity;
            BestSolution = null;

            // Perform algorithm runs
            for (int run = 0; run < runs; run++)
            {
                // For current run, get an initial solution
                CurrentSolution = initialSolution();
                BestSolutionInRun = CurrentSolution;

                // Perform a run of the algorithm
                for (int step = 0; step < maxSteps; step++)
                {
                    // Update temperature, if necessary
                    updateTemperature?.Invoke(step);

                    // Evaluate current solution
                    CurrentEvaluation = Evaluate(CurrentSolution);

                    // If it's the best in run, keep solution
                    if (compare(CurrentEvaluation, BestEvaluationInRun))
                    {
                        BestSolutionInRun = CurrentSolution;
                        BestEvaluationInRun = CurrentEvaluation;
                    }

                    // If we reached the criteria, break loop
                    if (compare(CurrentEvaluation, criteria)) break;

                    // Loop through each parameter
                    for (int i = 0; i < deltas.Length; i++)
                    {
                        // Best modif index so far
                        int bestModifIdx = -1;
                        float bestEval = sign * float.PositiveInfinity;

                        // Apply modifications to current parameter
                        for (int j = 0; j < modif.Length; j++)
                        {
                            // Temporary evaluation
                            float tempEval = 0;

                            // Determine current modification
                            float currentModif = deltas[i] * modif[j];
                            float newValue = CurrentSolution[i] + currentModif;

                            // Is current modif outside the solution domain?
                            if (newValue < solutionDomain[i].min
                                ||
                                newValue > solutionDomain[i].max)
                            {
                                // if so, penalize it
                                tempEval = sign * float.PositiveInfinity;
                            }
                            else
                            {
                                // Otherwise apply current modification,
                                // evaluate, and reset parameter
                                CurrentSolution[i] += currentModif;

                                // Perform required evaluations for current
                                // solution
                                for (int k = 0; k < evalsPerSolution; k++)
                                    tempEval += Evaluate(CurrentSolution);

                                // Get the mean of the evaluations and add the
                                // annealing parameter
                                tempEval = tempEval / evalsPerSolution
                                    + annealing?.Invoke() ?? 0;

                                // Reset parameter
                                CurrentSolution[i] -= currentModif;
                            }

                            // Is the temporary evaluation the best one?
                            if (compare(tempEval, bestEval))
                            {
                                bestModifIdx = j;
                                bestEval = tempEval;
                            }
                        }

                        // Was no modification the best modification?
                        if (bestModifIdx == 2)
                        {
                            // Deaccelerate search wrt this parameter
                            deltas[i] /= accel;
                        }
                        else
                        {
                            // Keep best modification
                            CurrentSolution[i] +=
                                deltas[i] * modif[bestModifIdx];

                            // Update acceleration correspondingly
                            deltas[i] *= Math.Abs(modif[bestModifIdx]);
                        }
                    }
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
            return new Result(BestSolution, BestEvaluation, Evaluations);
        }

        private float Evaluate(IList<float> solution)
        {
            Evaluations++;
            return evaluate(solution);
        }
    }
}
