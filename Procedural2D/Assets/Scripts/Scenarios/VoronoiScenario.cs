/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAIPrototypes.Procedural2D.Scenarios
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

        public override void Generate(Color[] pixels, int xDim, int yDim)
        {
            // Euclidean distance
            float EuclideanDistance((int x, int y) a, (int x, int y) b)
            {
                int dx = Math.Abs(a.x - b.x);
                int dy = Math.Abs(a.y - b.y);

                if (toroidal)
                {
                    if (dx > xDim / 2.0)
                        dx = xDim - dx;
                    if (dy > yDim / 2.0)
                        dy = yDim - dy;
                }

                return MathF.Sqrt(dx * dx + dy * dy);
            }

            // Manhattan distance
            float ManhattanDistance((int x, int y) a, (int x, int y) b)
            {
                int dx = Math.Abs(a.x - b.x);
                int dy = Math.Abs(a.y - b.y);

                if (toroidal)
                {
                    if (dx > xDim / 2)
                        dx = xDim - dx;
                    if (dy > yDim / 2)
                        dy = yDim - dy;
                }

                return dx + dy;
            }

            base.Generate(pixels, xDim, yDim);

            Func<(int, int), (int, int), float> distFunc = distanceType switch
            {
                Distance.Euclidean => EuclideanDistance,
                Distance.Manhattan => ManhattanDistance,
                _ => throw new InvalidOperationException("Unknown distance type")
            };

            IList<(int, int)> centers;

            ISet<(int, int)> unvisited = new HashSet<(int, int)>();

            IDictionary<(int, int), int> visited = new Dictionary<(int, int), int>();

            // Will not allow more areas than we have room for in the image
            if (maxAreas > xDim * yDim)
                maxAreas = xDim * yDim;

            // In the beginning, we only have unvisited tiles
            for (int y = 0; y < yDim; y++)
                for (int x = 0; x < xDim; x++)
                    unvisited.Add((x, y));

            // Determine area centers
            while (visited.Count < maxAreas)
            {
                // Find a random center
                (int, int) tilePos = (PRNG.Next(xDim), PRNG.Next(yDim));

                // Only allow non-repeated centers
                if (unvisited.Contains(tilePos))
                {
                    unvisited.Remove(tilePos);
                    visited.Add(tilePos, PRNG.Next(areaTypes.Count));
                }
            }

            centers = new List<(int, int)>(visited.Keys);

            // Assign each point to the closest area center
            for (int y = 0; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    // Only do this if the current point is not already a center
                    if (unvisited.Contains((x, y)))
                    {
                        double minDist = double.PositiveInfinity;
                        int closestCenter = int.MaxValue;

                        // Determine closest center
                        foreach ((int, int) point in centers)
                        {
                            double dist = distFunc((x, y), point);

                            if (dist < minDist)
                            {
                                minDist = dist;
                                closestCenter = visited[point];
                            }
                        }

                        // Remove current point from unvisited list, add it to
                        // visited collection, specifying the closest center
                        unvisited.Remove((x, y));
                        visited.Add((x, y), closestCenter);
                    }

                    // Color current point according to closest center
                    pixels[y * xDim + x] = areaTypes[visited[(x, y)]];
                }
            }
        }
    }
}