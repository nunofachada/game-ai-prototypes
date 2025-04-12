using System.Collections.Generic;
using UnityEngine;
using LibGameAI.Geometry;
using LibGameAI.PCG;
using LibGameAI.Util;
using System.Linq;
using System;
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
        private int maxTries = 6;

        [SerializeField]
        [MinMaxSlider(0.0f, 250.0f)]
        private Vector2 separation = new(0, 10f);

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
                (separation.x, separation.y),
                (xDim, yDim),
                toroidal,
                PRNG,
                gridDetail);

            (float x, float y, float r) initial =
                initialDisk == InitialDisk.Random
                ? (xDim * (float)PRNG.NextDouble(), yDim * (float)PRNG.NextDouble(), radius)
                : (xDim / 2f, yDim / 2f, radius);

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
