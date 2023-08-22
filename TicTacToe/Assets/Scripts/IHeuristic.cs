/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

namespace GameAIPrototypes.TicTacToe
{
    /// <summary>
    /// Interface to simplify the use of heuristics.
    /// </summary>
    public interface IHeuristic
    {
        // Get a board evaluation from the perspective of the given player
        float Evaluate(Board board, CellState player);

        // Maximum score for a win
        float WinScore { get; }
    }
}