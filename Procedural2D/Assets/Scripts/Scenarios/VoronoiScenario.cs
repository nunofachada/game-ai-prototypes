/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExamples.Procedural2D.Scenarios
{
    public class VoronoiScenario : StochasticScenario
    {
        private enum Distance { Euclidean, Manhattan }

        [SerializeField]
        private int maxAreas = 6;

        [SerializeField]
        private bool toroidal = true;

        [SerializeField]
        private Distance distanceType = Distance.Euclidean;

        [SerializeField]
        private List<Color> areaTypes = new List<Color>() {
            Color.blue, Color.green, Color.red, Color.yellow
        };

        public override void Generate(Color[] pixels, int width, int height)
        {
            // Euclidean distance
            double EuclideanDistance((int r, int c) a, (int r, int c) b)
            {
                int dr = Math.Abs(a.r - b.r);
                int dc = Math.Abs(a.c - b.c);

                if (toroidal)
                {
                    if (dr > height / 2.0)
                        dr = height - dr;
                    if (dc > width / 2.0)
                        dc = width - dc;
                }

                return Math.Sqrt(dr * dr + dc * dc);
            }

            // Manhattan distance
            double ManhattanDistance((int r, int c) a, (int r, int c) b)
            {
                int dr = Math.Abs(a.r - b.r);
                int dc = Math.Abs(a.c - b.c);

                if (toroidal)
                {
                    if (dr > height / 2)
                        dr = height - dr;
                    if (dc > width / 2)
                        dc = width - dc;
                }

                return dr + dc;
            }

            base.Generate(pixels, width, height);

            Func<(int, int), (int, int), double> distFunc = distanceType switch
            {
                Distance.Euclidean => EuclideanDistance,
                Distance.Manhattan => ManhattanDistance,
                _ => throw new InvalidOperationException("Unknown distance type")
            };

            IList<(int, int)> centers;

            ISet<(int, int)> unvisited = new HashSet<(int, int)>();

            IDictionary<(int, int), int> visited = new Dictionary<(int, int), int>();

            // Will not allow more areas than we have room for in the image
            if (maxAreas > width * height)
                maxAreas = width * height;

            // In the beginning, we only have unvisited tiles
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    unvisited.Add((i, j));

            // Determine area centers
            while (visited.Count < maxAreas)
            {
                // Find a random center
                (int, int) tilePos = (PRNG.Next(height), PRNG.Next(width));

                // Only allow non-repeated centers
                if (unvisited.Contains(tilePos))
                {
                    unvisited.Remove(tilePos);
                    visited.Add(tilePos, PRNG.Next(areaTypes.Count));
                }
            }

            centers = new List<(int, int)>(visited.Keys);

            // Assign each point to the closest area center
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // Only do this if the current point is not already a center
                    if (unvisited.Contains((i, j)))
                    {
                        double minDist = double.PositiveInfinity;
                        int closestCenter = int.MaxValue;

                        // Determine closest center
                        foreach ((int r, int c) point in centers)
                        {
                            double dist = distFunc((i, j), point);

                            if (dist < minDist)
                            {
                                minDist = dist;
                                closestCenter = visited[point];
                            }
                        }

                        // Remove current point from unvisited list, add it to
                        // visited collection, specifying the closest center
                        unvisited.Remove((i, j));
                        visited.Add((i, j), closestCenter);
                    }

                    // Color current point according to closest center
                    pixels[i * width + j] = areaTypes[visited[(i, j)]];
                }
            }
        }
    }
}