using System.Collections.Generic;
using UnityEngine;
using LibGameAI.Geometry;
using LibGameAI.PCG;

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
                () => (float)PRNG.NextDouble(),
                gridDetail);

            (float x, float y, float r) initial =
                initialDisk == InitialDisk.Random
                ? (xDim * (float)PRNG.NextDouble(), yDim * (float)PRNG.NextDouble(), radius)
                : (xDim / 2f, yDim / 2f, radius);

            foreach ((float x, float y, float r) disk in diskGen.GenerateDisks(initial))
            {
                IEnumerable<(int x, int y)> points = Bresenham.GetFilledCircle(((int)disk.x, (int)disk.y), (int)disk.r, (xDim, yDim), true);
                foreach ((int x, int y) in points)
                {
                    pixels[y * xDim + x] = Color.black;
                }
            }

        }

    }
}
