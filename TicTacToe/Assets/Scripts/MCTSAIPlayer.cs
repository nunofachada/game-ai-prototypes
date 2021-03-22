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
    public class MCTSAIPlayer : IPlayer
    {
        // Time I can take to play in seconds
        private const float timeToThink = 1f;
        // Balance between exploitation and exploration
        private readonly float k = 2 / Mathf.Sqrt(2);

        // Play a turn
        public Pos Play(Board gameBoard, CellState turn)
        {
            // Keep start time
            DateTime startTime = DateTime.Now;

            // What is my deadline?
            DateTime deadline = startTime + TimeSpan.FromSeconds(timeToThink);

            TicTacToeMCTSNode root = new TicTacToeMCTSNode(gameBoard, Board.NoMove);

            StringBuilder sb = new StringBuilder();

            // Keep improving statistics while we have time
            while (DateTime.Now < deadline)
            {
                MCTS(root);
            }

            sb.AppendFormat("MCTS took {0} ms", (DateTime.Now - startTime).TotalMilliseconds);

            foreach (AbstractMCTSNode<Pos, CellState> node in root.Children)
            {
                sb.AppendFormat("{0} -> {1:f4} ({2}/{3})\n",
                    node.Move, node.Wins / (float)node.Playouts, node.Wins, node.Playouts);
            }

            TicTacToeMCTSNode selected = SelectMovePolicy(root, 0);

            sb.AppendFormat("=== Selected {0} ===", selected.Move);

            Debug.Log(sb.ToString());

            return selected.Move;
        }

        private void MCTS(TicTacToeMCTSNode root)
        {
            TicTacToeMCTSNode current = root;
            bool selected = false;
            Stack<TicTacToeMCTSNode> moveSequence = new Stack<TicTacToeMCTSNode>();

            moveSequence.Push(current);

            // Tree policy
            while (!current.IsTerminal && !selected)
            {
                if (current.IsFullyExpanded)
                {
                    current = SelectMovePolicy(current, k);
                }
                else
                {
                    current = ExpandPolicy(current);
                    selected = true;
                }
                moveSequence.Push(current);
            }

            // Playout / rollout
            CellState result = current.Playout(PlayoutPolicy);

            // Backpropagate
            while (moveSequence.Count > 0)
            {
                TicTacToeMCTSNode node = moveSequence.Pop();
                node.Playouts++;
                if (result == node.Turn) node.Wins--;
                else if (result == node.Turn.Other()) node.Wins++;
            }

        }

        private TicTacToeMCTSNode SelectMovePolicy(TicTacToeMCTSNode node, float k)
        {
            float lnN = Mathf.Log(node.Playouts);
            TicTacToeMCTSNode bestChild = null;
            float bestUCT = float.NegativeInfinity;
            foreach (AbstractMCTSNode<Pos, CellState> childNode in node.Children)
            {
                float uct = childNode.Wins / (float)childNode.Playouts
                    + k * Mathf.Sqrt(lnN / childNode.Playouts);
                if (uct > bestUCT)
                {
                    bestUCT = uct;
                    bestChild = childNode as TicTacToeMCTSNode;
                }
            }
            return bestChild;
        }

        private TicTacToeMCTSNode ExpandPolicy(TicTacToeMCTSNode node)
        {
            IReadOnlyList<Pos> untriedMoves = node.UntriedMoves;

            Pos move = untriedMoves[UnityEngine.Random.Range(0, untriedMoves.Count)];

            TicTacToeMCTSNode childNode = node.MakeMove(move) as TicTacToeMCTSNode;

            return childNode;
        }
        private Pos PlayoutPolicy(IList<Pos> list)
        {
            UnityEngine.Assertions.Assert.IsTrue(list.Count > 0);
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}