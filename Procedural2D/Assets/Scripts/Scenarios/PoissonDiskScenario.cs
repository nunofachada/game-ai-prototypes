using System.Collections.Generic;
using UnityEngine;
using LibGameAI.Geometry;
using LibGameAI.PCG;
using LibGameAI.Util;
using System.Linq;
using System;

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
        private int maxTries = 6;

        [SerializeField]
        private float separation = 0.2f;

        [SerializeField]
        private float radius = 1;

        [SerializeField]
        private bool toroidal = false;

        [SerializeField]
        private float gridDetail = 1;

        [SerializeField]
        private InitialDisk initialDisk = InitialDisk.Random;



        public override void Generate(Color[] pixels, int xDim, int yDim)
        {
            base.Generate(pixels, xDim, yDim);

            Fill(pixels, Color.white);

            PoissonDiskGen diskGen = new(
                maxTries,
                separation,
                (xDim, yDim),
                toroidal,
                PRNG,
                gridDetail);

            (float x, float y, float r) initial =
                initialDisk == InitialDisk.Random
                ? (xDim * (float)PRNG.NextDouble(), yDim * (float)PRNG.NextDouble(), radius)
                : (xDim / 2f, yDim / 2f, radius);

            GridOccupancy go = diskGen.GenerateDisks(initial);

            for (int py = 0; py < yDim; py++)
            {
                for (int px = 0; px < xDim; px++)
                {
                    if (go[px, py])
                        pixels[py * xDim + px] = Color.gray;
                }
            }

        }

    }
}
