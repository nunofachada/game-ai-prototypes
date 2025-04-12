
/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System.Text;
using System.Collections.Generic;


namespace LibGameAI.Util
{
    public class GridOccupancy
    {

        private readonly bool[] grid;
        private readonly float detail;
        private readonly int gridWidth, gridHeight;


        public bool this[float x, float y] =>
            grid[MMath.Round(y * detail) * gridWidth + MMath.Round(x * detail)];


        public GridOccupancy(float width, float height, float detail)
        {
            this.detail = detail;
            gridWidth = MMath.Round(width * detail);
            gridHeight = MMath.Round(height * detail);
            grid = new bool[gridWidth * gridHeight];
        }

        public bool TryPlace(IEnumerable<(float x, float y)> toPlace)
        {

            List<(int x, int y)> available = new();

            // Check if any cells are occupied
            foreach ((float x, float y) in toPlace)
            {
                // Convert coordinates to grid space
                int cx = MMath.Round(x * detail);
                int cy = MMath.Round(y * detail);

                // If current coordinates are outside the grid, try next ones
                if (cx < 0 || cx >= gridWidth || cy < 0 || cy >= gridHeight) continue;

                // If so, don't add anything and return false
                if (grid[cy * gridWidth + cx])
                {
                    return false;
                }

                available.Add((cx, cy));
            }

            // If no cells are available, it's not possible to add this object
            // to the grid
            if (available.Count == 0) return false;

            // If we get here, it means there's space to place a the object
            foreach ((int cx, int cy) in available)
            {
                // Mark each "pixel" of the object as occupied
                grid[cy * gridWidth + cx] = true;
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            List<StringBuilder> output = new();
            for (int i = 0; i < grid.Length; i++)
            {
                if (i % gridWidth == 0 && i > 0)
                {
                    output.Add(sb);
                    sb = new StringBuilder();
                }
                sb.Append(grid[i] ? "1" : 0);
            }
            output.Add(sb);
            //output.Reverse();
            sb = new StringBuilder();
            sb.AppendJoin('\n', output);
            return sb.ToString();
        }


    }
}