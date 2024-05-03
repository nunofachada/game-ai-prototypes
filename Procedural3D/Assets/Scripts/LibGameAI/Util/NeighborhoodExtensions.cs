/* Copyright (c) 2018-2024 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

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