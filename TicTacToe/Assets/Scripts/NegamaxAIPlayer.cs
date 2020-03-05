/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

// Basic negamax algorithm for automatically playing TicTacToe
public class NegamaxAIPlayer : IPlayer
{
    // This struct is used internally for keeping tabs on possible moves
    // and scores
    private struct Move
    {
        public Vector2Int? Pos { get; set; }
        public int Score { get; set; }
        public Move(Vector2Int? pos, int score)
        {
            Pos = pos;
            Score = score;
        }
    }

    public Vector2Int Play(IBoard gameBoard, CellState turn)
    {
        // Get the move to perform by invoking the recursive negamax function
        Move move = Negamax(gameBoard, turn);
        return move.Pos ?? Vector2Int.zero;
    }

    // The recursive negamax function
    private Move Negamax(IBoard gameBoard, CellState turn)
    {
        // Is the game over?
        if (gameBoard.Status() == null)
        {
            // Game not over, try to find a good move
            Move bestMove = new Move(null, int.MinValue);

            // Who's turn is next?
            CellState nextTurn =
                turn == CellState.X ? CellState.O : CellState.X;

            // Test all possible moves (this is not efficient, alpha-beta
            // pruning is required for a proper implementation)
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // A vector with the current position to test
                    Vector2Int pos = new Vector2Int(i, j);

                    // Is the position empty?
                    if (gameBoard.GetStateAt(pos) == CellState.Undecided)
                    {
                        // If so, let's test it
                        Move move;

                        // Do the move
                        gameBoard.SetStateAt(pos, turn);

                        // Evaluate the board with this move
                        move = Negamax(gameBoard, nextTurn);

                        // Undo the move
                        gameBoard.SetStateAt(pos, CellState.Undecided);

                        // Invert the score
                        move.Score = -move.Score;

                        // Is this move better than the best move so far?
                        if (move.Score > bestMove.Score)
                        {
                            // If so, update best move so far
                            bestMove.Score = move.Score;
                            bestMove.Pos = pos;
                        }
                    }
                }
            }

            // Return best move found
            return bestMove;
        }
        else
        {
            // Game is over, so this is a terminal board
            // Return 0 if the game ended in a draw, or -10 if it ended in
            // a victory for either side
            // There is not move to return since this is a final board, so
            // the move is null
            int score;
            if (gameBoard.Status().Value == CellState.Undecided)
                score = 0;
            else if (gameBoard.Status().Value == turn)
                score = 10;
            else
                score = -10;
            return new Move(null, score);
        }
    }
}
