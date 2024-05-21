/* Copyright (c) 2018-2024 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using LibGameAI.Util;

namespace LibGameAI.PCG
{
    public static class Landscape
    {
        public static void FaultModifier(
            float[,] landscape, float depth, Func<float> randFloat,
            float decreaseDistance = 0)
        {
            // Create random fault epicenter and direction vector
            float cx = randFloat.Invoke() * landscape.GetLength(0);
            float cy = randFloat.Invoke() * landscape.GetLength(1);
            float direction = randFloat.Invoke() * 2 * (float)Math.PI;
            float dx = (float)Math.Cos(direction);
            float dy = (float)Math.Sin(direction);

            // Apply the fault
            for (int x = 0; x < landscape.GetLength(0); x++)
            {
                for (int y = 0; y < landscape.GetLength(1); y++)
                {
                    // Get the dot product of the location with the fault
                    float ox = cx - x;
                    float oy = cy - y;
                    float dp = ox * dx + oy * dy;
                    float change;

                    // Positive dot product goes up, negative goes down
                    if (dp > 0)
                    {
                        // Fault size will decrease with distance if
                        // decreaseDistance > 0
                        float decrease = decreaseDistance > 0
                            ? decreaseDistance / (decreaseDistance + dp)
                            : 1;
                        // Positive dot product goes up
                        change = depth * decrease;
                    }
                    else
                    {
                        // Fault size will decrease with distance if
                        // decreaseDistance > 0
                        float decrease = decreaseDistance > 0
                            ? decreaseDistance / (decreaseDistance - dp)
                            : 1;
                        // Negative dot product goes down
                        change = -depth * decrease;
                    }

                    // Apply fault modification
                    landscape[x, y] += change;
                }
            }
        }

        // Diamond-Square algorithm
        // Seems to be working, needs verification / testing
        public static void DiamondSquare(float[,] landscape,
            float maxInitHeight, float roughness, Func<float> randFloat)
        {
            int tileSide;
            float randness = maxInitHeight;
            int vSide = 1;
            int height = landscape.GetLength(0);
            int width = landscape.GetLength(1);
            int largerSize = Math.Max(height, width);

            Func<float> rand = () => 2 * (randFloat() - 0.5f);

            // Get the smallest 2^n + 1 value larger than the largest size of
            // the landscape
            for (int n = 1; vSide < largerSize; n++)
            {
                vSide <<= 1;
            }
            vSide += 1;

            // Flatten height map
            for (int i = 0; i < landscape.GetLength(0); i++)
            {
                for (int j = 0; j < landscape.GetLength(1); j++)
                {
                    landscape[i, j] = 0f;
                }
            }

            // Set virtual corners of the landscape to a random value within the
            // specified limit
            landscape[0, 0] = randFloat() * randness;
            int horizNeigh = Grid.Wrap(vSide - 1, width).pos;
            int vertNeigh = Grid.Wrap(vSide - 1, height).pos;

            landscape[0, horizNeigh] = landscape[0, 0];
            landscape[vertNeigh, 0] = landscape[0, 0];
            landscape[vertNeigh, horizNeigh] = landscape[0, 0];

            tileSide = vSide - 1;

            // UnityEngine.Debug.Log($"({height},{width})");

            while (tileSide > 1)
            {
                int halfSide = tileSide / 2;
                randness *= roughness;

                // Diamond step
                for (int r = 0; r < vSide; r += tileSide)
                {
                    for (int c = 0; c < vSide; c += tileSide)
                    {
                        int rWrap = Grid.Wrap(r, height).pos;
                        int cWrap = Grid.Wrap(c, width).pos;
                        int rTSWrap = Grid.Wrap(r + tileSide, height).pos;
                        int cTSWrap = Grid.Wrap(c + tileSide, width).pos;
                        int rHSWrap = Grid.Wrap(r + halfSide, height).pos;
                        int cHSWrap = Grid.Wrap(c + halfSide, width).pos;
                        // UnityEngine.Debug.Log($"({rWrap},{cWrap}) -- ({rTSWrap},{cWrap}) -- ({rWrap,{cTSWrap}) -- ({rTSWrap},{cTSWrap})");

                        float cornerSum =
                            landscape[rWrap, cWrap]
                            + landscape[rTSWrap, cWrap]
                            + landscape[rWrap, cTSWrap]
                            + landscape[rTSWrap, cTSWrap];

                        landscape[rHSWrap, cHSWrap] = cornerSum / 4f + rand() * randness;
                    }
                }

                // Square step
                for (int r = 0; r < vSide; r += halfSide)
                {
                    for (int c = (r + halfSide) % tileSide; c < vSide; c += tileSide)
                    {
                        int rWrap = Grid.Wrap(r, height).pos;
                        int cWrap = Grid.Wrap(c, width).pos;

                        int rHSPlusWrap = Grid.Wrap((r - halfSide + vSide - 1) % (vSide - 1), height).pos;
                        int rHSMinusWrap = Grid.Wrap((r + halfSide) % (vSide - 1), height).pos;
                        int cHSPlusWrap = Grid.Wrap((c + halfSide) % (vSide - 1), width).pos;
                        int cHSMinusWrap = Grid.Wrap((c - halfSide + vSide - 1) % (vSide - 1), width).pos;


                        float sideSum =
                            landscape[rHSPlusWrap, cWrap]
                            + landscape[rHSMinusWrap, cWrap]
                            + landscape[rWrap, cHSPlusWrap]
                            + landscape[rWrap, cHSMinusWrap];

                        landscape[rWrap, cWrap] = sideSum / 4f + rand() * randness;

                        if (r == 0)
                        {
                            int rVSWrap = Grid.Wrap(vSide - 1, height).pos;
                            landscape[rVSWrap, cWrap] = landscape[r, cWrap];
                        }
                        if (c == 0)
                        {
                            int cVSWrap = Grid.Wrap(vSide - 1, width).pos;
                            landscape[rWrap, cVSWrap] = landscape[rWrap, c];
                        }
                    }
                }

                tileSide /= 2;
            }
        }

        // Per Bak sandpile model
        public static void Sandpile(float[,] landscape, float threshold,
            float increment, float decrement, float grainDropDensity,
            bool staticDrop, bool stochastic, Func<int, int> randInt,
            Func<double> randDouble,
            (int x, int y)[] neighs = null)
        {
            void Drop(int x, int y, float inc)
            {
                float inThresh = stochastic
                    ? (float)((Stats.NextNormalDouble(randDouble) + threshold) * (increment / threshold))
                    : threshold;

                landscape[x, y] += inc;
                if (landscape[x, y] >= inThresh)
                {
                    float inDec = stochastic
                        ? (float)((Stats.NextNormalDouble(randDouble) + decrement) * (increment / threshold))
                        : decrement;

                    landscape[x, y] -= inDec;
                    float slip = inDec / neighs.Length;
                    foreach ((int x, int y) neigh in neighs)
                    {
                        int nx = x + neigh.x;
                        int ny = y + neigh.y;
                        if (nx < 0 || nx >= landscape.GetLength(0) ||
                            ny < 0 || ny >= landscape.GetLength(1))
                        {
                            continue;
                        }
                        Drop(x + neigh.x, y + neigh.y, slip);
                    }
                }
            }

            int xDrop = randInt(landscape.GetLength(0));
            int yDrop = randInt(landscape.GetLength(1));
            int totalGrains = (int)(grainDropDensity * landscape.GetLength(0) * landscape.GetLength(1));
            if (neighs is null)
                neighs = new (int x, int y)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
            for (int i = 0; i < totalGrains; i++)
            {
                Drop(xDrop, yDrop, increment);

                if (!staticDrop)
                {
                    xDrop = randInt(landscape.GetLength(0));
                    yDrop = randInt(landscape.GetLength(1));
                }
            }

        }

        // Simple Thermal Erosion
        public static void ThermalErosion(float[,] landscape, float threshold, int iterations, bool toroidal)
        {
            int xDim = landscape.GetLength(0);
            int yDim = landscape.GetLength(1);

            // Von Neumann neighborhood
            (int, int)[] neighbors =
                new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

            // Create a copy of the landscape
            float[,] landscapeCopy = new float[xDim, yDim];
            Array.Copy(landscape, landscapeCopy, xDim * yDim);


            // Apply erosion
            for (int i = 0; i < iterations; i++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    for (int y = 0; y < yDim; y++)
                    {
                        float height = landscape[x, y];
                        float limit = height - threshold;

                        foreach ((int x, int y) d in neighbors)
                        {

                            int nx = x + d.x;
                            int ny = y + d.y;

                            if (toroidal)
                            {
                                nx = Grid.Wrap(nx, xDim).pos;
                                ny = Grid.Wrap(ny, yDim).pos;
                            }
                            else if (nx < 0 || ny < 0 || nx >= xDim || ny >= yDim)
                            {
                                continue;
                            }

                            float nHeight = landscape[nx, ny];

                            // Is the neighbor below the threshold?
                            if (nHeight < limit)
                            {
                                // Some of the height moves, from 0 to 1/4 of the
                                // threshold, depending on the height difference
                                float delta = (limit - nHeight) / threshold;
                                if (delta > 2) delta = 2;
                                float change = delta * threshold / 4;

                                // Write new height to original landscape
                                landscapeCopy[x, y] -= change;
                                landscapeCopy[nx, ny] += change;
                            }
                        }
                    }
                }
                Array.Copy(landscapeCopy, landscape, xDim * yDim);
                (landscapeCopy, landscape) = (landscape, landscapeCopy);
            }

        }
    }
}
