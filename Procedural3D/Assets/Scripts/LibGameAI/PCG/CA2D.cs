/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Diagnostics;

namespace LibGameAI.PCG
{
    public class CA2D
    {

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

        public static void DoStep(int[] map_in, int[] map_out, int width, int height)
        {
            int radius = 1;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int numNeighs = CountNeighbors(map_in, width, height, i, j, radius);

                    if (numNeighs > 4) map_out[i * width + j] = 1;
                    else if (numNeighs < 4) map_out[i * width + j] = 0;
                    else map_out[i * width + j] = map_in[i * width + j];
                }
            }
        }

        private static int CountNeighbors(int[] map, int width, int height, int row, int col, int radius,
            bool toroidal = true, int nonToroidalBorderCells = 0, int neighValue = 1,
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

                    int r = Wrap(row + i, height, out bool wrapRow);
                    int c = Wrap(col + j, width, out bool wrapCol);

                    if (map.Length <= r * width + c)
                    {
                        UnityEngine.Debug.Log(Wrap(-2, 10, out bool mywrap));
                        throw new IndexOutOfRangeException($"map length is {map.Length}, index is {r * width + c} (r={r}, c={c}, row={row}, col={col}, width={width}, height={height})");
                    }

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