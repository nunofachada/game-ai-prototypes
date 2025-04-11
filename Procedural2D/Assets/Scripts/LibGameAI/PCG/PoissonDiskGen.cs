/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;
using LibGameAI.Geometry;
using LibGameAI.Util;

namespace LibGameAI.PCG
{
    public class PoissonDiskGen
    {
        private class PlacedGrid
        {
            private readonly bool[] grid;
            private readonly float detail;
            private readonly int gridWidth, gridHeight;
            private readonly List<(float x, float y, float r)> placed;
            private readonly bool toroidal;

            public IEnumerable<(float x, float y, float r)> Placed => placed;

            public PlacedGrid(float width, float height, float detail, bool toroidal = false)
            {

                this.detail = detail;
                this.toroidal = toroidal;
                gridWidth = (int)(width * detail);
                gridHeight = (int)(height * detail);
                grid = new bool[gridWidth * gridHeight];
                placed = new List<(float x, float y, float r)>();
            }

            public bool TryPlace(float x, float y, float r)
            {
                IEnumerable<(int x, int y)> cellsToCheck =
                    Bresenham.GetFilledCircle(
                        ((int)(x * detail), (int)(y * detail)),
                        (int)(r * detail),
                        (gridWidth, gridHeight),
                        toroidal
                    );

                // Check if any cells are occupied
                foreach ((int cx, int cy) in cellsToCheck)
                {
                    // If so, don't add anything and return false
                    if (grid[cy * gridWidth + cx])
                        return false;
                }

                // If we get here, it means there's space to place a new disk
                foreach ((int cx, int cy) in cellsToCheck)
                {
                    grid[cy * gridWidth + cx] = true;
                }

                placed.Add((x, y, r));
                return true;
            }

        }

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


        public IEnumerable<(float x, float y, float r)> GenerateDisks()
        {
            return GenerateDisks((nextFloat() * dims.width, nextFloat() * dims.height, separation * 2));
        }

        public IEnumerable<(float x, float y, float r)> GenerateDisks((float x, float y, float r) initial)
        {
            RandomizedSet<(float x, float y, float r)> active = new();
            PlacedGrid placedGrid = new(dims.width, dims.height, gridDetail);

            active.Add(initial);
            placedGrid.TryPlace(initial.x, initial.y, initial.r);

            // Use the same radius for now
            float currentRadius = initial.r;

            while (active.Count > 0)
            {
                (float x, float y, float r) current = active.GetRandom();
                bool placed = false;

                for (int i = 0; i < maxTries; i++)
                {
                    float angle = i / maxTries * 2 * MathF.PI;
                    float r = 2 * currentRadius + separation * nextFloat.Invoke();

                    (float x, float y, float r) newDisk = (
                        current.x + r * MathF.Cos(angle),
                        current.y + r * MathF.Sin(angle),
                        currentRadius);

                    if (placedGrid.TryPlace(newDisk.x, newDisk.y, newDisk.r))
                    {
                        active.Add(newDisk);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    active.Remove(current);
                }
            }

            return placedGrid.Placed;
        }

    }
}