/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace AIUnityExamples.RobbyOptimize
{
    public struct Situation
    {
        private readonly int index;

        private static readonly int numBase;
        private const int NUM_TILES = 5;

        public Tile North { get; private set; }
        public Tile South { get; private set; }
        public Tile East { get; private set; }
        public Tile West { get; private set; }
        public Tile Current { get; private set; }

        private Tile this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return North;
                    case 1: return South;
                    case 2: return East;
                    case 3: return West;
                    case 4: return Current;
                    default:
                        throw new ArgumentOutOfRangeException(
                           "Situation index must be between 0 and 4");
                }
            }
            set
            {
                switch (i)
                {
                    case 0: North = value; break;
                    case 1: South = value; break;
                    case 2: East = value; break;
                    case 3: West = value; break;
                    case 4: Current = value; break;
                    default:
                        throw new ArgumentOutOfRangeException(
                           "Situation index must be between 0 and 4");
                }
            }
        }

        public int Index => index;

        static Situation()
        {
            numBase = Enum.GetValues(typeof(Tile)).Length;
        }

        private int GetIndex()
        {
            int index = 0;
            int current = 1;
            for (int i = 0; i < NUM_TILES; i++)
            {
                int digit = (int)this[i];
                index += current * digit;
                current *= numBase;
            }
            return index;
        }

        public Situation(Tile north, Tile south, Tile east, Tile west, Tile current)
        {
            North = north;
            South = south;
            East = east;
            West = west;
            Current = current;
            index = GetIndex();
        }

        public Situation(int index)
        {
            this.index = index;
            int i = 0;
            while (index > 0)
            {
                this[i] = (Tile)(index % numBase);
                index /= numBase;
                i++;
            }
        }
    }
}