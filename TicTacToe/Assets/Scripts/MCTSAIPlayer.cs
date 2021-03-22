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
    /// <summary>
    /// A basic MCTS player.
    /// </summary>
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

            // Create the root node using the current table
            TicTacToeMCTSNode root = new TicTacToeMCTSNode(gameBoard, Board.NoMove);

            // The node to be selected for play
            TicTacToeMCTSNode selected;

            // A string builder to build our MCTS log
            StringBuilder sb = new StringBuilder();

            // Number of simulations performed
            int simulations = 0;

            // Run MCTS and keep improving statistics while we have time
            while (DateTime.Now < deadline)
            {
                MCTS(root);
            }

            // Get the best move, i.e. the one with a higher win ratio
            // (by setting k = 0)
            selected = SelectMovePolicy(root, 0);

            // Build our debug log and count number of simulations performed
            foreach (AbstractMCTSNode<Pos, CellState> node in root.Children)
            {
                sb.AppendFormat("{0} -> {1:f4} ({2}/{3})\n",
                    node.Move,
                    node.Wins / (float)node.Playouts,
                    node.Wins,
                    node.Playouts);
                simulations += node.Playouts;
            }

            // Add summary to the beginning of log
            sb.Insert(0, string.Format(
                "Selected {0} with ratio {1} after {2} simulations\n",
                selected.Move,
                selected.Wins / (float)selected.Playouts,
                simulations));

            // Set the log variable (will be returned via ref)
            log = sb.ToString();

            // Return the selected move
            return selected.Move;
        }

        // Run an MCTS iteration
        private void MCTS(TicTacToeMCTSNode root)
        {
            // Current node is the root node
            TicTacToeMCTSNode current = root;

            // No node is initially selected in the tree policy (selection + expansion)
            bool selected = false;

            // The move sequence, so we can backpropagate results
            Stack<TicTacToeMCTSNode> moveSequence = new Stack<TicTacToeMCTSNode>();

            // The root node is the first in the sequence
            moveSequence.Push(current);

            // Tree policy (selection + expansion), to be performed while the
            // current node is not terminal AND no node is selected
            // (i.e. the loop stops if the current node becomes terminal OR a
            // node is selected)
            while (!current.IsTerminal && !selected)
            {
                // Is the current node fully expanded? (i.e. have we
                // tried/expanded all possible moves?)
                if (current.IsFullyExpanded)
                {
                    // Then the "new" current node will be selected among the
                    // children of the "current" current node
                    current = SelectMovePolicy(current, k);
                }
                else
                {
                    // Otherwise let's expand one of the currently untried
                    // moves and select one of its children as the current node
                    current = ExpandPolicy(current);
                    selected = true;
                }

                // Add another node to the sequence
                moveSequence.Push(current);
            }

            // Perform a playout / rollout from the current node until the end
            // of the game and obtain the result
            CellState result = current.Playout(PlayoutPolicy);

            // Backpropagate the result along the move sequence
            while (moveSequence.Count > 0)
            {
                // Pop the top node in the sequence
                TicTacToeMCTSNode node = moveSequence.Pop();

                // Increment its number of playouts
                node.Playouts++;

                // Update the win/lose count according whose turn it was to play
                // in the previous turn
                if (result == node.Turn.Other()) node.Wins++;
                else if (result == node.Turn) node.Wins--;
            }
        }

        // Policy to select a move among the children of the given node
        // k is the balance between choosing the most successfully simulated
        // child nodes vs the most unexplored child nodes
        // The higher the k the more weight we put on exploring less explored
        // nodes
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

        // The expand policy determines how we choose a yet untried node among
        // the existing untried nodes
        // An untried node represents a move that was never part of a move
        // sequence
        // The expand policy in this MCTS implementation is to randomly select
        // an untried node
        private TicTacToeMCTSNode ExpandPolicy(TicTacToeMCTSNode node)
        {
            // Get the currently untried moves
            IReadOnlyList<Pos> untriedMoves = node.UntriedMoves;

            // Randomly select one of the untried moves
            Pos move = untriedMoves[random.Next(untriedMoves.Count)];

            // Make the untried move and return the respective node
            return node.MakeMove(move) as TicTacToeMCTSNode;
        }

        // The playout policy determines how to select a child node during a
        // playout/simulation
        // The playout policy in this MCTS implementation is to randomly
        // select a child node
        private Pos PlayoutPolicy(IList<Pos> list)
        {
            System.Diagnostics.Debug.Assert(list.Count > 0);
            return list[random.Next(list.Count)];
        }
    }
}