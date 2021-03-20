/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExamples.TicTacToe
{
    public class MCTSBoard : Board
    {
        private List<(Vector2Int, MCTSBoard)> childMoves;

        public CellState Turn { get; private set; }

        public IReadOnlyList<(Vector2Int, MCTSBoard)> ChildMoves
        {
            get
            {
                if (childMoves == null)
                {
                    CellState nextTurn = Turn.Other();
                    childMoves = new List<(Vector2Int, MCTSBoard)>();
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Vector2Int move = new Vector2Int(i, j);
                            if (GetStateAt(move) == CellState.Undecided)
                            {
                                MCTSBoard boardWithMove = new MCTSBoard(this);
                                boardWithMove.SetStateAt(move, nextTurn);
                                boardWithMove.Turn = nextTurn;
                                childMoves.Add((move, boardWithMove));
                            }
                        }
                    }
                }
                return childMoves;
            }
        }

        public int Wins { get; set; }
        public int Playouts { get; set; }

        public bool FullyExplored
        {
            get
            {
                foreach ((Vector2Int move, MCTSBoard board) movBoard in ChildMoves)
                {
                    if (movBoard.board.Playouts == 0) return false;
                }
                return true;
            }
        }

        public MCTSBoard(Board board)
        {
            int intTurn = (int)CellState.X;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    CellState piece = board.GetStateAt(new Vector2Int(i, j));
                    if (piece == CellState.X) intTurn++;
                    else if (piece == CellState.O) intTurn--;

                    SetStateAt(new Vector2Int(i, j), piece);
                }
            }

            Turn = (CellState)intTurn;
        }

        public override string ToString() =>
            GetStateAt(new Vector2Int(0, 0)).ToShortString() +
            GetStateAt(new Vector2Int(0, 1)).ToShortString() +
            GetStateAt(new Vector2Int(0, 2)).ToShortString() +
            GetStateAt(new Vector2Int(1, 0)).ToShortString() +
            GetStateAt(new Vector2Int(1, 1)).ToShortString() +
            GetStateAt(new Vector2Int(1, 2)).ToShortString() +
            GetStateAt(new Vector2Int(2, 0)).ToShortString() +
            GetStateAt(new Vector2Int(2, 1)).ToShortString() +
            GetStateAt(new Vector2Int(2, 2)).ToShortString();
    }
}

