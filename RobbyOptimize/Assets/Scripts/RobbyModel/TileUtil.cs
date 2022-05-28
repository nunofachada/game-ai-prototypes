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
        public static readonly int numStates;
        public static readonly int numRules;
        public const int NUM_NEIGHBORS = 5;

        static TileUtil()
        {
            numStates = Enum.GetValues(typeof(Tile)).Length;
            numRules = 1;
            for (int i = 0; i < NUM_NEIGHBORS; i++) numRules *= numStates;
        }

        public static int ToDecimal(Tile[] tiles)
        {
            System.Diagnostics.Debug.Assert(tiles.Length == NUM_NEIGHBORS);

            int base10 = 0;
            int current = 1;
            for (int i = 0; i < NUM_NEIGHBORS; i++)
            {
                int digit = (int)tiles[i];
                base10 += current * digit;
                current *= numStates;
            }
            return base10;
        }
        public static void FromDecimal(int base10, Tile[] tiles)
        {
            System.Diagnostics.Debug.Assert(tiles.Length == NUM_NEIGHBORS);
            System.Diagnostics.Debug.Assert(base10 >= 0 && base10 < numRules);

            for (int i = 0; i < NUM_NEIGHBORS; i++)
            {   if (base10 > 0)
                {
                    tiles[i] = (Tile)(base10 % numStates);
                    base10 /= numStates;
                }
                else
                {
                    tiles[i] = Tile.Empty;
                }
            }
        }

        public static string ToString(Tile[] tiles)
        {
            System.Diagnostics.Debug.Assert(tiles.Length == NUM_NEIGHBORS);

            return string.Format(
                "North:{0} | South:{1} | East:{2} | West:{3} | Current:{4}",
                tiles[0], tiles[1], tiles[2], tiles[3], tiles[4]);
        }
    }
}