/* Copyright (c) 2018-2024 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using LibGameAI.Util;

namespace LibGameAI.PCG
{
    /// <summary>
    /// A 2D discrete cellular automaton (CA).
    /// </summary>
    /// <remarks>
    /// Instances of this class represent a cellular automaton, but the class
    /// also provides CA-related functionality via static methods for those
    /// interested in directly handling the CA state.
    /// </remarks>
    public class CA2D
    {
        // Grids supporting the CA lattice
        private int[] gridCurrent, gridNext;

        // The transition rule governing this CA
        private readonly ICA2DRule rule;

        // Is the CA initialized?
        private bool initialized;

        /// <summary>
        /// Width of the CA.
        /// </summary>
        public int XDim { get; }

        /// <summary>
        /// Height of the CA.
        /// </summary>
        public int YDim { get; }

        /// <summary>
        /// Is the CA toroidal?
        /// </summary>
        public bool Toroidal { get; }

        /// <summary>
        /// What is the value of off-grid cells in case the CA is not toroidal?
        /// </summary>
        public int NonToroidalOffGridCells { get; }

        /// <summary>
        /// A read-only indexer returning the value of the CA cell at the
        /// specified indexes.
        /// </summary>
        /// <remarks>
        /// Internally, rows are kept in sequence (row-major order). Therefore,
        /// when iterating, use `y` for the outer loop and `x` for the inner
        /// loop.
        /// </remarks>
        /// <param name="x">Horizontal position of the cell.</param>
        /// <param name="y">Vertical position of the cell.</param>
        /// <returns>Value of the CA cell at the specified indexes.</returns>
        public int this[int x, int y] => gridCurrent[y * XDim + x];

        /// <summary>
        /// A read-only indexer returning the value of the CA cell at the
        /// specified index in row-major order.
        /// </summary>
        /// <param name="i">Position of the cell in row-major order.</param>
        /// <returns>Value of the CA cell at the specified index.</returns>
        public int this[int i] => gridCurrent[i];

        /// <summary>
        /// Create a new 2D discrete cellular automaton.
        /// </summary>
        /// <param name="rule">Transition rule governing the CA.</param>
        /// <param name="xDim">Width of the CA.</param>
        /// <param name="yDim">Height of the CA.</param>
        /// <param name="toroidal">Is the CA toroidal?</param>
        /// <param name="nonToroidalBorderCells">
        /// The value of off-grid cells in case the CA is not toroidal.
        /// </param>
        public CA2D(ICA2DRule rule, int xDim, int yDim, bool toroidal, int nonToroidalBorderCells = 0)
        {
            this.rule = rule;
            initialized = false;
            gridCurrent = new int[xDim * yDim];
            gridNext = new int[xDim * yDim];
            XDim = xDim;
            YDim = yDim;
            Toroidal = toroidal;
            NonToroidalOffGridCells = nonToroidalBorderCells;
        }

        /// <summary>
        /// Perform a step in the CA evolution.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Exception thrown if the CA is not initialized.
        /// </exception>
        public void DoStep()
        {
            // CA must be initialized
            if (!initialized)
            {
                throw new InvalidOperationException($"{nameof(CA2D)} instance is not yet initialized.");
            }

            // Apply transition rule to each cell
            for (int y = 0; y < YDim; y++)
            {
                for (int x = 0; x < XDim; x++)
                {
                    gridNext[y * XDim + x] = rule.ProcessRule(this, x, y);
                }
            }

            // Swap buffers
            (gridNext, gridCurrent) = (gridCurrent, gridNext);
        }

        /// <summary>
        /// Count neighbors with a specific value around a given cell.
        /// </summary>
        /// <param name="x">Horizontal position of the cell.</param>
        /// <param name="y">Vertical position of the cell.</param>
        /// <param name="radius">Neighborhood radius to search.</param>
        /// <param name="neighType">Neighborhood type.</param>
        /// <param name="neighValue">Count neighbors with this value.</param>
        /// <param name="countSelf">Count self?</param>
        /// <returns>
        /// Number of neighbors with a specific value around a given cell.
        /// </returns>
        public int CountNeighbors(int x, int y, int radius,
            Neighborhood neighType = Neighborhood.Moore, int neighValue = 1, bool countSelf = false)
        {
            // Delegate the operation to the static method
            return CountNeighbors(gridCurrent, XDim, YDim, x, y, Toroidal,
                NonToroidalOffGridCells, radius, neighValue, countSelf, neighType);
        }

        /// <summary>
        /// Randomly initialize the CA using the system's random number generator.
        /// </summary>
        /// <param name="values">
        /// Possible values with which to initialize the cells.
        /// </param>
        /// <param name="probabilities">
        /// Probabilities of each value being assigned. If not given, each value
        /// will be assigned with equal probability.
        /// </param>
        /// <param name="seed">
        /// Seed for the random number generator. If not given, generator will be
        /// randomly initialized.
        /// </param>
        public void InitRandom(int[] values, float[] probabilities = null, int? seed = null)
        {
            Random rnd = seed.HasValue ? new Random(seed.Value) : new Random();
            if (probabilities == null)
            {
                probabilities = new float[values.Length];
                Array.Fill(probabilities, 1.0f / values.Length);
            }
            InitRandom(values, probabilities, () => (float)rnd.NextDouble());
        }

        /// <summary>
        /// Randomly initialize the CA with an external random number generator.
        /// </summary>
        /// <param name="values">
        /// Possible values with which to initialize the cells.
        /// </param>
        /// <param name="probabilities">
        /// Probabilities of each value being assigned. If not given, each value
        /// will be assigned with equal probability.
        /// </param>
        /// <param name="nextFloat">
        /// Delegate which will return a random float between 0 and 1.
        /// </param>
        public void InitRandom(int[] values, float[] probabilities, Func<float> nextFloat)
        {
            RandomFill(gridCurrent, values, probabilities, nextFloat);
            initialized = true;
        }

        /// <summary>
        /// Initialize the CA with the exact state given.
        /// </summary>
        /// <param name="initialState">
        /// The state with which to initialize the CA.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Exception thrown if the size of the given initial state is different
        /// from the size of the CA grid.
        /// </exception>
        public void InitExact(int[,] initialState)
        {
            if (initialState.GetLength(0) != XDim || initialState.GetLength(1) != YDim)
            {
                throw new ArgumentException(
                    "Size of given initial state is different from size of CA grid:"
                    + $" ({initialState.GetLength(0)}, {initialState.GetLength(1) != XDim} != ({XDim}, {YDim})");
            }
            for (int y = 0; y < YDim; y++)
            {
                for (int x = 0; x < XDim; x++)
                {
                    gridCurrent[y * XDim + x] = initialState[x, y];
                }
            }
            initialized = true;
        }

        /// <summary>
        /// Initialize the CA with the exact state given.
        /// </summary>
        /// <param name="initialState">
        /// The state with which to initialize the CA in row-major order.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Exception thrown if the size of given initial state is different
        /// from the size of the CA grid.
        /// </exception>
        public void InitExact(int[] initialState)
        {
            if (initialState.Length != gridCurrent.Length)
            {
                throw new ArgumentException(
                    "Size of given initial state is different from size of CA grid"
                    + $" {initialState.Length} != {gridCurrent.Length}");
            }
            Array.Copy(initialState, gridCurrent, initialState.Length);
            initialized = true;
        }

        /// <summary>
        /// Initialize the CA using a delegate.
        /// </summary>
        /// <param name="initializer">
        /// Delegate used to initialize the CA, which should accept the `x` and
        /// `y` cell position and return its value.
        /// </param>
        public void InitFunc(Func<int, int, int> initializer)
        {
            for (int y = 0; y < YDim; y++)
            {
                for (int x = 0; x < XDim; x++)
                {
                    gridCurrent[y * XDim + x] = initializer.Invoke(x, y);
                }
            }
            initialized = true;
        }

        /// <summary>
        /// Randomly fill the <paramref name="map"/> with the given
        /// <paramref name="values"/> with the specified
        /// <paramref name="probabilities"/> using a delegate method for
        /// random number generation.
        /// </summary>
        /// <param name="map">Integer array to fill.</param>
        /// <param name="values">Values with which to fill the array.</param>
        /// <param name="probabilities">
        /// Probabilities associated with each value.
        /// </param>
        /// <param name="nextFloat">
        /// Delegate which will return a random float between 0 and 1.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <paramref name="values"/> and <paramref name="probabilities"/>
        /// have different lengths.
        /// </exception>
        public static void RandomFill(int[] map, int[] values, float[] probabilities, Func<float> nextFloat)
        {
            // Values and probabilities need to have the same size
            if (values.Length != probabilities.Length)
                throw new InvalidOperationException(
                    $"'{nameof(values)}' and '{nameof(probabilities)}' have different lengths!");

            // Obtain cumulative probabilities
            float[] ncProbs = MMath.CumSum(probabilities);

            // Initialize int array
            for (int i = 0; i < map.Length; i++)
            {
                float prob = nextFloat?.Invoke() ?? 1.0f;
                for (int j = 0; j < ncProbs.Length; j++)
                {
                    if (prob <= ncProbs[j])
                    {
                        map[i] = values[j];
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Count neighbors with a specific value around a given cell.
        /// </summary>
        /// <param name="map">
        /// An integer array representing a grid in row-major format.
        /// </param>
        /// <param name="xDim">Grid width.</param>
        /// <param name="yDim">Grid height.</param>
        /// <param name="xCell">Horizontal cell position.</param>
        /// <param name="yCell">Vertical cell position.</param>
        /// <param name="toroidal">Consider the grid toroidal?</param>
        /// <param name="nonToroidalOffGridCells">
        /// Value of the off-grid cells in case the grid is not toroidal.
        /// </param>
        /// <param name="radius">Neighborhood radius.</param>
        /// <param name="neighValue">Value of neighbors to count.</param>
        /// <param name="countSelf">Count central cell?</param>
        /// <param name="neighType">Neighborhood type.</param>
        /// <returns>
        /// The number of neighbors with a specific value around a given cell.
        /// </returns>
        public static int CountNeighbors(int[] map, int xDim, int yDim, int xCell, int yCell,
            bool toroidal = true, int nonToroidalOffGridCells = 0, int radius = 1,
            int neighValue = 1, bool countSelf = false, Neighborhood neighType = Neighborhood.Moore)
        {
            int numNeighs = 0;

            foreach ((int x, int y) in neighType.GetNeighborhood(radius))
            {
                // Don't count self if countSelf is false
                if (!countSelf && x == 0 && y == 0) continue;

                (int yNeigh, bool yWrap) = Grid.Wrap(yCell + y, yDim);
                (int xNeigh, bool xWrap) = Grid.Wrap(xCell + x, xDim);

                if (!toroidal && (yWrap || xWrap))
                {
                    if (nonToroidalOffGridCells == neighValue)
                    {
                        numNeighs++;
                    }
                }
                else if (map[yNeigh * xDim + xNeigh] == neighValue)
                {
                    numNeighs++;
                }
            }
            return numNeighs;
        }
    }
}