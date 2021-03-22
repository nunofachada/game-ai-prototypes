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
    public class TicTacToeMCTSNode : AbstractMCTSNode<Pos, CellState>
    {
        private Board board;

        private IList<Pos> validMoves;

        public override bool IsTerminal => board.Status().HasValue;

        public override CellState Result => board.Status().Value;

        public override CellState Turn => board.Turn;

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

        private IList<Pos> ValidMovesFromBoard(Board board)
        {
            List<Pos> validMoves = new List<Pos>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Pos move = new Pos(i, j);
                    if (board.GetStateAt(move) == CellState.Undecided)
                    {
                        validMoves.Add(move);
                    }
                }
            }
            return validMoves;
        }

        public TicTacToeMCTSNode(Board board, Pos move) : base(move)
        {
            this.board = board;
        }

        protected override AbstractMCTSNode<Pos, CellState> DoMakeMove(Pos move)
        {
            Board boardCopy = board.Copy();
            boardCopy.DoMove(move);

            return new TicTacToeMCTSNode(boardCopy, move);
        }

        public override CellState Playout(Func<IList<Pos>, Pos> strategy)
        {
            Board boardCopy = board.Copy();

            while (!boardCopy.Status().HasValue)
            {
                Pos move = strategy(ValidMovesFromBoard(boardCopy));
                boardCopy.DoMove(move);
            }

            return boardCopy.Status().Value;
        }
    }
}