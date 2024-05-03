/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using LibGameAI.Util;

namespace LibGameAI.PCG
{
    public class CA2D
    {
        private int[] gridCurrent, gridNext;
        private readonly ICA2DRule rule;
        private bool initialized;

        public int XDim { get; }
        public int YDim { get; }
        public bool Toroidal { get; }
        public int NonToroidalBorderCells { get; }

        public int this[int x, int y] => gridCurrent[y * XDim + x];

        public CA2D(ICA2DRule rule, int xDim, int yDim, bool toroidal, int nonToroidalBorderCells = 0)
        {
            this.rule = rule;
            initialized = false;
            gridCurrent = new int[xDim * yDim];
            gridNext = new int[xDim * yDim];
            XDim = xDim;
            YDim = yDim;
            Toroidal = toroidal;
            NonToroidalBorderCells = nonToroidalBorderCells;
        }

        public void DoStep()
        {
            if (!initialized)
            {
                throw new InvalidOperationException($"{nameof(CA2D)} instance is not yet initialized.");
            }

            for (int y = 0; y < YDim; y++)
            {
                for (int x = 0; x < XDim; x++)
                {
                    gridNext[y * XDim + x] = rule.ProcessRule(this, x, y);
                }
            }


            (gridNext, gridCurrent) = (gridCurrent, gridNext);
        }

        public int CountNeighbors(int x, int y, int radius,
            Neighborhood neighType = Neighborhood.Moore, int neighValue = 1, bool countSelf = false)
        {
            return CountNeighbors(gridCurrent, XDim, YDim, x, y, Toroidal, NonToroidalBorderCells, radius, neighValue, countSelf, neighType);
        }

        public void InitRandom(int[] values, int? seed = null, float[] probabilities = null)
        {
            Random rnd = seed.HasValue ? new Random(seed.Value) : new Random();
            if (probabilities == null)
            {
                probabilities = new float[values.Length];
                Array.Fill(probabilities, 1.0f / values.Length);
            }
            InitRandom(values, probabilities, () => (float)rnd.NextDouble());
        }

        public void InitRandom(int[] values, float[] probabilities, Func<float> nextFloat)
        {
            RandomFill(gridCurrent, values, probabilities, nextFloat);
            initialized = true;
        }


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

        public void InitExact(int[] initialState)
        {
            if (initialState.Length != gridCurrent.Length)
            {
                throw new ArgumentException(
                    "Size of given initial state is different from size of CA grid_"
                    + $" {initialState.Length} != {gridCurrent.Length}");
            }
            Array.Copy(initialState, gridCurrent, initialState.Length);
            initialized = true;
        }

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

        public static void RandomFill(CA2D ca, int[] values, float[] probabilities, Func<float> nextFloat)
        {
            RandomFill(ca.gridCurrent, values, probabilities, nextFloat);
        }

        public static void RandomFill(int[] map, int[] values, float[] probabilities, Func<float> nextFloat)
        {
            if (values.Length != probabilities.Length)
                throw new InvalidOperationException(
                    $"'{nameof(values)}' and '{nameof(probabilities)}' have different lengths!");

            float[] ncProbs = MMath.CumSum(probabilities);

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

        public static int CountNeighbors(int[] map, int xDim, int yDim, int xCell, int yCell,
            bool toroidal = true, int nonToroidalBorderCells = 0, int radius = 1,
            int neighValue = 1, bool countSelf = false, Neighborhood neighType = Neighborhood.Moore)
        {
            int numNeighs = 0;

            foreach ((int x, int y) in neighType.GetNeighborhood(radius, countSelf))
            {
                int yNeigh = Wrap(yCell + y, yDim, out bool yWrap);
                int xNeigh = Wrap(xCell + x, xDim, out bool xWrap);

                if (!toroidal && (yWrap || xWrap))
                {
                    if (nonToroidalBorderCells == neighValue)
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

        private static int Wrap(int pos, int max, out bool wrap)
        {
            wrap = false;
            if (pos < 0)
            {
                pos += max;
                wrap = true;
            }
            else if (pos >= max)
            {
                pos -= max;
                wrap = true;
            }
            return pos;
        }
    }
}