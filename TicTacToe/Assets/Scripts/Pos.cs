/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

namespace GameAIPrototypes.TicTacToe
{
    /// <summary>
    /// Represents a position in the Tic Tac Toe board.
    /// </summary>
    public struct Pos
    {
        public readonly int row;
        public readonly int col;

        public Pos(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public override string ToString() => $"({row}, {col})";
    }
}