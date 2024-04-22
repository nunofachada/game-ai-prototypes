/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameAIPrototypes.ProceduralLandscape
{
    public class CA
    {
        private enum NeighType { Moore, VonNeumann };

        private int CountNeighbors(int[] map, int width, int height, int row, int col,
            int radius, bool toroidal, int nonToroidalBorderCells, int neighValue,
            NeighType neighType)
        {
            int numNeighs = 0;

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    if (neighType == NeighType.VonNeumann && Math.Abs(i) + Math.Abs(j) < radius)
                    {
                        continue;
                    }

                    int r = Wrap(row + i, height, out bool wrapRow);
                    int c = Wrap(col + j, width, out bool wrapCol);

                    if (!toroidal && (wrapRow || wrapCol))
                    {
                        if (nonToroidalBorderCells == neighValue)
                        {
                            numNeighs++;
                        }
                    }
                    else if (map[r * width + c] == neighValue)
                    {
                        numNeighs++;
                    }
                }
            }
            return numNeighs;
        }

        private int Wrap(int pos, int max, out bool wrap)
        {
            wrap = false;
            if (pos < 0)
            {
                pos += pos;
                wrap = true;
            }
            else if (pos >= max)
            {
                pos -= max;
                wrap = true;
            }
            return pos;
        }
    }
}