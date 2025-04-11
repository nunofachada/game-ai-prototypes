/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;
using LibGameAI.Util;

namespace LibGameAI.Geometry
{
    public static class Bresenham
    {
        public static IEnumerable<(int x, int y)> GetLine(
            (int x, int y) start,
            (int x, int y) end,
            (int x, int y)? lims = null,
            bool toroidal = false)
        {
            (int width, int height) = lims ?? (int.MaxValue, int.MaxValue);

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

                if (toroidal)
                {
                    (px, _) = Grid.Wrap(px, width);
                    (py, _) = Grid.Wrap(py, height);
                    yield return (px, py);
                }
                else
                {
                    if (px >= 0 && px < width && py >= 0 && py < height)
                        yield return (px, py);
                }

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
            int radius,
            (int x, int y)? lims = null,
            bool toroidal = false)
        {
            (int cx, int cy) = center;
            (int maxX, int maxY) = lims ?? (int.MaxValue, int.MaxValue);

            int x = 0;
            int y = radius;
            int d = 3 - 2 * radius;

            HashSet<(int x, int y)> visited = new();

            void TryAdd(int px, int py)
            {
                if (toroidal)
                {
                    // Wrap coordinates
                    (int wrappedX, _) = Grid.Wrap(px, maxX);
                    (int wrappedY, _) = Grid.Wrap(py, maxY);
                    visited.Add((wrappedX, wrappedY));
                }
                else
                {
                    if (px >= 0 && px <= maxX && py >= 0 && py <= maxY)
                    {
                        visited.Add((px, py));
                    }
                }
            }

            void PlotCirclePoints(int xc, int yc, int x, int y)
            {
                TryAdd(xc + x, yc + y);
                TryAdd(xc - x, yc + y);
                TryAdd(xc + x, yc - y);
                TryAdd(xc - x, yc - y);
                TryAdd(xc + y, yc + x);
                TryAdd(xc - y, yc + x);
                TryAdd(xc + y, yc - x);
                TryAdd(xc - y, yc - x);
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

        public static IEnumerable<(int x, int y)> GetFilledCircle(
            (int x, int y) center,
            int radius,
            (int x, int y)? lims = null,
            bool toroidal = false)
        {
            int x = radius;
            int y = 0;
            int decision = 1 - x;

            (int maxX, int maxY) = lims ?? (int.MaxValue, int.MaxValue);

            List<(int x, int y)> circlePoints = new();

            IEnumerable<(int x, int y)> DrawHorizontalLine(
                int width,
                int height,
                int xStart,
                int xEnd,
                int y)
            {
                if (height <= 0 || width <= 0)
                    yield break;

                if (toroidal)
                {
                    (int wrappedY, _) = Grid.Wrap(y, height);

                    for (int x = xStart; x <= xEnd; x++)
                    {
                        (int wrappedX, _) = Grid.Wrap(x, width);
                        yield return (wrappedX, wrappedY);
                    }
                }
                else
                {
                    if (y < 0 || y >= height)
                        yield break;

                    xStart = Math.Clamp(xStart, 0, width - 1);
                    xEnd = Math.Clamp(xEnd, 0, width - 1);

                    for (int x = xStart; x <= xEnd; x++)
                    {
                        yield return (x, y);
                    }
                }
            }

            while (x >= y)
            {
                circlePoints.AddRange(
                    DrawHorizontalLine(
                        maxX, maxY, center.x - x, center.x + x, center.y + y));
                circlePoints.AddRange(
                    DrawHorizontalLine(
                        maxX, maxY, center.x - x, center.x + x, center.y - y));
                circlePoints.AddRange(
                    DrawHorizontalLine(
                        maxX, maxY, center.x - y, center.x + y, center.y + x));
                circlePoints.AddRange(
                    DrawHorizontalLine(
                        maxX, maxY, center.x - y, center.x + y, center.y - x));

                y++;

                if (decision <= 0)
                {
                    decision += 2 * y + 1;
                }
                else
                {
                    x--;
                    decision += 2 * (y - x) + 1;
                }
            }

            return circlePoints;

        }

    }
}