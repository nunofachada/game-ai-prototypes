using System.Collections.Generic;
using UnityEngine;
using LibGameAI.Geometry;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    /// <summary>
    /// Poisson Disks scenario, draws actual disks using the Midpoint Circle
    /// Algorithm (i.e. Bresenham's Circle) for smoothness.
    /// </summary>
    public class PoissonDiskScenario : StochasticScenario
    {
        [SerializeField]
        private int maxTries = 6;

        [SerializeField]
        private float separation = 1;

        public readonly struct Disk
        {
            public float X { get; }
            public float Y { get; }
            public float Radius { get; }

            public Disk(float x, float y, float radius)
            {
                X = x;
                Y = y;
                Radius = radius;
            }
        }

        private IEnumerable<Disk> GenerateDisks(int xDim, int yDim)
        {
            yield return new Disk(xDim / 2, yDim / 2, 10);
            yield return new Disk(xDim / 2 + 10, yDim / 2 + 20, 5);
            yield return new Disk(xDim / 2 + 20, yDim / 2 - 15, 7);
            yield return new Disk(xDim / 2 + 10, yDim / 2 - 12, 2);
            yield return new Disk(xDim / 2 + 12, yDim / 2 - 7, 1);
            yield return new Disk(xDim / 2 - 20, yDim / 2 + 9, 3);
            yield return new Disk(xDim / 2 - 100, yDim / 2 - 80, 30);

            // yield return new Disk(0, 0, 10);
            // yield return new Disk(10, 20, 5);
            // yield return new Disk(20, -15, 7);
            // yield return new Disk(10, -12, 2);
            // yield return new Disk(12, -7, 1);
            // yield return new Disk(-20, 9, 3);
            // yield return new Disk(-100, -80, 30);
        }

        // private IEnumerable<Disk> GenerateDisks(Disk initial)
        // {
        //     Queue<Disk> active = new Queue<Disk>();
        //     List<Disk> placed = new List<Disk>();

        //     active.Enqueue(initial);
        //     placed.Add(initial);

        //     // Use the same radius for now
        //     float radius = initial.Radius;

        //     while (active.Count > 0)
        //     {
        //         Disk current = active.Peek();

        //         for (int i = 0; i < maxTries; i++)
        //         {
        //             float angle = i / maxTries * 2 * Mathf.PI;
        //             float r = 2 * radius + separation * (float)PRNG.NextDouble();

        //             Disk newDisk = new Disk(
        //                 current.X + r * Mathf.Cos(angle),
        //                 current.Y + r * Mathf.Sin(angle),
        //                 radius);
        //         }
        //     }
        // }

        public override void Generate(Color[] pixels, int xDim, int yDim)
        {
            base.Generate(pixels, xDim, yDim);

            Fill(pixels, Color.white);

            foreach (Disk disk in GenerateDisks(xDim, yDim))
            {
                IEnumerable<(int x, int y)> points = Bresenham.GetFilledCircle(((int)disk.X, (int)disk.Y), (int)disk.Radius, (xDim, yDim));
                foreach ((int x, int y) in points)
                {
                    pixels[y * xDim + x] = Color.black;
                }
            }

        }

    //     private void FillCircleLines(Color[] pixels, int width, int height, int cx, int cy, int x, int y)
    //     {
    //         DrawHorizontalLine(pixels, width, height, cx - x, cx + x, cy + y);
    //         DrawHorizontalLine(pixels, width, height, cx - x, cx + x, cy - y);
    //         DrawHorizontalLine(pixels, width, height, cx - y, cx + y, cy + x);
    //         DrawHorizontalLine(pixels, width, height, cx - y, cx + y, cy - x);
    //     }

    //     private void DrawHorizontalLine(Color[] pixels, int width, int height, int xStart, int xEnd, int y)
    //     {
    //         if (y < 0 || y >= height) return;

    //         xStart = Mathf.Clamp(xStart, 0, width - 1);
    //         xEnd = Mathf.Clamp(xEnd, 0, width - 1);

    //         for (int x = xStart; x <= xEnd; x++)
    //         {
    //             int index = y * width + x;
    //             pixels[index] = Color.black;
    //         }
    //     }

    }
}
