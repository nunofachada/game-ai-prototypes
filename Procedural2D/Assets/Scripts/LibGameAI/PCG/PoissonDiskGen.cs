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
        private readonly (float min, float max) separation;
        private readonly (float min, float max) radius;

        private readonly (float width, float height) dims;

        private readonly bool toroidal;
        private readonly Random random;
        private readonly float gridDetail;

        public PoissonDiskGen(
            int maxTries,
            (float min, float max) separation,
            (float min, float max) radius,
            (float width, float height) dims,
            bool toroidal = false,
            Random random = null,
            float gridDetail = 1)
        {
            this.maxTries = maxTries;
            this.separation = separation;
            this.radius = radius;
            this.dims = dims;
            this.toroidal = toroidal;
            this.random = random ?? new Random();
            this.gridDetail = gridDetail;
        }


        public PoissonDiskGen(
            int maxTries,
            (float min, float max) separation,
            (float min, float max) radius,
            int seed,
            (float width, float height) dims,
            bool toroidal = false,
            float gridDetail = 1)
            : this(
                maxTries,
                separation,
                radius,
                dims,
                toroidal,
                new Random(seed),
                gridDetail)
        { }


        public IEnumerable<(float x, float y)> DiskPoints((float x, float y, float r) disk, float minSep = 0)
        {
            IEnumerable<(int x, int y)> iDiskPoints =
                Bresenham.GetFilledCircle(
                   (MMath.Round(disk.x), MMath.Round(disk.y)),
                   MMath.Round(disk.r + minSep));

            int width = MMath.Round(dims.width);
            int height = MMath.Round(dims.height);

            iDiskPoints = toroidal
                ? iDiskPoints.Select(pt => (Grid.Wrap(pt.x, width).pos, Grid.Wrap(pt.y, height).pos))
                : iDiskPoints.Where(pt => pt.x >= 0 && pt.x < width && pt.y >= 0 && pt.y < height);

            return iDiskPoints.Select(pf => ((float)pf.x, (float)pf.y));
        }

        public IEnumerable<(float x, float y, float r)> GenerateDisks()
        {
            return GenerateDisks((random.NextFloat() * dims.width, random.NextFloat() * dims.height, (separation.min + separation.max) / 2f));
        }

        public IEnumerable<(float x, float y, float r)> GenerateDisks((float x, float y, float r) initial)
        {
            RandomizedSet<(float x, float y, float r)> active = new(MMath.Round(random.Next()));
            GridOccupancy placedGrid = new(dims.width, dims.height, gridDetail);
            List<(float x, float y, float r)> placed = new();

            // Get "pixels" for the initial disk
            IEnumerable<(float x, float y)> disk = DiskPoints(initial, separation.min);

            // Place disk in collision grid
            if (!placedGrid.TryPlace(disk))
                throw new InvalidOperationException(
                    $"Unable to place initial disk {disk}");

            // Add the initial disk to the placed disks list
            placed.Add(initial);

            // Add the initial disk to the active list
            active.Add(initial);

            float[] anglesToTry = new float[maxTries];
            for (int i = 0; i < maxTries; i++)
            {
                anglesToTry[i] = (float)i / maxTries * 2 * MathF.PI;
            }

            while (active.Count > 0)
            {
                (float x, float y, float r) current = active.GetRandom();
                bool placedSuccessfully = false;

                anglesToTry.Shuffle(random);

                foreach (float angle in anglesToTry)
                {
                    float nextRadius = random.Range(radius.min, radius.max);
                    float sep = current.r + nextRadius + random.Range(separation.min, separation.max);

                    (float x, float y, float r) newDisk = (
                        current.x + sep * MathF.Cos(angle),
                        current.y + sep * MathF.Sin(angle),
                        nextRadius);

                    if (placedGrid.TryPlace(DiskPoints(newDisk, separation.min)))
                    {
                        active.Add(newDisk);
                        placed.Add(newDisk);
                        placedSuccessfully = true;
                        break;
                    }
                }

                if (!placedSuccessfully)
                {
                    active.Remove(current);
                }
            }

            return placed;
        }
    }
}