/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;
using System.Linq;


namespace LibGameAI.Geometry
{
    public static class Bresenham
    {
        public static IEnumerable<(int x, int y)> GetLine(
            (int x, int y) start,
            (int x, int y) end)
        {
            int x0 = start.x;
            int y0 = start.y;
            int x1 = end.x;
            int y1 = end.y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            int err = dx - dy;

            while (true)
            {
                int px = x0;
                int py = y0;

                yield return (px, py);

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;

                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }


        public static IEnumerable<(int x, int y)> GetCircle(
            (int x, int y) center,
            int radius)
        {
            (int cx, int cy) = center;

            int x = 0;
            int y = radius;
            int d = 3 - 2 * radius;

            HashSet<(int x, int y)> visited = new();

            void PlotCirclePoints(int xc, int yc, int x, int y)
            {
                visited.Add((xc + x, yc + y));
                visited.Add((xc - x, yc + y));
                visited.Add((xc + x, yc - y));
                visited.Add((xc - x, yc - y));
                visited.Add((xc + y, yc + x));
                visited.Add((xc - y, yc + x));
                visited.Add((xc + y, yc - x));
                visited.Add((xc - y, yc - x));
            }

            while (x <= y)
            {
                PlotCirclePoints(cx, cy, x, y);
                if (d < 0)
                {
                    d += 4 * x + 6;
                }
                else
                {
                    d += 4 * (x - y) + 10;
                    y--;
                }
                x++;
            }

            return visited;

        }

        /// <summary>
        /// Returns all the points inside a filled circle by leveraging the border
        /// points computed with GetCircle(). For each row, fill from the leftmost
        /// to the rightmost border pixel.
        /// </summary>
        public static IEnumerable<(int x, int y)> GetFilledCircle(
            (int x, int y) center,
            int radius)
        {
           // Use GetCircle() to get the perimeter points considering the
            // extended limits
            IEnumerable<(int x, int y)> borderPoints = GetCircle(center, radius);
            if (borderPoints.Count() == 0)
                yield break;

            // Determine the vertical bounds from the border points
            int minY = borderPoints.Min(p => p.y);
            int maxY = borderPoints.Max(p => p.y);

            // Group the border points by y.
            // For each row that has border pixels, determine the leftmost and rightmost x
            Dictionary<int, (int min, int max)> rows = borderPoints
                .GroupBy(p => p.y)
                .ToDictionary(g => g.Key, g => (g.Min(p => p.x), g.Max(p => p.x)));

            // Now iterate over every row between minY and maxY
            for (int y = minY; y <= maxY; y++)
            {
                // Use the leftmost and rightmost border pixels on this row
                int xStart = rows[y].min;
                int xEnd = rows[y].max;

                // Fill the horizontal line from xStart to xEnd for this row
                for (int x = xStart; x <= xEnd; x++)
                {
                    yield return (x, y);
                }
            }
        }

    }
}