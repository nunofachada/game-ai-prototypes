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
    /// A concrete MCTS node for the Tic Tac Toe game.
    /// </summary>
    public class TicTacToeMCTSNode : AbstractMCTSNode<Pos, CellState>
    {
        // Reference to the current Tic Tac Toe board
        private Board board;

        // List of valid moves
        private IList<Pos> validMoves;

        // Is this a terminal node?
        public override bool IsTerminal => board.Status().HasValue;

        // Whose turn is it?
        public override CellState Turn => board.Turn;

        // Implementation of hook property which provides the base class access
        // to the concrete valid moves
        protected override IEnumerable<Pos> ValidMoves
        {
            get
            {
                if (validMoves == null)
                {
                    validMoves = ValidMovesFromBoard(board);
                }
                return validMoves;
            }
        }

        // Node constructor
        public TicTacToeMCTSNode(Board board, Pos move) : base(move)
        {
            this.board = board;
        }

        // Implementation of hook method which performs the move on the actual
        // Tic Tac Toe game board
        protected override AbstractMCTSNode<Pos, CellState> DoMakeMove(Pos move)
        {
            // Get a copy of the game board
            Board boardCopy = board.Copy();

            // Perform the move
            boardCopy.DoMove(move);

            // Return a new node representing the new state of the board
            return new TicTacToeMCTSNode(boardCopy, move);
        }

        // Perform a playout / simulation using the given strategy
        public override CellState Playout(Func<IList<Pos>, Pos> strategy)
        {
            // Do the playout on a copy of the board
            Board boardCopy = board.Copy();

            // Keep playing using the given strategy until an endgame is reached
            while (!boardCopy.Status().HasValue)
            {
                // Use the given strategy to obtain a move
                Pos move = strategy(ValidMovesFromBoard(boardCopy));

                // Perform move
                boardCopy.DoMove(move);
            }

            return boardCopy.Status().Value;
        }

        // Helper method which obtains valid moves for the given board
        private static IList<Pos> ValidMovesFromBoard(Board board)
        {
            // List of valid moves
            List<Pos> validMoves = new List<Pos>();

            // Search for valid moves
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Is current position a valid move?
                    Pos move = new Pos(i, j);
                    if (board.GetStateAt(move) == CellState.Undecided)
                    {
                        // If so, add it to list of valid moves
                        validMoves.Add(move);
                    }
                }
            }
            return validMoves;
        }
    }
}