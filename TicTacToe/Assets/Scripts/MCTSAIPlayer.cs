/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Text;
using System.Collections.Generic;

namespace AIUnityExamples.TicTacToe
{
    public class MCTSAIPlayer : IPlayer
    {
        // Time I can take to play in seconds
        private const float timeToThink = 0.5f;

        // Balance between exploitation and exploration
        private readonly float k = 2 / (float)Math.Sqrt(2);

        // Random number generator
        private Random random;

        // Create an instance of an MCTSAIPlayer
        public MCTSAIPlayer()
        {
            random = new Random();
        }

        // Play a turn
        public Pos Play(Board gameBoard, ref string log)
        {
            // Keep start time
            DateTime startTime = DateTime.Now;

            // What is my deadline?
            DateTime deadline = startTime + TimeSpan.FromSeconds(timeToThink);

            TicTacToeMCTSNode root = new TicTacToeMCTSNode(gameBoard, Board.NoMove);

            TicTacToeMCTSNode selected;

            StringBuilder sb = new StringBuilder();

            int simulations = 0;

            // Keep improving statistics while we have time
            while (DateTime.Now < deadline)
            {
                MCTS(root);
            }

            selected = SelectMovePolicy(root, 0);

            foreach (AbstractMCTSNode<Pos, CellState> node in root.Children)
            {
                sb.AppendFormat("{0} -> {1:f4} ({2}/{3})\n",
                    node.Move,
                    node.Wins / (float)node.Playouts,
                    node.Wins,
                    node.Playouts);
                simulations += node.Playouts;
            }

            sb.Insert(0, string.Format(
                "Selected {0} with ratio {1} after {2} simulations\n",
                selected.Move,
                selected.Wins / (float)selected.Playouts,
                simulations));

            log = sb.ToString();

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
            float lnN = (float)Math.Log(node.Playouts);
            TicTacToeMCTSNode bestChild = null;
            float bestUCT = float.NegativeInfinity;
            foreach (AbstractMCTSNode<Pos, CellState> childNode in node.Children)
            {
                float uct = childNode.Wins / (float)childNode.Playouts
                    + k * (float)Math.Sqrt(lnN / childNode.Playouts);
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

            Pos move = untriedMoves[random.Next(untriedMoves.Count)];

            TicTacToeMCTSNode childNode = node.MakeMove(move) as TicTacToeMCTSNode;

            return childNode;
        }
        private Pos PlayoutPolicy(IList<Pos> list)
        {
            System.Diagnostics.Debug.Assert(list.Count > 0);
            return list[random.Next(list.Count)];
        }
    }
}