/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegamaxAIPlayer : IPlayer
{
    struct Move
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
        Move move = Negamax(gameBoard, turn);
        return move.Pos ?? Vector2Int.zero;
    }

    private Move Negamax(IBoard gameBoard, CellState turn)
    {
        if (gameBoard.Status() == null)
        {
            Move bestMove = new Move(null, int.MinValue);
            CellState proxTurn =
                turn == CellState.X ? CellState.O : CellState.X;

            // Jogo nao terminou
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2Int pos = new Vector2Int(i, j);
                    if (gameBoard.GetStateAt(pos) == CellState.Undecided)
                    {
                        Move move;

                        gameBoard.SetStateAt(pos, turn);

                        move = Negamax(gameBoard, proxTurn);

                        gameBoard.SetStateAt(pos, CellState.Undecided);

                        move.Score = -move.Score;

                        if (move.Score > bestMove.Score)
                        {
                            bestMove.Score = move.Score;
                            bestMove.Pos = pos;
                        }
                    }
                }
            }
            return bestMove;

        }
        else
        {
            // Posição final
            int score = gameBoard.Status().Value == CellState.Undecided
                ? 0 : -10;
            return new Move(null, score);
        }
    }
}
