
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

namespace AIUnityExamples.TicTacToe
{
    /// <summary>
    /// The possible states of a board position (cell) in TicTacToe.
    /// </summary>
    public enum CellState
    {
        // No move
        Undecided,
        // X move
        X,
        // O move
        O
    }
}