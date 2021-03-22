/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace AIUnityExamples.TicTacToe
{
    /// <summary>
    /// A simple Negamax AI player.
    /// </summary>
    public class NegamaxAIPlayer : IPlayer
    {
        // Maximum depth
        private int maxDepth;

        // Number of evaluations performed
        private int numEvals;

        // Heuristic to use
        private IHeuristic heuristic;

        // Initialize the AI
        // Edit the values inside to configure the AI
        public NegamaxAIPlayer()
        {
            maxDepth = 3000;
            heuristic = new AvailableLinesHeuristic();
        }

        // Play a turn
        public Pos Play(Board gameBoard, ref string log)
        {
            // Number of evaluations (recursive Negamax calls) starts at zero
            numEvals = 0;

            // Call Negamax at root board
            (float score, Pos move) decision = Negamax(gameBoard, 0);

            // Provide debug information
            log = $"Performed {numEvals} evaluations";

            // Return best move
            return decision.move;
        }

        // Process given board with ABNegamax
        private (float score, Pos move) Negamax(Board board, int depth)
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
                    return (0, Board.NoMove);
                }
                else if (board.Status().Value == board.Turn)
                {
                    // Current player wins, return max heuristic value
                    return (heuristic.WinScore, Board.NoMove);
                }
                else
                {
                    // The other player won, return min heuristic value
                    return (-heuristic.WinScore, Board.NoMove);
                }
            }
            else if (depth == maxDepth)
            {
                // We reached the max depth, return the heuristic value for this
                // board
                return (heuristic.Evaluate(board, board.Turn), Board.NoMove);
            }
            else
            {
                // Game is not over and we haven't reached maximum depth, so let's
                // recursively call Negamax on all possible moves

                // Declare best move, which for now is no move at all
                (float score, Pos move) bestMove =
                    (float.NegativeInfinity, Board.NoMove);

                // Try to play on all board positions
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Get the current board position
                        Pos pos = new Pos(i, j);

                        // Only consider making a move at this position if it's
                        // not already occupied
                        if (board.GetStateAt(pos) == CellState.Undecided)
                        {
                            // Score for current move
                            float score;

                            // Make a virtual move at this position
                            board.DoMove(pos);

                            // Get score for this move
                            score = -Negamax(board, depth + 1).score;

                            // Undo the move we just evaluated
                            board.UndoMove();

                            // Is this the best move so far?
                            if (score > bestMove.score)
                            {
                                // If so, keep it
                                bestMove = (score, pos);
                            }
                        }
                    }
                }
                // Return the best move found among all the tested moves
                return bestMove;
            }
        }
    }
}