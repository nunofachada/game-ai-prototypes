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

            int index = 0;
            int current = 1;
            for (int i = 0; i < NUM_NEIGHBORS; i++)
            {
                int digit = (int)tiles[i];
                index += current * digit;
                current *= numStates;
            }
            return index;
        }
        public static void FromDecimal(int index, Tile[] tiles)
        {
            System.Diagnostics.Debug.Assert(tiles.Length == NUM_NEIGHBORS);
            System.Diagnostics.Debug.Assert(index < numRules && index >= 0);

            for (int i = 0; i < NUM_NEIGHBORS; i++)
            {   if (index > 0)
                {
                    tiles[i] = (Tile)(index % numStates);
                    index /= numStates;
                }
                else
                {
                    tiles[i] = Tile.Empty;
                }
            }
        }
    }
}