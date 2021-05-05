/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace AIUnityExamples.RobbyOptimize.RobbyModel
{
    public static class TileUtil
    {
        private static readonly int numBase;
        private const int NUM_TILES = 5;

        static TileUtil()
        {
            numBase = Enum.GetValues(typeof(Tile)).Length;
        }

        public static int ToDecimal(Tile[] tiles)
        {
            int index = 0;
            int current = 1;
            for (int i = 0; i < NUM_TILES; i++)
            {
                int digit = (int)tiles[i];
                index += current * digit;
                current *= numBase;
            }
            return index;
        }
        public static void FromDecimal(int index, Tile[] tiles)
        {
            int i = 0;
            while (index > 0)
            {
                tiles[i] = (Tile)(index % numBase);
                index /= numBase;
                i++;
            }
        }
    }
}