/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

namespace GameAIPrototypes.TicTacToe
{
    /// <summary>
    /// A random TicTacToe player, not very smart.
    /// </summary>
    public class RandomAIPlayer : IPlayer
    {
        private Random random;

        public RandomAIPlayer()
        {
            random = new Random();
        }

        public Pos Play(Board gameBoard, ref string log)
        {
            // Populate a list with available board positions
            IList<Pos> emptyPositions = new List<Pos>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Pos pos = new Pos(i, j);
                    if (gameBoard.GetStateAt(pos) == CellState.Undecided)
                    {
                        emptyPositions.Add(pos);
                    }
                }
            }

            // Return a random empty board position
            return emptyPositions[random.Next(emptyPositions.Count)];
        }
    }
}