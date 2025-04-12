
/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibGameAI.Util
{
    public class GridOccupancy
    {

        private readonly bool[] grid;
        private readonly float detail;
        private readonly int gridWidth, gridHeight;
        //private readonly List<(float x, float y, float r)> placed;
        //private readonly bool toroidal;

        //public IEnumerable<(float x, float y)> Placed => placed;

        //public int Count => placed.Count;

        public bool this[float x, float y] =>
            grid[MMath.Round(y *  detail) * gridWidth + MMath.Round(x * detail)];


        public GridOccupancy(float width, float height, float detail) //, bool toroidal = false)
        {

            this.detail = detail;
            //this.toroidal = toroidal;
            gridWidth = MMath.Round(width * detail);
            gridHeight = MMath.Round(height * detail);
            grid = new bool[gridWidth * gridHeight];
            //placed = new List<(float x, float y, float r)>();
        }

        public bool TryPlace(IEnumerable<(float x, float y)> toPlace)
        {

            //UnityEngine.Debug.Log($"Putting {cellsToCheck.Count()} cells for ({x}, {y}, {r})!");
            bool anyInGrid = false;

            // Check if any cells are occupied
            foreach ((float x, float y) in toPlace)
            {
                int cx = MMath.Round(x * detail);
                int cy = MMath.Round(y * detail);

                if (cx < 0 || cx >= gridWidth || cy < 0 || cy >= gridHeight) continue;
                anyInGrid = true;

                // If so, don't add anything and return false
                if (grid[cy * gridWidth + cx])
                {
                    //UnityEngine.Debug.Log($"   => Unable to place in grid: ({cx}, {cy}) is true! Real = ({x}, {y})!");
                    return false;
                }
            }

            if (!anyInGrid)
            {
                //UnityEngine.Debug.Log($"   => Unable to place {toPlace}! All points outside grid!");
                return false;
            }

            // If we get here, it means there's space to place a new disk
                foreach ((float x, float y) in toPlace)
                {
                    int cx = MMath.Round(x * detail);
                    int cy = MMath.Round(y * detail);

                    if (cx < 0 || cx >= gridWidth || cy < 0 || cy >= gridHeight) continue;

                    //UnityEngine.Debug.Log($"   => Set grid ({cx}, {cy}) to True!!!!");
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
            output.Reverse();
            sb = new StringBuilder();
            sb.AppendJoin('\n', output);
            return sb.ToString();
        }


    }
}