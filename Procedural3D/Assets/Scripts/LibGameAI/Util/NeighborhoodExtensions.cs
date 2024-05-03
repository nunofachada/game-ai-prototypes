/* Copyright (c) 2018-2024 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;

namespace LibGameAI.Util
{
    /// <summary>
    /// Useful extension methods for processing neighborhoods.
    /// </summary>
    public static class NeighborhoodExtensions
    {
        private const string UNKNOWN_TYPE = "Unknown neighborhood type!";

        /// <summary>
        /// Maximum number of neighbors for this neighborhood type and radius.
        /// </summary>
        /// <param name="neighborhood">Neighborhood type.</param>
        /// <param name="radius">Neighborhood radius.</param>
        /// <returns>
        /// Maximum number of neighbors for this neighborhood type and radius.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the neighborhood type is unknown.
        /// </exception>
        public static int MaxNeighbors(this Neighborhood neighborhood, int radius)
        {
            return neighborhood switch
            {
                Neighborhood.Moore => (2 * radius + 1) * (2 * radius + 1) - 1,
                Neighborhood.VonNeumann => 2 * radius * (1 + radius),
                Neighborhood.Hexagonal => 3 * radius * (radius + 1),
                _ => throw new ArgumentException(UNKNOWN_TYPE)
            };
        }

        /// <summary>
        /// Returns a collection of relative positions constituting the
        /// neighborhood of the given neighborhood type and radius.
        /// </summary>
        /// <param name="neighborhood">Neighborhood type.</param>
        /// <param name="radius">Neighborhood radius.</param>
        /// <returns>
        /// A collection of relative positions constituting the neighborhood of
        /// the given neighborhood type and radius.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the neighborhood type is unknown.
        /// </exception>
        public static IEnumerable<(int x, int y)> GetNeighborhood(this Neighborhood neighborhood, int radius)
        {
            return neighborhood switch
            {
                Neighborhood.VonNeumann => VonNeumannNeighbors(radius),
                Neighborhood.Moore => MooreNeighbors(radius),
                Neighborhood.Hexagonal => HexNeighbors(radius),
                _ => throw new ArgumentException(UNKNOWN_TYPE),
            };
        }

        /// <summary>
        /// Returns a collection of relative positions constituting the Moore
        /// neighborhood with the given radius.
        /// </summary>
        /// <param name="radius">Neighborhood radius.</param>
        /// <returns>
        /// A collection of relative positions constituting the Moore
        /// neighborhood with the given radius.
        /// </returns>
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

        /// <summary>
        /// Returns a collection of relative positions constituting the Von
        /// Neumann neighborhood with the given radius.
        /// </summary>
        /// <param name="radius">Neighborhood radius.</param>
        /// <returns>
        /// A collection of relative positions constituting the Von Neumann
        /// neighborhood with the given radius.
        /// </returns>
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

        /// <summary>
        /// Returns a collection of relative positions constituting the
        /// hexagonal neighborhood with the given radius.
        /// </summary>
        /// <param name="radius">Neighborhood radius.</param>
        /// <returns>
        /// A collection of relative positions constituting the hexagonal
        /// neighborhood with the given radius.
        /// </returns>
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