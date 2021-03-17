/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;

namespace AIUnityExamples.TicTacToe
{
    /// <summary>
    /// Negamax AI with alpha-beta pruning.
    /// </summary>
    public class ABNegamaxAIPlayer : IPlayer
    {
        // Maximum depth
        private int maxDepth;

        // Number of evaluations performed
        private int numEvals;

        // Heuristic to use
        private IHeuristic heuristic;

        // Initialize the AI
        // Edit the values inside to configure the AI
        public ABNegamaxAIPlayer()
        {
            maxDepth = 3000;
            heuristic = new AvailableLinesHeuristic();
        }

        // Play a turn
        public Vector2Int Play(Board gameBoard, CellState turn)
        {
            // Number of evaluations (recursive ABNegamax calls) starts at zero
            numEvals = 0;

            // Keep start time
            DateTime startTime = DateTime.Now;

            // Call ABNegamax at root board
            (float score, Vector2Int move) decision = ABNegamax(
                gameBoard, turn, 0, float.NegativeInfinity, float.PositiveInfinity);

            // Provide debug information
            Debug.Log(string.Format(
                "ABNegamax called {0} times, took {1} ms",
                numEvals,
                (DateTime.Now - startTime).TotalMilliseconds));

            // Return best move
            return decision.move;
        }

        // Process given board with ABNegamax
        private (float score, Vector2Int move) ABNegamax(
            Board board, CellState turn, int depth, float alpha, float beta)
        {
            // Increment number of evaluations (recursive ABNegamax calls)
            numEvals++;

            // Check what's the status of this board
            if (board.Status().HasValue)
            {
                // GAME OVER! Who won?

                if (board.Status().Value == CellState.Undecided)
                {
                    // It's a tie, return 0
                    return (0, Vector2Int.zero);
                }
                else if (board.Status().Value == turn)
                {
                    // Current player wins, return max heuristic value
                    return (heuristic.WinScore, Vector2Int.zero);
                }
                else
                {
                    // The other player won, return min heuristic value
                    return (-heuristic.WinScore, Vector2Int.zero);
                }
            }
            else if (depth == maxDepth)
            {
                // We reached the max depth, return the heuristic value for this
                // board
                return (heuristic.Evaluate(board, turn), board.NoMove);
            }
            else
            {
                // Game is not over and we haven't reached maximum depth, so let's
                // recursively call ABNegamax on all possible moves

                // Declare best move, which for now is no move at all
                (float score, Vector2Int move) bestMove =
                    (float.NegativeInfinity, board.NoMove);

                // Try to play on all board positions
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Get a vector reference to the current board position
                        Vector2Int pos = new Vector2Int(i, j);

                        // Only consider making a move at this position if it's
                        // not already occupied
                        if (board.GetStateAt(pos) == CellState.Undecided)
                        {
                            // Score for current move
                            float score;

                            // Make a virtual move at this position
                            board.SetStateAt(pos, turn);

                            // Get score for this move
                            score = -ABNegamax(
                                    board, turn.Other(), depth + 1, -beta, -alpha)
                                .score;

                            // Undo the move we just evaluated
                            board.SetStateAt(pos, CellState.Undecided);

                            // Is this the best move so far?
                            if (score > bestMove.score)
                            {
                                // If so, update best move
                                bestMove = (score, pos);
                            }

                            // Do we have a larger alpha?
                            if (score > alpha)
                            {
                                // If so, update alpha
                                alpha = score;
                            }

                            // Is alpha higher than beta?
                            if (alpha >= beta)
                            {
                                // If so, make alpha-beta cut and return the
                                // best move so far
                                return bestMove;
                            }
                        }
                    }
                }

                // If we get here, it means no alpha-beta pruning occurred
                // Return the best move found among all the tested moves
                return bestMove;
            }
        }
    }
}