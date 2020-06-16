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
        // Maximum number of steps per run
        private int maxSteps;

        // Stoping criteria per run
        private float criteria;

        // Minimum improvement required per step
        private float epsilon;

        // Acceleration factor para adaptive steps
        private float accel;

        // Solution domain
        private (float min, float max)[] solutionDomain;

        // Function for evaluating a solution
        private Func<IList<float>, float> evaluate;

        // Function for comparing two solutions based on their evaluation
        private Func<float, float, bool> compare;

        // Function for generation initial solutions
        private Func<IList<float>> initialSolution;

        // Worst evaluation possible
        private float WorstEvaluationPossible => sign * float.PositiveInfinity;

        // Sign multiplier
        private int sign;

        // Initial temperature for simulated annealing
        private float t0;

        // Temperature decrease rate
        private float r;

        // Problem dimensions
        private int numDims;

        // Number of evaluations to perform for each solution
        private int evalsPerSolution;

        // Random number generator
        private Random random;

        // Initial deltas
        private float[] deltas;

        // Minimum deltas
        private float[] minDeltas;

        // Parameter modifiers
        private float[] modif;

        // Number of runs to perform per optimization
        private int runs;

        /// <summary>TODO</summary>
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
        public HillClimber(
            Func<IList<float>, float> evaluate,
            IList<(float min, float max)> solutionDomain,
            bool minimize = true,
            int maxSteps = int.MaxValue,
            float criteria = float.PositiveInfinity,
            float epsilon = float.NegativeInfinity,
            Func<IList<float>> initialSolution = null,
            IList<float> deltas = null,
            IList<float> minDeltas = null,
            int runs = 1,
            int evalsPerSolution = 1,
            float t0 = 0f,
            float r = 0.1f,
            float accel = 1f,
            int? seed = null)
        {
            // Check if one of maxSteps, criteria or epsilon was specified
            if (maxSteps == int.MaxValue
                && criteria == float.PositiveInfinity
                && epsilon == float.NegativeInfinity)
            {
                throw new ArgumentException(string.Format(
                    "One of '{0}', '{1}' or '{2}' must be specified.",
                    nameof(maxSteps), nameof(criteria), nameof(epsilon)));
            }

            // Do not allow a null evaluation function
            if (evaluate == null)
                throw new ArgumentNullException(
                    $"The '{nameof(evaluate)}' parameter cannot be null.");

            // Keep evaluation function
            this.evaluate = evaluate;

            // Do not allow a null solution domain
            if (solutionDomain == null)
                throw new ArgumentNullException(
                    $"The '{nameof(solutionDomain)}' parameter cannot be null.");

            // Check if solution domain is valid
            for (int i = 0; i < solutionDomain.Count; i++)
            {
                if (!(solutionDomain[i].min < solutionDomain[i].max))
                {
                    throw new ArgumentException(string.Format(
                        "Error in solution domain for parameter {0}: "
                        + "{1} is not less than {2}",
                        i, solutionDomain[i].min, solutionDomain[i].max));
                }
            }

            // Keep solution domain
            this.solutionDomain =
                new (float min, float max)[solutionDomain.Count];
            solutionDomain.CopyTo(this.solutionDomain, 0);

            // Determine number of dimensions in this problem
            numDims = solutionDomain.Count;

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

            // Keep required parameters
            this.maxSteps = maxSteps;
            this.criteria = criteria;
            this.epsilon = epsilon;
            this.accel = accel;
            this.t0 = t0;
            this.r = r;
            this.runs = runs;
            this.evalsPerSolution = evalsPerSolution;

            // Initialize random number generator
            random = seed.HasValue ? new Random(seed.Value) : new Random();

            // Initialize vector to keep initial deltas
            this.deltas = new float[numDims];

            // Were initial deltas specified in the parameters?
            if (deltas is null)
                // If not, initialize them all to 1
                for (int i = 0; i < numDims; i++) this.deltas[i] = 1f;
            else
                // Otherwise keep a local copy
                deltas.CopyTo(this.deltas, 0);

            // Initialize minimum deltas to zeros (minimum change hill climber
            // can apply to each parameter)
            this.minDeltas = new float[numDims];

            // If minimum deltas were specified, use them instead
            if (!(minDeltas is null)) minDeltas.CopyTo(this.minDeltas, 0);

            // Determine modifications to perform to each parameter
            // Depends whether there is an acceleration factor or not
            modif = accel > 1.0f
                ? new float[] { -accel, -1f / accel, 0, 1f / accel, accel }
                : new float[] { -1, 0, 1 };

            // If initial solution function is null, create one which will
            // spawn uniformly random solutions within the solution domain
            if (initialSolution is null)
            {
                this.initialSolution = () =>
                {
                    float[] initSol = new float[numDims];
                    for (int i = 0; i < numDims; i++)
                    {
                        float paramRange =
                            solutionDomain[i].max - solutionDomain[i].min;
                        initSol[i] = (float)random.NextDouble() * paramRange
                            + solutionDomain[i].min;
                    }
                    return initSol;
                };
            }
            else
            {
                this.initialSolution = initialSolution;
            }
        }

        /// <summary>
        /// Optimize / search for a good solution.
        /// </summary>
        /// <returns>
        /// The best solution found, its evaluation and number of evaluations
        /// required to find it.
        /// </returns>
        public (IList<float>, float, int) Optimize()
        {
            // Best evaluation so far in all finished runs
            float bestEvalAllRuns = WorstEvaluationPossible;
            // Best evaluation so far in current run
            float bestEvalInRun = WorstEvaluationPossible;
            // Current (last) evaluation
            float currentEval = WorstEvaluationPossible;

            // Best solution so far in all finished runs
            IList<float> bestSolutionAllRuns = null;
            // Best solution so far in current run
            IList<float> bestSolutionInRun = null;
            // Current (last) solution
            IList<float> currentSolution;

            // Number of evaluations performed
            int numEvals = 0;

            // Create an array of possibly dynamic deltas, i.e., maximum
            // changes in each parameter
            float[] localDeltas = new float[numDims];

            // Perform algorithm runs
            for (int run = 0; run < runs; run++)
            {
                // Reset temperature for simulated annealing to initial value
                float t = t0;

                // Set previous evaluation to worst possible
                float prevEval = WorstEvaluationPossible;

                // Reset deltas
                deltas.CopyTo(localDeltas, 0);

                // For current run, get an initial solution and evaluate it
                currentSolution = initialSolution();
                bestSolutionInRun = currentSolution;
                currentEval = Evaluate(
                    currentSolution, evalsPerSolution, ref numEvals);

                // Perform a run of the algorithm
                for (int step = 0;
                    step < maxSteps
                    && compare(criteria, currentEval)
                    && sign * (prevEval - currentEval) > epsilon;
                    step++)
                {
                    // Loop through each parameter
                    for (int i = 0; i < localDeltas.Length; i++)
                    {
                        // Best modif index so far
                        int bestModifIdx = -1;
                        float bestEval = WorstEvaluationPossible;

                        // Apply modifications to current parameter
                        for (int j = 0; j < modif.Length; j++)
                        {
                            // Temporary evaluation
                            float tempEval = 0;

                            // Determine current modification
                            float currentModif = localDeltas[i] * modif[j];
                            float newValue = currentSolution[i] + currentModif;

                            // Is current modif outside the solution domain?
                            if (newValue < solutionDomain[i].min
                                ||
                                newValue > solutionDomain[i].max)
                            {
                                // if so, penalize it
                                tempEval = WorstEvaluationPossible;
                            }
                            else
                            {
                                // Otherwise apply current modification,
                                // evaluate, and reset parameter
                                currentSolution[i] += currentModif;

                                // Evaluate current solution
                                tempEval = Evaluate(
                                    currentSolution, evalsPerSolution,
                                        ref numEvals);

                                // Add annealing term?
                                if (t0 > 0) tempEval += t *
                                    (float)(random.NextDouble()
                                        - random.NextDouble());

                                // Reset parameter
                                currentSolution[i] -= currentModif;
                            }

                            // Is the temporary evaluation the best one?
                            if (compare(tempEval, bestEval))
                            {
                                bestModifIdx = j;
                                bestEval = tempEval;
                            }
                        }

                        // Was no modification the best modification?
                        if (modif[bestModifIdx] == 0.0f)
                        {
                            // Deaccelerate search wrt this parameter
                            localDeltas[i] /= accel;
                        }
                        else
                        {
                            // Keep best modification
                            currentSolution[i] +=
                                localDeltas[i] * modif[bestModifIdx];

                            // Update acceleration correspondingly
                            localDeltas[i] *= Math.Abs(modif[bestModifIdx]);
                        }

                        // Don't let deltas go below defined minimum
                        localDeltas[i] =
                            Math.Max(localDeltas[i], minDeltas[i]);

                        // Notify listeners current paramter is optimized for
                        // current step
                        PerParam?.Invoke(
                            i, currentSolution, bestEval, numEvals);
                    }

                    // Update annealing temperature, if necessary
                    if (t0 > 0) t = t0 * (float)Math.Exp(-r * step);

                    // Keep previous evaluation
                    prevEval = currentEval;

                    // Evaluate current solution
                    currentEval = Evaluate(
                        currentSolution, evalsPerSolution, ref numEvals);

                    // If it's the best in run, keep solution
                    if (compare(currentEval, bestEvalInRun))
                    {
                        bestSolutionInRun = currentSolution;
                        bestEvalInRun = currentEval;
                        BestInRunUpdate?.Invoke(
                            step, bestSolutionInRun, bestEvalInRun, numEvals);
                    }

                    // Notify listeners current step is over
                    PerStep?.Invoke(
                        step, currentSolution, currentEval, numEvals);
                }

                // Is last run's best solution better than the best solution
                // found so far in all runs?
                if (compare(bestEvalInRun, bestEvalAllRuns)
                    || bestSolutionAllRuns == null)
                {
                    bestSolutionAllRuns = bestSolutionInRun;
                    bestEvalAllRuns = bestEvalInRun;
                    BestAllRunsUpdate?.Invoke(
                        run, bestSolutionAllRuns, bestEvalAllRuns, numEvals);
                }

                // Notify listeners current run is over
                PerRun?.Invoke(
                    run, bestSolutionInRun, bestEvalInRun, numEvals);
            }

            // Return best solution found and its evaluation
            return (bestSolutionAllRuns, bestEvalAllRuns, numEvals);
        }

        // Evaluate a solution a number of times, return the average
        private float Evaluate(
            IList<float> solution, int evalsPerSolution, ref int evaluations)
        {
            float eval = 0;
            for (int i = 0; i < evalsPerSolution; i++)
            {
                evaluations++;
                eval += evaluate(solution);
            }
            return eval / evalsPerSolution;
        }

        public event Action<int, IList<float>, float, int> PerParam;
        public event Action<int, IList<float>, float, int> PerStep;
        public event Action<int, IList<float>, float, int> PerRun;
        public event Action<int, IList<float>, float, int> BestInRunUpdate;
        public event Action<int, IList<float>, float, int> BestAllRunsUpdate;
    }
}
