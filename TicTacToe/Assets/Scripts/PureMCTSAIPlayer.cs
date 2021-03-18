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
        private const float timeToThink = 10f;
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

            KeyValuePair<Vector2Int, MCTSBoard> current =
                new KeyValuePair<Vector2Int, MCTSBoard>(Board.NoMove, board);

            int testedMoves;

            DateTime start = DateTime.Now;

            moveSequence.Push(board);

            // 1. Selection
            while (current.Value.FullyExplored && !current.Value.Status().HasValue)
            {
                MCTSBoard prevCurrent = current.Value;
                current = SelectMovePolicy(current.Value);
                if (current.Value == prevCurrent)
                    break;
                moveSequence.Push(current.Value);

                // if (DateTime.Now > start + TimeSpan.FromSeconds(1))
                // {
                //     Debug.Log($"Board is {board}, current is {current.Value}");
                //     UnityEditor.EditorApplication.isPlaying = false;
                //     return false;
                // }
            }

            // 2. Expansion
            current = ChooseUnexploredMovePolicy(current.Value);
            if (current.Value != board)
                moveSequence.Push(current.Value);

            // 3. Simulation
            result = PlayoutPolicy(current.Value);

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
            List<Vector2Int> availableMoves = new List<Vector2Int>();
            CellState turn = board.Turn;
            CellState? currentStatus = board.Status();

            // No need to playout anything if this is a final board
            if (currentStatus.HasValue) return currentStatus.Value;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2Int pos = new Vector2Int(i, j);
                    if (board.GetStateAt(pos) == CellState.Undecided)
                    {
                        availableMoves.Add(pos);
                    }
                }
            }

            do
            {
                int index = UnityEngine.Random.Range(0, availableMoves.Count);
                Vector2Int pos = availableMoves[index];
                availableMoves.RemoveAt(index);
                boardCopy.SetStateAt(pos, turn);
                turn = turn.Other();
            }
            while (!boardCopy.Status().HasValue);

            return boardCopy.Status().Value;
        }

        private KeyValuePair<Vector2Int, MCTSBoard> SelectMovePolicy(MCTSBoard board)
        {
            float lnN = Mathf.Log(board.Playouts);
            KeyValuePair<Vector2Int, MCTSBoard> bestMove =
                new KeyValuePair<Vector2Int, MCTSBoard>(Board.NoMove, board);
            float bestUCT = 0;
            foreach (KeyValuePair<Vector2Int, MCTSBoard> childMove in board.ChildMoves)
            {
                float uct = childMove.Value.Wins / (float)childMove.Value.Playouts
                    + k * Mathf.Sqrt(lnN / childMove.Value.Playouts);
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

            foreach (KeyValuePair<Vector2Int, MCTSBoard> childMove in root.ChildMoves)
            {
                float winPlayoutRatio = childMove.Value.Wins / (float)childMove.Value.Playouts;
                if (winPlayoutRatio >= maxWinPlayoutRatio)
                {
                    maxWinPlayoutRatio = winPlayoutRatio;
                    bestMove = childMove.Key;
                }
                log.AppendFormat("Move {0} has ratio {1} ({2} / {3})\n",
                    childMove.Key, winPlayoutRatio, childMove.Value.Wins,
                    childMove.Value.Playouts);
            }

            log.Insert(
                0, $"Select move {bestMove} with ratio {maxWinPlayoutRatio}\n");

            Debug.Log(log.ToString());
            return bestMove;
        }

        private KeyValuePair<Vector2Int, MCTSBoard> ChooseUnexploredMovePolicy(MCTSBoard board)
        {
            List<KeyValuePair<Vector2Int, MCTSBoard>> unexplored
                = new List<KeyValuePair<Vector2Int, MCTSBoard>>();

            foreach (KeyValuePair<Vector2Int, MCTSBoard> childMove in board.ChildMoves)
            {
                if (childMove.Value.Playouts == 0)
                {
                    unexplored.Add(childMove);
                }
            }

            if (unexplored.Count > 0)
                return unexplored[UnityEngine.Random.Range(0, unexplored.Count)];
            else
                return new KeyValuePair<Vector2Int, MCTSBoard>(Board.NoMove, board);
        }
    }
}