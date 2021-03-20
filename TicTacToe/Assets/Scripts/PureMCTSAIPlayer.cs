/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExamples.TicTacToe
{
    public class PureMCTSAIPlayer : IPlayer
    {
        // Time I can take to play in seconds
        private const float timeToThink = 1f;
        // Balance between exploitation and exploration
        private readonly float k = 2 / Mathf.Sqrt(2);

        // Play a turn
        public Vector2Int Play(Board gameBoard, CellState turn)
        {
            // Keep start time
            DateTime startTime = DateTime.Now;

            // What is my deadline?
            DateTime deadline = startTime + TimeSpan.FromSeconds(timeToThink);

            MCTSBoard root = new MCTSBoard(gameBoard);

            // Keep improving statistics while we have time
            while (DateTime.Now < deadline && MCTS(root));

            Debug.LogFormat("MCTS took {0} ms",
                (DateTime.Now - startTime).TotalMilliseconds);

            return BestMovePolicy(root);
        }

        private bool MCTS(MCTSBoard board)
        {
            CellState result;

            Stack<MCTSBoard> moveSequence = new Stack<MCTSBoard>();

            (Vector2Int move, MCTSBoard board) current = (Board.NoMove, board);

            int testedMoves;

            DateTime start = DateTime.Now;

            moveSequence.Push(board);

            // 1. Selection
            while (current.board.FullyExplored && !current.board.Status().HasValue)
            {
                MCTSBoard prevCurrent = current.board;
                current = SelectMovePolicy(current.board);
                if (current.board == prevCurrent)
                    break;
                moveSequence.Push(current.board);

                // if (DateTime.Now > start + TimeSpan.FromSeconds(1))
                // {
                //     Debug.Log($"Board is {board}, current is {current.Value}");
                //     UnityEditor.EditorApplication.isPlaying = false;
                //     return false;
                // }
            }

            // 2. Expansion
            current = ChooseUnexploredMovePolicy(current.board);
            if (current.board != board)
                moveSequence.Push(current.board);

            // 3. Simulation
            result = PlayoutPolicy(current.board);

            testedMoves = moveSequence.Count;

            // 4. Backpropagation
            while (moveSequence.Count > 0)
            {
                MCTSBoard move = moveSequence.Pop();
                if (result == move.Turn)
                    move.Wins++;
                move.Playouts++;
            }

            return testedMoves > 1;
        }

        private CellState PlayoutPolicy(MCTSBoard board)
        {
            MCTSBoard boardCopy = new MCTSBoard(board);
            CellState? currentStatus = board.Status();
            CellState turn = board.Turn;

            // No need to playout anything if this is a final board
            if (currentStatus.HasValue) return currentStatus.Value;

            do
            {
                IReadOnlyList<(Vector2Int move, MCTSBoard board)> childMoves =
                    board.ChildMoves;
                Vector2Int move =
                    childMoves[UnityEngine.Random.Range(0, childMoves.Count)].move;
                boardCopy.SetStateAt(move, turn);
                turn = turn.Other();
            }
            while (!boardCopy.Status().HasValue);

            return boardCopy.Status().Value;
        }

        private (Vector2Int move, MCTSBoard board) SelectMovePolicy(MCTSBoard board)
        {
            float lnN = Mathf.Log(board.Playouts);
            (Vector2Int, MCTSBoard) bestMove = (Board.NoMove, board);
            float bestUCT = 0;
            foreach ((Vector2Int move, MCTSBoard board) childMove in board.ChildMoves)
            {
                float uct = childMove.board.Wins / (float)childMove.board.Playouts
                    + k * Mathf.Sqrt(lnN / childMove.board.Playouts);
                if (uct > bestUCT)
                {
                    bestUCT = uct;
                    bestMove = childMove;
                }
            }
            return bestMove;
        }

        private Vector2Int BestMovePolicy(MCTSBoard root)
        {
            float maxWinPlayoutRatio = 0;
            Vector2Int bestMove = Board.NoMove;
            StringBuilder log = new StringBuilder();

            foreach ((Vector2Int move, MCTSBoard board) childMove in root.ChildMoves)
            {
                float winPlayoutRatio = childMove.board.Wins / (float)childMove.board.Playouts;
                if (winPlayoutRatio >= maxWinPlayoutRatio)
                {
                    maxWinPlayoutRatio = winPlayoutRatio;
                    bestMove = childMove.move;
                }
                log.AppendFormat("Move {0} has ratio {1} ({2} / {3})\n",
                    childMove.move, winPlayoutRatio, childMove.board.Wins,
                    childMove.board.Playouts);
            }

            log.Insert(
                0, $"Select move {bestMove} with ratio {maxWinPlayoutRatio}\n");

            Debug.Log(log.ToString());
            return bestMove;
        }

        private (Vector2Int move, MCTSBoard board) ChooseUnexploredMovePolicy(MCTSBoard board)
        {
            List<(Vector2Int move, MCTSBoard board)> unexplored
                = new List<(Vector2Int move, MCTSBoard board)>();

            foreach ((Vector2Int move, MCTSBoard board) childMove in board.ChildMoves)
            {
                if (childMove.board.Playouts == 0)
                {
                    unexplored.Add(childMove);
                }
            }

            if (unexplored.Count > 0)
                return unexplored[UnityEngine.Random.Range(0, unexplored.Count)];
            else
                return (Board.NoMove, board);
        }
    }
}