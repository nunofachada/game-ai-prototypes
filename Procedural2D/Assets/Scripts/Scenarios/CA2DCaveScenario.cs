/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using NaughtyAttributes;
namespace AIUnityExamples.Procedural2D.Scenarios
{
    public class CA2DCaveScenario : StochasticScenario
    {
        private enum CellType { Rock, Floor }

        [SerializeField]
        private int rockThreshold = 5;

        [SerializeField]
        [Range(1, 10)]
        private int neighSize = 1;

        [SerializeField]
        [Range(0, 1)]
        private float initRocks = 0.5f;

        [SerializeField]
        private int steps = 5;

        [SerializeField]
        private bool drawRockOutline = true;

        [SerializeField]
        private bool toroidal = true;

        [SerializeField] [DisableIf(nameof(toroidal))]
        private CellType nonToroidalBorderCells;

        private readonly Color ROCK = Color.white;
        private readonly Color FLOOR = Color.grey;
        private readonly Color BORDER = Color.red;

        // TODO Move CA2D generic functionality to libGameAI
        public override void Generate(Color[] pixels, int width, int height)
        {
            base.Generate(pixels, width, height);

            Color[] aux;
            Color[] buf1 = new Color[pixels.Length];
            Color[] buf2 = new Color[pixels.Length];

            // Randomly place rocks in the scenario
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // Put rock or floor, randomly
                    buf1[i * width + j] =
                        PRNG.NextDouble() < initRocks ? ROCK : FLOOR;
                }
            }

            // Run cellular automata
            for (int step = 0; step < steps; step++)
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        // How many rocks around here?
                        int numRocks = CountRocks(buf1, width, height, i, j, neighSize);

                        // Put rock or floor, randomly
                        buf2[i * width + j] =
                            numRocks >= rockThreshold ? ROCK : FLOOR;
                    }
                }

                // Swap buffers
                aux = buf1;
                buf1 = buf2;
                buf2 = aux;
            }

            // Post-process border rocks for visual effect
            if (drawRockOutline)
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (buf1[i * width + j] == ROCK)
                        {
                            // How many rocks around here?
                            int numRocks = CountRocks(buf1, width, height, i, j, 1);

                            if (numRocks < 9)
                            {
                                // Put border rock
                                buf1[i * width + j] = BORDER;
                            }
                        }
                    }
                }
            }

            // Copy buf1 data to pixels
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = buf1[i];
        }

        private int CountRocks(Color[] pixels, int width, int height, int row, int col, int nSize)
        {
            int numRocks = 0;

            for (int i = -nSize; i <= nSize; i++)
            {
                for (int j = -nSize; j <= nSize; j++)
                {
                    bool wrapRow, wrapCol;
                    int r = Wrap(row + i, height, out wrapRow);
                    int c = Wrap(col + j, width, out wrapCol);

                    if (!toroidal && (wrapRow || wrapCol))
                    {
                        if (nonToroidalBorderCells == CellType.Rock)
                        {
                            numRocks++;
                        }
                    }
                    else if (pixels[r * width + c] != FLOOR)
                    {
                        numRocks++;
                    }
                }
            }
            return numRocks;
        }

        private int Wrap(int pos, int max, out bool wrap)
        {
            wrap = false;
            if (pos < 0)
            {
                pos = max + pos;
                wrap = true;
            }
            else if (pos >= max)
            {
                pos = pos - max;
                wrap = true;
            }
            return pos;
        }
    }
}