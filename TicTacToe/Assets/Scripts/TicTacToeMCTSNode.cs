/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExamples.TicTacToe
{
    public class TicTacToeMCTSNode : AbstractMCTSNode<Vector2Int, CellState>
    {
        private Board board;

        private IList<Vector2Int> validMoves;

        public override bool IsTerminal => board.Status().HasValue;

        public override CellState Result => board.Status().Value;

        public override CellState Turn { get; }

        protected override IEnumerable<Vector2Int> ValidMoves
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

        private IList<Vector2Int> ValidMovesFromBoard(Board board)
        {
            List<Vector2Int> validMoves = new List<Vector2Int>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2Int move = new Vector2Int(i, j);
                    if (board.GetStateAt(move) == CellState.Undecided)
                    {
                        validMoves.Add(move);
                    }
                }
            }
            return validMoves;
        }

        public TicTacToeMCTSNode(Board board, Vector2Int move) : base(move)
        {
            this.board = board;

            int turnInt = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2Int pos = new Vector2Int(i, j);
                    CellState piece = board.GetStateAt(pos);
                    if (piece == CellState.X) turnInt++;
                    else if (piece == CellState.O) turnInt--;
                }
            }
            Turn = turnInt == 0 ? CellState.X : CellState.O;
        }

        protected override AbstractMCTSNode<Vector2Int, CellState> DoMakeMove(Vector2Int move)
        {
            (Board board, CellState turn) bt = CopyBoard(this.board);
            bt.board.SetStateAt(move, bt.turn);

            TicTacToeMCTSNode childNode = new TicTacToeMCTSNode(bt.board, move);

            return childNode;
        }

        public override CellState Playout(Func<IList<Vector2Int>, Vector2Int> strategy)
        {
            (Board board, CellState turn) bt = CopyBoard(this.board);

            while (!bt.board.Status().HasValue)
            {
                Vector2Int move = strategy(ValidMovesFromBoard(bt.board));
                bt.board.SetStateAt(move, bt.turn);
                bt.turn = bt.turn.Other();
            }

            return bt.board.Status().Value;
        }

        private static (Board, CellState) CopyBoard(Board board)
        {
            Board boardCopy = new Board();
            CellState turn;
            int turnInt = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2Int pos = new Vector2Int(i, j);
                    CellState piece = board.GetStateAt(pos);
                    boardCopy.SetStateAt(pos, piece);
                    if (piece == CellState.X) turnInt++;
                    else if (piece == CellState.O) turnInt--;
                }
            }
            turn = turnInt == 0 ? CellState.X : CellState.O;

            return (boardCopy, turn);
        }

    }
}