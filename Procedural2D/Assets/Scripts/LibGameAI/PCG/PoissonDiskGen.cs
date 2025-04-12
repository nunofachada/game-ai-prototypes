/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;
using System.Linq;
using LibGameAI.Geometry;
using LibGameAI.Util;

namespace LibGameAI.PCG
{
    public class PoissonDiskGen
    {
        private readonly int maxTries;
        private readonly float separation;
        private readonly (float width, float height) dims;
        private readonly bool toroidal;
        private readonly Random random;
        private readonly float gridDetail;

        public PoissonDiskGen(
            int maxTries,
            float separation,
            (float width, float height) dims,
            bool toroidal = false,
            Random random = null,
            float gridDetail = 1)
        {
            this.maxTries = maxTries;
            this.separation = separation;
            this.dims = dims;
            this.toroidal = toroidal;
            this.random = random ?? new Random();
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
                new Random(seed),
                gridDetail)
        { }

        private float NextFloat() => (float)random.NextDouble();

        public GridOccupancy GenerateDisks()
        {
            return GenerateDisks((NextFloat() * dims.width, NextFloat() * dims.height, separation * 2));
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

            RandomizedSet<(float x, float y, float r)> active = new(MMath.Round(random.Next()));
            GridOccupancy placedGrid = new(dims.width, dims.height, gridDetail);

            IEnumerable<(float x, float y)> disk = DiskPoints(initial);

            if (!placedGrid.TryPlace(disk))
                throw new InvalidOperationException(
                    $"Unable to place initial disk {disk}");

            active.Add(initial);

            // Use the same radius for now
            float currentRadius = initial.r;

            while (active.Count > 0)
            {
                (float x, float y, float r) current = active.GetRandom();
                bool placed = false;

                for (int i = 0; i < maxTries; i++)
                {
                    float angle = (float)i / maxTries * 2 * MathF.PI;
                    float r = 2 * currentRadius + separation * NextFloat();

                    (float x, float y, float r) newDisk = (
                        current.x + r * MathF.Cos(angle),
                        current.y + r * MathF.Sin(angle),
                        currentRadius);

                    if (placedGrid.TryPlace(DiskPoints(newDisk)))
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

            return placedGrid;
        }

    }
}