/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace LibGameAI.PCG
{
    public class CA2D
    {

        public enum Rule
        {
            Smooth44,
            Smooth45,
            Majority38,
            Majority39,
            Majority40,
            Majority41,
            Majority42,
            Majority43,
            MajR2N13,
            WalledCities,
            Diamoeba,
            Coral,
            HighLife,
            CavesR1N5,
            CavesR2N13,
            GameOfLife,
            Serviettes,
            Flakes,
        }

        public static void RandomFill(int[] map, int[] values, float[] probabilities, Func<float> nextFloat)
        {
            if (values.Length != probabilities.Length)
                throw new InvalidOperationException(
                    $"'{nameof(values)}' and '{nameof(probabilities)}' have different lengths!");

            float[] ncProbs = NormCumulProbs(probabilities);

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


        // TODO: This could go to Utils
        private static float[] NormCumulProbs(float[] probabilities)
        {
            float totProb = 0.0f;
            float cumulProb = 0.0f;
            float[] ncProbs = new float[probabilities.Length];

            for (int i = 0; i < probabilities.Length; i++)
            {
                totProb += probabilities[i];
            }

            for (int i = 0; i < ncProbs.Length; i++)
            {
                cumulProb += probabilities[i] / totProb;
                ncProbs[i] = cumulProb;
            }

            return ncProbs;
        }

        public static void DoStep(int[] map_in, int[] map_out, int width, int height, bool toroidal, int nonToroidalBorderCells, Rule rule)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    if (rule == Rule.Smooth44)
                    {
                        // 45678/5678
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 1, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        // if (numNeighs > 4) map_out[i * width + j] = 1;
                        // else if (numNeighs < 4) map_out[i * width + j] = 0;
                        // else map_out[i * width + j] = map_in[i * width + j];

                        if (map_in[i * width + j] == 1 && numNeighs >= 4)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else if (map_in[i * width + j] == 0 && numNeighs >= 5)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else map_out[i * width + j] = 0;

                    }
                    else if (rule == Rule.Smooth45)
                    {
                        // 5678/5678
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 1, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        // if (numNeighs > 4) map_out[i * width + j] = 1;
                        // else map_out[i * width + j] = 0;

                        if (map_in[i * width + j] == 1 && numNeighs >= 5)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else if (map_in[i * width + j] == 0 && numNeighs >= 5)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else map_out[i * width + j] = 0;
                    }
                    else if (rule == Rule.Majority38)
                    {
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 4, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (numNeighs >= 38) map_out[i * width + j] = 1;
                        else map_out[i * width + j] = 0;

                    }
                    else if (rule == Rule.Majority39)
                    {
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 4, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (numNeighs >= 39) map_out[i * width + j] = 1;
                        else map_out[i * width + j] = 0;

                    }
                    else if (rule == Rule.Majority40)
                    {
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 4, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (numNeighs >= 40) map_out[i * width + j] = 1;
                        else map_out[i * width + j] = 0;

                    }
                    else if (rule == Rule.Majority41)
                    {
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 4, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (numNeighs >= 41) map_out[i * width + j] = 1;
                        else map_out[i * width + j] = 0;

                    }
                    else if (rule == Rule.Majority42)
                    {
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 4, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (numNeighs >= 42) map_out[i * width + j] = 1;
                        else map_out[i * width + j] = 0;

                    }
                    else if (rule == Rule.Majority43)
                    {
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 4, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (numNeighs >= 43) map_out[i * width + j] = 1;
                        else map_out[i * width + j] = 0;

                    }
                    else if (rule == Rule.MajR2N13)
                    {
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 2, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        map_out[i * width + j] = numNeighs >= 13 ? 1 : 0;
                    }
                    else if (rule == Rule.WalledCities)
                    {
                        // 2345/45678
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 1, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (map_in[i * width + j] == 1 && numNeighs >= 2 && numNeighs <= 5)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else if (map_in[i * width + j] == 0 && numNeighs >= 4 && numNeighs <= 8)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else map_out[i * width + j] = 0;
                    }
                    else if (rule == Rule.Diamoeba)
                    {
                        // 5678/35678
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 1, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (map_in[i * width + j] == 1 && numNeighs >= 5 && numNeighs <= 8)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else if (map_in[i * width + j] == 0 && numNeighs >= 3 && numNeighs <= 8 && numNeighs != 4)
                        //else if (map_in[i * width + j] == 0 && (numNeighs >= 5 && numNeighs <= 8 || numNeighs == 3)) // Like ARR had
                        {
                            map_out[i * width + j] = 1;
                        }
                        else map_out[i * width + j] = 0;

                    }
                    else if (rule == Rule.Coral)
                    {
                        // 45678/3
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 1, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (map_in[i * width + j] == 1 && numNeighs >= 4 && numNeighs <= 8)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else if (map_in[i * width + j] == 0 && numNeighs == 3)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else map_out[i * width + j] = 0;

                    }
                    else if (rule == Rule.HighLife)
                    {
                        // 23/36
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 1, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (map_in[i * width + j] == 1 && numNeighs >= 2 && numNeighs <= 3)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else if (map_in[i * width + j] == 0 && (numNeighs == 3 || numNeighs == 6))
                        {
                            map_out[i * width + j] = 1;
                        }
                        else map_out[i * width + j] = 0;


                    }
                    else if (rule == Rule.CavesR1N5)
                    {
                        // 45678/5678
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 1, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (map_in[i * width + j] == 1 && numNeighs >= 4)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else if (map_in[i * width + j] == 0 && numNeighs >= 5)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else
                        {
                            map_out[i * width + j] = 0;
                        }
                    }
                    else if (rule == Rule.CavesR2N13)
                    {
                        // 12..Nmax/13..Nmax
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 2, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (map_in[i * width + j] == 1 && numNeighs >= 12)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else if (map_in[i * width + j] == 0 && numNeighs >= 13)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else
                        {
                            map_out[i * width + j] = 0;
                        }
                    }
                    else if (rule == Rule.GameOfLife)
                    {
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 1, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        // GoL rules 23/3
                        if (map_in[i * width + j] == 1 && numNeighs >= 2 && numNeighs <= 3)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else if (map_in[i * width + j] == 0 && numNeighs == 3)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else
                        {
                            map_out[i * width + j] = 0;
                        }

                    }
                    else if (rule == Rule.Serviettes)
                    {
                        // /234
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 1, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (map_in[i * width + j] == 1)
                        {
                            map_out[i * width + j] = 0;
                        }
                        else if (map_in[i * width + j] == 0 && numNeighs >= 2 && numNeighs <= 4)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else map_out[i * width + j] = 0;

                    }
                    else if (rule == Rule.Flakes)
                    {
                        // 012345678/3
                        int numNeighs = CountNeighbors(map_in, width, height, i, j, 1, toroidal: toroidal, nonToroidalBorderCells :  nonToroidalBorderCells);

                        if (map_in[i * width + j] == 1 && map_in[i * width + j] < 9)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else if (map_in[i * width + j] == 0 && numNeighs == 3)
                        {
                            map_out[i * width + j] = 1;
                        }
                        else map_out[i * width + j] = 0;

                    }

                }
            }
        }

        public static int CountNeighbors(int[] map, int width, int height, int row, int col, int radius,
            bool toroidal = true, int nonToroidalBorderCells = 0, int neighValue = 1, bool countSelf = false,
            NeighType neighType = NeighType.Moore)
        {
            int numNeighs = 0;

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    if (neighType == NeighType.VonNeumann && Math.Abs(i) + Math.Abs(j) < radius)
                    {
                        continue;
                    }

                    if (!countSelf && i == 0 && j ==0)
                    {
                        continue;
                    }

                    int r = Wrap(row + i, height, out bool wrapRow);
                    int c = Wrap(col + j, width, out bool wrapCol);

                    if (!toroidal && (wrapRow || wrapCol))
                    {
                        if (nonToroidalBorderCells == neighValue)
                        {
                            numNeighs++;
                        }
                    }
                    else if (map[r * width + c] == neighValue)
                    {
                        numNeighs++;
                    }
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