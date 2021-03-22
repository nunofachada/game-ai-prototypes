/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace AIUnityExamples.TicTacToe
{
    /// <summary>
    /// Simple heuristic which only checks available lines for each player.
    /// </summary>
    public class AvailableLinesHeuristic : IHeuristic
    {
        // Evaluate the given board from the perspective of the given player
        public float Evaluate(Board board, CellState player)
        {
            // The final evaluation is equal to the score for the given player
            // minus the score for the other player
            // This could all be done in one function call and be more efficient,
            // but this way it's simpler to understand
            return ScoreFor(board, player) - ScoreFor(board, player.Other());
        }

        // Maximum score for a win
        public float WinScore => float.PositiveInfinity;

        // Get board score for the specified player, ignoring the other player
        private float ScoreFor(Board board, CellState player)
        {
            // Current score
            float score = 0;

            // Search all corridors in the board
            foreach (Pos[] corridor in Board.winCorridors)
            {
                // By default we assume a line is available
                bool lineAvailable = true;

                // Cycle through all the positions in the current corridor
                foreach (Pos position in corridor)
                {
                    // Check if there's an opponent piece at this position
                    if (board.GetStateAt(position) == player.Other())
                    {
                        // If so, it means there's no line available here, so
                        // let's take a note of that and skip the rest of the line
                        lineAvailable = false;
                        break;
                    }
                }

                // Was the previous line available for our player?
                if (lineAvailable)
                {
                    // If so, increment score
                    score++;
                }
            }

            // Return the score
            return score;
        }
    }
}