/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using System;

namespace LibGameAI.ProcGen
{
    public static class Landscape
    {
        public static void FaultModifier(
            float[,] landscape, float depth, Func<float> randFloat,
            float decreaseDistance = 0)
        {
            // Create random fault epicentre and direction vector
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

        // TODO Not working properly
        // Check https://github.com/creativitRy/Erosion
        // and "Fast Hydraulic Erosion Simulation and Visualization on GPU" by Xing Mei, Philippe Decaudin, Bao-Gang Hu
        // "Fast Hydraulic and Thermal Erosion on the GPU" by Balazs Jako
        public static void ThermalErosion(float[,] landscape, float threshold)
        {
            // This should be updated to use System.ReadOnlySpan when
            // Unity supports .NET Standard 2.1 in order to avoid heap
            // allocations
            (int, int)[] neighbors =
                new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

            // Create a copy of the landscape
            float[,] landscapeCopy =
                new float[landscape.GetLength(0), landscape.GetLength(1)];
            Array.Copy(landscape, landscapeCopy,
                landscape.GetLength(0) * landscape.GetLength(1));

            // Apply erosion
            for (int x = 1; x < landscape.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < landscape.GetLength(1) - 1; y++)
                {
                    float height = landscapeCopy[x, y];
                    float limit = height - threshold;

                    foreach ((int x, int y) d in neighbors)
                    {
                        int nx = x + d.x;
                        int ny = y + d.y;
                        float nHeight = landscapeCopy[nx, ny];

                        // Is the neighbor below the threshold?
                        if (nHeight < limit)
                        {
                            // Some of the height moves, from 0 to 1/4 of the
                            // threshold, depending on the height difference
                            float delta = (limit - nHeight) / threshold;
                            if (delta > 2) delta = 2;
                            float change = delta * threshold / 8;

                            // Write new height to original landscape
                            landscape[x, y] = -change;
                            landscape[nx, ny] += change;
                        }
                    }
                }
            }
        }
    }
}
