/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

namespace AIUnityExamples.TicTacToe
{
    /// <summary>
    /// Represents a TicTacToe board.
    /// </summary>
    public class Board
    {
        // 2D array containing board
        private CellState[,] board;

        // Stack of moves
        private Stack<Pos> moves;

        /// <summary>
        /// Represents a "no move".
        /// </summary>
        public static Pos NoMove => new Pos(int.MinValue, int.MinValue);

        /// <summary>
        /// Whose turn is it?
        /// </summary>
        public CellState Turn { get; private set; }

        /// <summary>
        /// Possible winning corridors.
        /// </summary>
        public static readonly IEnumerable<IReadOnlyList<Pos>> winCorridors;

        /// <summary>
        /// Create a new empty board.
        /// </summary>
        public Board()
        {
            board = new CellState[3, 3];

            moves = new Stack<Pos>();

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    board[x, y] = CellState.Undecided;

            Turn = CellState.X;
        }

        /// <summary>
        /// Static constructor, sets up the winning corridors.
        /// </summary>
        static Board()
        {
            winCorridors = new List<Pos[]>()
            {
                new Pos[] { new Pos(0, 0), new Pos(1, 0), new Pos(2, 0)},
                new Pos[] { new Pos(0, 1), new Pos(1, 1), new Pos(2, 1)},
                new Pos[] { new Pos(0, 2), new Pos(1, 2), new Pos(2, 2)},
                new Pos[] { new Pos(0, 0), new Pos(0, 1), new Pos(0, 2)},
                new Pos[] { new Pos(1, 0), new Pos(1, 1), new Pos(1, 2)},
                new Pos[] { new Pos(2, 0), new Pos(2, 1), new Pos(2, 2)},
                new Pos[] { new Pos(0, 0), new Pos(1, 1), new Pos(2, 2)},
                new Pos[] { new Pos(0, 2), new Pos(1, 1), new Pos(2, 0)}
            };
        }

        /// <summary>
        /// Get board state at specified position.
        /// </summary>
        /// <param name="pos">Position from where to get state.</param>
        /// <returns>board state at specified position.</returns>
        public CellState GetStateAt(Pos pos) => board[pos.row, pos.col];

        /// <summary>
        /// Make a move.
        /// </summary>
        /// <param name="pos">Position of board where to make move.</param>
        public void DoMove(Pos pos)
        {
            if (pos.row > 2 || pos.row < 0 || pos.col > 2 || pos.col < 0)
            {
                throw new InvalidOperationException(
                    $"Position {pos} is invalid for TicTacToe!");
            }
            if (board[pos.row, pos.col] != CellState.Undecided)
            {
                throw new InvalidOperationException(
                    $"Position {pos} is already occupied with {board[pos.row, pos.col]}");
            }
            board[pos.row, pos.col] = Turn;

            moves.Push(pos);

            Turn = Turn.Other();
        }

        /// <summary>
        /// Undo last move.
        /// </summary>
        /// <returns>Last move, which is now undone.</returns>
        public Pos UndoMove()
        {
            if (moves.Count == 0)
            {
                throw new InvalidOperationException(
                    "Board is empty, cannot undo moves.");
            }

            Pos move = moves.Pop();

            board[move.row, move.col] = CellState.Undecided;

            Turn = Turn.Other();

            return move;
        }

        /// <summary>
        /// Board winner status.
        /// </summary>
        /// <returns>
        /// This method can return the following values:
        /// * `null` - Game in progress
        /// * `CellState.Undecided` - Game over, there is a draw.
        /// * `CellState.X` - Game over, X wins
        /// * `CellState.O` - Game over, O wins
        /// </returns>
        public CellState? Status()
        {
            // Check win status for X and O
            foreach (CellState state in new CellState[] { CellState.X, CellState.O })
            {
                foreach (IReadOnlyList<Pos> wp in winCorridors)
                {
                    if (board[wp[0].row, wp[0].col] == state
                        && board[wp[1].row, wp[1].col] == state
                        && board[wp[2].row, wp[2].col] == state)
                    {
                        return state;
                    }
                }
            }

            // If board not full return null, meaning game is not over yet
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    if (board[x, y] == CellState.Undecided)
                        return null;

            // Game is over with a draw
            return CellState.Undecided;
        }

        /// <summary>
        /// Returns a copy of this board.
        /// </summary>
        /// <returns>A copy of this board.</returns>
        public Board Copy()
        {
            // Use memberwise clone to do a shallow copy of our board
            // We will have to copy the reference types by hand below
            Board boardCopy = MemberwiseClone() as Board;

            // Copy the move sequence to an array
            Pos[] movesArray = moves.ToArray();

            // Create a new move sequence in the copied board...
            boardCopy.moves = new Stack<Pos>();

            // ...and push the moves in reverse order they appear in the array
            for (int i = movesArray.Length - 1; i >= 0; i--)
                boardCopy.moves.Push(movesArray[i]);

            // Copy the board's contents onto the new board object
            boardCopy.board = board.Clone() as CellState[,];

            // Return a copy of the board
            return boardCopy;
        }

        /// <summary>
        /// Return a nice string representation of this board.
        /// </summary>
        /// <returns>A nice string representation of this board.</returns>
        public override string ToString() =>
            board[0, 0].ToShortString() +
            board[0, 1].ToShortString() +
            board[0, 2].ToShortString() +
            board[1, 0].ToShortString() +
            board[1, 1].ToShortString() +
            board[1, 2].ToShortString() +
            board[2, 0].ToShortString() +
            board[2, 1].ToShortString() +
            board[2, 2].ToShortString();
    }
}
