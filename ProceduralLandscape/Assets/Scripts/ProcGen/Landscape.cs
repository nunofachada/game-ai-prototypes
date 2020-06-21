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
            float[,] landscape, float depth, Func<float> randFloat)
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

                    // Positive dot product goes up, negative goes down
                    float change = dp > 0 ? depth : -depth;

                    // Apply fault modification
                    landscape[x, y] += change;
                }
            }
        }

    }
}
