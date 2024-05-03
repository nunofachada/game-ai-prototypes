/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

namespace LibGameAI.Util
{
    public static class NeighborhoodExtensions
    {
        public static IEnumerable<(int x, int y)> GetNeighborhood(this Neighborhood neighborhood, int radius, bool self = false)
        {
            return neighborhood switch
            {
                Neighborhood.VonNeumann => VonNeumannNeighboors(radius, self),
                Neighborhood.Moore => MooreNeighboors(radius, self),
                Neighborhood.Hexagonal => HexNeighboors(radius, self),
                _ => throw new ArgumentException("Unknown neighborhood!"),
            };
        }

        public static IEnumerable<(int x, int y)> MooreNeighboors(int radius, bool self = false)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (!self && x == 0 && y == 0)
                        continue;
                    yield return (x, y);
                }
            }
        }

        public static IEnumerable<(int x, int y)> VonNeumannNeighboors(int radius, bool self = false)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (Math.Abs(y) + Math.Abs(x) < radius)
                    {
                        continue;
                    }
                    if (!self && x == 0 && y == 0)
                    {
                        continue;
                    }
                    yield return (x, y);
                }
            }
        }

        public static IEnumerable<(int x, int y)> HexNeighboors(int radius, bool self = false)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = Math.Max(-radius, -y - radius); x <= Math.Min(radius, -y + radius); x++)
                {
                    if (!self && x == 0 && y == 0)
                        continue;
                    yield return (x, y);
                }
            }
        }
    }
}