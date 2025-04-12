/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using UnityEngine;
using LibGameAI.PCG;
using LibGameAI.Util;
using System.Linq;
using NaughtyAttributes;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    /// <summary>
    /// Poisson Disks scenario, draws actual disks using the Midpoint Circle
    /// Algorithm (i.e. Bresenham's Circle) for smoothness.
    /// </summary>
    public class PoissonDiskScenario : StochasticScenario
    {
        private enum InitialDisk { Random, Center }

        [SerializeField]
        private InitialDisk initialDisk = InitialDisk.Random;

        [SerializeField]
        private int maxTries = 6;

        [SerializeField]
        [MinMaxSlider(0.0f, 250.0f)]
        private Vector2 separation = new(0, 10f);

        [SerializeField]
        [MinMaxSlider(0.0f, 100.0f)]
        private Vector2 radius = new(0, 3f);

        [SerializeField]
        private bool toroidal = false;

        public override void Generate(Color[] pixels, int xDim, int yDim)
        {
            base.Generate(pixels, xDim, yDim);

            Fill(pixels, Color.white);

            PoissonDiskGen diskGen = new(
                maxTries,
                (separation.x, separation.y),
                (radius.x, radius.y),
                (xDim, yDim),
                toroidal,
                PRNG);

            float initRadius = PRNG.Range(radius.x, radius.y);

            (float x, float y, float r) initial =
                initialDisk == InitialDisk.Random
                ? (xDim * (float)PRNG.NextDouble(), yDim * (float)PRNG.NextDouble(), initRadius)
                : (xDim / 2f, yDim / 2f, initRadius);

            foreach ((float x, float y, float r) disk in diskGen.GenerateDisks(initial))
            {
                IEnumerable<(int px, int py)> diskPoints = diskGen
                    .DiskPoints(disk)
                    .Select(fxy => (MMath.Round(fxy.x), MMath.Round(fxy.y)));

                foreach ((int px, int py) in diskPoints)
                {
                    pixels[py * xDim + px] = Color.black;
                }
            }
        }
    }

}
