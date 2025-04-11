using System;
using System.Collections.Generic;

using LibGameAI.Util;

namespace LibGameAI.PCG
{
    public class PoissonDiskGen
    {

        // private IEnumerable<Circle> GenerateDisks(int xDim, int yDim)
        // {
        //     yield return new Circle(xDim / 2, yDim / 2, 10);
        //     yield return new Circle(xDim / 2 + 10, yDim / 2 + 20, 5);
        //     yield return new Circle(xDim / 2 + 20, yDim / 2 - 15, 7);
        //     yield return new Circle(xDim / 2 + 10, yDim / 2 - 12, 2);
        //     yield return new Circle(xDim / 2 + 12, yDim / 2 - 7, 1);
        //     yield return new Circle(xDim / 2 - 20, yDim / 2 + 9, 3);
        //     yield return new Circle(xDim / 2 - 100, yDim / 2 - 80, 30);

        //     // yield return new Circle(0, 0, 10);
        //     // yield return new Circle(10, 20, 5);
        //     // yield return new Circle(20, -15, 7);
        //     // yield return new Circle(10, -12, 2);
        //     // yield return new Circle(12, -7, 1);
        //     // yield return new Circle(-20, 9, 3);
        //     // yield return new Circle(-100, -80, 30);
        // }

        // private IEnumerable<Circle> GenerateDisks(Circle initial)
        // {
        //     Queue<Circle> active = new Queue<Circle>();
        //     List<Circle> placed = new List<Circle>();

        //     active.Enqueue(initial);
        //     placed.Add(initial);

        //     // Use the same radius for now
        //     float radius = initial.Radius;

        //     while (active.Count > 0)
        //     {
        //         Circle current = active.Peek();

        //         for (int i = 0; i < maxTries; i++)
        //         {
        //             float angle = i / maxTries * 2 * MathF.PI;
        //             float r = 2 * radius + separation * (float)PRNG.NextDouble();

        //             Circle newDisk = new Circle(
        //                 current.X + r * MathF.Cos(angle),
        //                 current.Y + r * MathF.Sin(angle),
        //                 radius);
        //         }
        //     }
        // }

        // public override void Generate(Color[] pixels, int xDim, int yDim)
        // {
        //     base.Generate(pixels, xDim, yDim);

        //     Fill(pixels, Color.white);

        //     foreach (Disk disk in GenerateDisks(xDim, yDim))
        //     {
        //         int x = (int)disk.Radius;
        //         int y = 0;
        //         int decision = 1 - x;

        //         while (x >= y)
        //         {
        //             FillCircleLines(pixels, xDim, yDim, (int)disk.X, (int)disk.Y, x, y);
        //             y++;

        //             if (decision <= 0)
        //             {
        //                 decision += 2 * y + 1;
        //             }
        //             else
        //             {
        //                 x--;
        //                 decision += 2 * (y - x) + 1;
        //             }
        //         }
        //     }

        // }

        // private void FillCircleLines(Color[] pixels, int width, int height, int cx, int cy, int x, int y)
        // {
        //     DrawHorizontalLine(pixels, width, height, cx - x, cx + x, cy + y);
        //     DrawHorizontalLine(pixels, width, height, cx - x, cx + x, cy - y);
        //     DrawHorizontalLine(pixels, width, height, cx - y, cx + y, cy + x);
        //     DrawHorizontalLine(pixels, width, height, cx - y, cx + y, cy - x);
        // }

        // private void DrawHorizontalLine(Color[] pixels, int width, int height, int xStart, int xEnd, int y)
        // {
        //     if (y < 0 || y >= height) return;

        //     xStart = Mathf.Clamp(xStart, 0, width - 1);
        //     xEnd = Mathf.Clamp(xEnd, 0, width - 1);

        //     for (int x = xStart; x <= xEnd; x++)
        //     {
        //         int index = y * width + x;
        //         pixels[index] = Color.black;
        //     }
        // }
    }
}