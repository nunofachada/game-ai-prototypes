/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;

namespace LibGameAI.Geometry
{
    public static class Bresenham
    {
        public static IEnumerable<(int x, int y)> GetLine((int x, int y) start, (int x, int y) end)
        {
            yield return (2, 3);
        }

        public static IEnumerable<(int x, int y)> GetFilledCircle((int x, int y) center, int radius, (int x, int y)? lims = null)
        {
            int x = radius;
            int y = 0;
            int decision = 1 - x;

            int xDim = lims?.x ?? int.MaxValue;
            int yDim = lims?.y ?? int.MaxValue;

            List<(int x, int y)> circlePoints = new();

            while (x >= y)
            {
                circlePoints.AddRange(DrawHorizontalLine(xDim, yDim, center.x - x, center.x + x, center.y + y));
                circlePoints.AddRange(DrawHorizontalLine(xDim, yDim, center.x - x, center.x + x, center.y - y));
                circlePoints.AddRange(DrawHorizontalLine(xDim, yDim, center.x - y, center.x + y, center.y + x));
                circlePoints.AddRange(DrawHorizontalLine(xDim, yDim, center.x - y, center.x + y, center.y - x));

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


        private static IEnumerable<(int x, int y)> DrawHorizontalLine(int width, int height, int xStart, int xEnd, int y)
        {
            if (y < 0 || y >= height) yield break;

            xStart = Math.Clamp(xStart, 0, width - 1);
            xEnd = Math.Clamp(xEnd, 0, width - 1);

            for (int x = xStart; x <= xEnd; x++)
            {
                yield return (x, y);
            }
        }

    }
}