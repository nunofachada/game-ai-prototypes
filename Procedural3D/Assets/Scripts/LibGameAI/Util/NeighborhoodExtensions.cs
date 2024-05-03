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
        public static IEnumerable<(int x, int y)> GetNeighborhood(this Neighborhood neighborhood, int radius)
        {
            return neighborhood switch
            {
                Neighborhood.VonNeumann => VonNeumannNeighbors(radius),
                Neighborhood.Moore => MooreNeighbors(radius),
                Neighborhood.Hexagonal => HexNeighbors(radius),
                _ => throw new ArgumentException("Unknown neighborhood!"),
            };
        }

        public static IEnumerable<(int x, int y)> MooreNeighbors(int radius)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    yield return (x, y);
                }
            }
        }

        public static IEnumerable<(int x, int y)> VonNeumannNeighbors(int radius)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = Math.Abs(y) - radius; x <= -Math.Abs(y) + radius; x++)
                {
                    yield return (x, y);
                }
            }
        }

        public static IEnumerable<(int x, int y)> HexNeighbors(int radius)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = Math.Max(-radius, -y - radius); x <= Math.Min(radius, -y + radius); x++)
                {
                    yield return (x, y);
                }
            }
        }
    }
}