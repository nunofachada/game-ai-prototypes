/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGameAI.Geometry;
using LibGameAI.Util;

namespace LibGameAI.PCG
{
    public class PoissonDiskGen
    {
        // private class PlacedGrid
        // {
        //     private readonly bool[] grid;
        //     private readonly float detail;
        //     private readonly int gridWidth, gridHeight;
        //     private readonly List<(float x, float y, float r)> placed;
        //     private readonly bool toroidal;

        //     public IEnumerable<(float x, float y, float r)> Placed => placed;

        //     public int Count => placed.Count;


        //     public PlacedGrid(float width, float height, float detail, bool toroidal = false)
        //     {

        //         this.detail = detail;
        //         this.toroidal = toroidal;
        //         gridWidth = MMath.Round(width * detail);
        //         gridHeight = MMath.Round(height * detail);
        //         grid = new bool[gridWidth * gridHeight];
        //         placed = new List<(float x, float y, float r)>();
        //     }

        //     public bool TryPlace(float x, float y, float r)
        //     {
        //         IEnumerable<(int x, int y)> cellsToCheck =
        //             Bresenham.GetFilledCircle(
        //                 (MMath.Round(x * detail), MMath.Round(y * detail)),
        //                 MMath.Round(r * detail),
        //                 (gridWidth, gridHeight),
        //                 toroidal
        //             );

        //         //UnityEngine.Debug.Log($"Putting {cellsToCheck.Count()} cells for ({x}, {y}, {r})!");

        //         // Check if any cells are occupied
        //         foreach ((int cx, int cy) in cellsToCheck)
        //         {
        //             // If so, don't add anything and return false
        //             if (grid[cy * gridWidth + cx])
        //             {
        //                 UnityEngine.Debug.Log($"   => Unable to place in grid: ({cx}, {cy}) is true ({grid[cy * gridWidth + cx]})!");
        //                 return false;
        //             }
        //         }

        //         // If we get here, it means there's space to place a new disk
        //         foreach ((int cx, int cy) in cellsToCheck)
        //         {
        //             UnityEngine.Debug.Log($"   => Set grid ({cx}, {cy}) to True!!!!");
        //             grid[cy * gridWidth + cx] = true;
        //         }

        //         placed.Add((x, y, r));
        //         UnityEngine.Debug.Log($" --- Added {(x, y, r)}");
        //         return true;
        //     }

        //     public override string ToString()
        //     {
        //         StringBuilder sb = new();
        //         List<StringBuilder> output = new();
        //         for (int i = 0; i < grid.Length; i++)
        //         {
        //             if (i % gridWidth == 0 && i > 0)
        //             {
        //                 output.Add(sb);
        //                 sb = new StringBuilder();
        //             }
        //             sb.Append(grid[i] ? "1" : 0);
        //         }
        //         output.Add(sb);
        //         output.Reverse();
        //         sb = new StringBuilder();
        //         sb.AppendJoin('\n', output);
        //         return sb.ToString();
        //     }

        // }

        private readonly int maxTries;
        private readonly float separation;
        private readonly (float width, float height) dims;
        private readonly bool toroidal;
        private readonly Func<float> nextFloat;
        private readonly float gridDetail;



        public PoissonDiskGen(
            int maxTries,
            float separation,
            (float width, float height) dims,
            bool toroidal = false,
            Func<float> nextFloat = null,
            float gridDetail = 1)
        {
            this.maxTries = maxTries;
            this.separation = separation;
            this.dims = dims;
            this.toroidal = toroidal;
            this.nextFloat = nextFloat ?? (() => (float)new Random().NextDouble());
            this.gridDetail = gridDetail;
        }


        public PoissonDiskGen(
            int maxTries,
            float separation,
            int seed,
            (float width, float height) dims,
            bool toroidal = false,
            float gridDetail = 1)
            : this(
                maxTries,
                separation,
                dims,
                toroidal,
                () => (float)new Random(seed).NextDouble(),
                gridDetail)
        { }


        public GridOccupancy GenerateDisks()
        {
            return GenerateDisks((nextFloat.Invoke() * dims.width, nextFloat() * dims.height, separation * 2));
        }

        public GridOccupancy GenerateDisks((float x, float y, float r) initial)
        {

            IEnumerable<(float x, float y)> DiskPoints((float x, float y, float r) disk)
            {
                return Bresenham
                    .GetFilledCircle(
                        (MMath.Round(disk.x), MMath.Round(disk.y)),
                        MMath.Round(disk.r),
                        (MMath.Round(dims.width), MMath.Round(dims.height)),
                        toroidal)
                    .Select(pf => ((float)pf.x, (float)pf.y));
            }

            RandomizedSet<(float x, float y, float r)> active = new(MMath.Round(nextFloat.Invoke() * int.MaxValue));
            GridOccupancy placedGrid = new(dims.width, dims.height, gridDetail);


            IEnumerable<(float x, float y)> disk = DiskPoints(initial);

            if (!placedGrid.TryPlace(disk))
                throw new InvalidOperationException(
                    $"Unable to place initial disk {disk}");

            active.Add(initial);

            // Use the same radius for now
            float currentRadius = initial.r;

            while (active.Count > 0) //  && active.Count < 1230
            {
                //UnityEngine.Debug.Log($"Active count = {active.Count}");
                (float x, float y, float r) current = active.GetRandom();
                bool placed = false;

                for (int i = 0; i < maxTries; i++)
                {
                    float angle = (float)i / maxTries * 2 * MathF.PI;
                    float r = 2 * currentRadius + separation * nextFloat.Invoke();

                    (float x, float y, float r) newDisk = (
                        current.x + r * MathF.Cos(angle),
                        current.y + r * MathF.Sin(angle),
                        currentRadius);

                    if (placedGrid.TryPlace(DiskPoints(newDisk)))
                    {
                        //UnityEngine.Debug.Log($"Placed {newDisk} [{MMath.Round(newDisk.x)}, {MMath.Round(newDisk.y)}, {MMath.Round(newDisk.r)}]");
                        active.Add(newDisk);
                        placed = true;
                        break;
                    }
                    else
                    {
                        //UnityEngine.Debug.Log($"  Position unavailable for {newDisk}");
                    }
                }

                if (!placed)
                {
                    active.Remove(current);
                    //UnityEngine.Debug.Log($"Unable to placed around {current}, active count is {active.Count}");
                }
            }

            //UnityEngine.Debug.Log("\n" + placedGrid.ToString());

            return placedGrid;
        }

    }
}