/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using LibGameAI.PCG;
using NaughtyAttributes;
using System;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    public class CA2DCaveScenario : StochasticScenario
    {
        private enum CellType { Floor = 0, Rock = 1, Wall = 2 }

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

        [SerializeField]
        [DisableIf(nameof(toroidal))]
        [Dropdown(nameof(allowedToroidalCells))]
        private CellType nonToroidalBorderCells;

        private readonly CellType[] allowedToroidalCells =
            new CellType[] { CellType.Floor, CellType.Rock };

        private readonly Color ROCK = Color.white;
        private readonly Color FLOOR = Color.grey;
        private readonly Color WALL = Color.red;

        public override void Generate(Color[] pixels, int xDim, int yDim)
        {
            base.Generate(pixels, xDim, yDim);

            // Define cellular automata (CA) Caves rule
            CA2DBinaryRule rule = new CA2DBinaryRule(
                $"M,{neighSize}/{rockThreshold - 1}-/{rockThreshold}-");

            // Setup CA with specified rule and parameters
            CA2D ca = new CA2D(
                rule, xDim, yDim, toroidal,
                nonToroidalBorderCells == CellType.Rock ? 1 : 0);

            // Initialize CA
            ca.InitRandom(
                // Initial types of cell
                new int[] { (int)CellType.Floor, (int)CellType.Rock },
                // Percentage of initial types of cell
                new float[] { 1 - initRocks, initRocks },
                // Method for generating random numbers
                () => (float)PRNG.NextDouble());

            // Run CA for the specified number of steps
            for (int i = 0; i < steps; i++)
            {
                ca.DoStep();
            }

            // Convert CA to image colors and post-process border rocks for
            // walls visual effect
            for (int y = 0; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    Color pixelColor;

                    if (ca[x, y] == (int)CellType.Floor)
                    {
                        pixelColor = FLOOR;
                    }
                    else // We assume it's Rock
                    {
                        pixelColor = ROCK;

                        if (drawRockOutline)
                        {
                            // How much rocks around here?
                            int numRocks = ca.CountNeighbors(
                                x, y, 1, neighValue: (int)CellType.Rock);

                            if (numRocks < 8)
                            {
                                // Put rock wall
                                pixelColor = WALL;
                            }
                        }
                    }
                    pixels[y * xDim + x] = pixelColor;
                }
            }

        }
    }
}