/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;
using System.Linq;
using LibGameAI.Util;

namespace LibGameAI.Geometry
{
    public static class Bresenham
    {
        public static IEnumerable<(int x, int y)> GetLine(
            (int x, int y) start,
            (int x, int y) end//,
            // (int x, int y)? lims = null,
            // bool toroidal = false
            )
        {
            //(int width, int height) = lims ?? (int.MaxValue, int.MaxValue);

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

                // if (toroidal)
                // {
                //     (px, _) = Grid.Wrap(px, width);
                //     (py, _) = Grid.Wrap(py, height);
                //     yield return (px, py);
                // }
                // else
                // {
                //     if (px >= 0 && px < width && py >= 0 && py < height)
                //         yield return (px, py);
                // }

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
            int radius//,
            //(int x, int y)? lims = null,
            //bool toroidal = false
            )
        {
            (int cx, int cy) = center;
            //(int maxX, int maxY) = lims ?? (int.MaxValue, int.MaxValue);

            int x = 0;
            int y = radius;
            int d = 3 - 2 * radius;

            HashSet<(int x, int y)> visited = new();

            // void TryAdd(int px, int py)
            // {
            //     if (toroidal)
            //     {
            //         // Wrap coordinates
            //         (int wrappedX, _) = Grid.Wrap(px, maxX);
            //         (int wrappedY, _) = Grid.Wrap(py, maxY);
            //         visited.Add((wrappedX, wrappedY));
            //     }
            //     else
            //     {
            //         if (px >= 0 && px < maxX && py >= 0 && py < maxY)
            //         {
            //             visited.Add((px, py));
            //         }
            //     }
            // }

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

        // public static IEnumerable<(int x, int y)> GetFilledCircle(
        //     (int x, int y) center,
        //     int radius,
        //     (int x, int y)? lims = null,
        //     bool toroidal = false)
        // {
        //     int x = radius;
        //     int y = 0;
        //     int decision = 1 - x;

        //     // Treat lims as the grid dimensions (width, height)
        //     (int gridWidth, int gridHeight) = lims ?? (int.MaxValue, int.MaxValue);

        //     // This dictionary will record for each y the leftmost and rightmost x of the circle outline.
        //     Dictionary<int, (int minX, int maxX)> rowExtents = new();

        //     // A helper to record a point on the outline.
        //     void RecordPoint(int px, int py)
        //     {
        //         // For non-toroidal mode, skip points outside the grid.
        //         if (!toroidal && (py < 0 || py >= gridHeight)) //  || px < 0 || px >= gridWidth
        //             return;

        //         if (rowExtents.TryGetValue(py, out var ext))
        //         {
        //             // Update the min and max encountered on this row.
        //             ext.minX = Math.Min(ext.minX, px);
        //             ext.maxX = Math.Max(ext.maxX, px);
        //             rowExtents[py] = ext;
        //         }
        //         else
        //         {
        //             rowExtents[py] = (px, px);
        //         }
        //     }


        //     // The Bresenham loop: Record outline points in all 8 octants.
        //     while (x >= y)
        //     {
        //         // For each computed pair, record the corresponding symmetric points.
        //         RecordPoint(center.x - x, center.y + y);
        //         RecordPoint(center.x + x, center.y + y);
        //         RecordPoint(center.x - x, center.y - y);
        //         RecordPoint(center.x + x, center.y - y);

        //         if (x != y)
        //         {
        //             RecordPoint(center.x - y, center.y + x);
        //             RecordPoint(center.x + y, center.y + x);
        //             RecordPoint(center.x - y, center.y - x);
        //             RecordPoint(center.x + y, center.y - x);
        //         }

        //         y++;

        //         if (decision <= 0)
        //         {
        //             decision += 2 * y + 1;
        //         }
        //         else
        //         {
        //             x--;
        //             decision += 2 * (y - x) + 1;
        //         }
        //     }

        //     // Now create the filled circle by drawing a horizontal span for each row between the min and max x.
        //     List<(int x, int y)> filledPoints = new();
        //     foreach (var kv in rowExtents)
        //     {
        //         int py = kv.Key;
        //         // In non-toroidal mode, clamp the row extents to the grid width.
        //         int minX = toroidal ? kv.Value.minX : Math.Clamp(kv.Value.minX, 0, gridWidth - 1);
        //         int maxX = toroidal ? kv.Value.maxX : Math.Clamp(kv.Value.maxX, 0, gridWidth - 1);
        //         for (int px = minX; px <= maxX; px++)
        //         {
        //             if (toroidal)
        //             {
        //                 // In toroidal mode, wrap the coordinates.
        //                 (int wrappedX, _) = Grid.Wrap(px, gridWidth);
        //                 (int wrappedY, _) = Grid.Wrap(py, gridHeight);
        //                 filledPoints.Add((wrappedX, wrappedY));
        //             }
        //             else
        //             {
        //                 filledPoints.Add((px, py));
        //             }
        //         }
        //     }

        //     return new SortedSet<(int x, int y)>(filledPoints);
        // }


        public static IEnumerable<(int x, int y)> GetFilledCirclez(
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

                    for (int x = xStart; x <= xEnd; x++)
                    {
                        if (x >= 0 && x < width)
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

            return new SortedSet<(int x, int y)>(circlePoints);

        }

        /// <summary>
        /// Returns all the points inside a filled circle by leveraging the border
        /// points computed with GetCircle(). For each row, fill from the leftmost
        /// to the rightmost border pixel.
        /// </summary>
        public static IEnumerable<(int x, int y)> GetFilledCircle(
            (int x, int y) center,
            int radius//,
            //(int x, int y)? lims = null,
            //bool toroidal = false
            )
        {
            // If limits are provided, use them as width and height
            //(int width, int height) = lims ?? (int.MaxValue, int.MaxValue);



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
                    // if (toroidal)
                    // {
                    //     // Wrap both x and y coordinates.
                    //     (int wrappedX, _) = Grid.Wrap(x, width);
                    //     (int wrappedY, _) = Grid.Wrap(y, height);
                    //     yield return (wrappedX, wrappedY);
                    // }
                    // else
                    // {
                    //     // If limits are provided and we are not using toroidal wrapping,
                    //     // ensure the point is actually within bounds.

                    //     if (x < 0 || x >= width || y < 0 || y >= height)
                    //         continue;
                    //     yield return (x, y);
                    // }
                }
            }
        }

    }
}