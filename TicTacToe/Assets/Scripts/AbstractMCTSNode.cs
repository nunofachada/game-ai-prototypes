/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

namespace GameAIPrototypes.TicTacToe
{
    /// <summary>
    /// Represents an abstract MCTS node.
    /// </summary>
    /// <typeparam name="M">
    /// Type representing board moves or positions.
    /// </typeparam>
    /// <typeparam name="R">
    /// Type representing game results and/or whose turn is to play.
    /// </typeparam>
    public abstract class AbstractMCTSNode<M, R>
    {
        // List of untried moves (i.e. moves that were never part of a move
        // sequence)
        private List<M> untriedMoves;

        // Children of this node
        private List<AbstractMCTSNode<M, R>> children;

        // Is this node terminal?
        public abstract bool IsTerminal { get; }

        // Whose turn is to play in this node?
        public abstract R Turn { get; }

        // Is the node fully expanded? (i.e., were all children part of a move
        // sequence previously?)
        public bool IsFullyExpanded => UntriedMoves.Count == 0;

        // The move that led to this node
        public M Move { get; }

        // Number of wins minus losses for the player that made the move leading
        // to this node in all performed playouts / simulations
        public int Wins { get; set; }

        // Number of playouts / simulations performed under this node
        public int Playouts { get; set; }

        // Children nodes of the current node
        public IEnumerable<AbstractMCTSNode<M, R>> Children
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsFullyExpanded);
                System.Diagnostics.Debug.Assert(!IsTerminal);
                return children;
            }
        }

        // Public accessor for the untried moves in this node
        public IReadOnlyList<M> UntriedMoves
        {
            get
            {
                if (untriedMoves is null)
                {
                    untriedMoves = new List<M>(ValidMoves);
                }
                return untriedMoves;
            }
        }

        // Valid moves available to this node
        // This is a hook property which must be overridden by derived classes
        protected abstract IEnumerable<M> ValidMoves { get; }

        // Make a move in this node and return the resulting child node
        // This is a hook method which must be overridden by derived classes
        protected abstract AbstractMCTSNode<M, R> DoMakeMove(M move);

        // Perform a playout / simulation using the given strategy
        public abstract R Playout(Func<IList<M>, M> strategy);

        // Constructor, to be invoked by derived classes
        public AbstractMCTSNode(M move)
        {
            Move = move;
            children = new List<AbstractMCTSNode<M, R>>();
        }

        // Make a move in this node and return the resulting child node
        // MCTS calls this method, derived classes should override DoMakeMove()
        // to perform the actual move
        public AbstractMCTSNode<M, R> MakeMove(M move)
        {
            // The move must be in the untriedMoves list, otherwise it's a bug
            System.Diagnostics.Debug.Assert(untriedMoves.Contains(move));

            // Call the hook method, to be implemented by derived classes and
            // get the resulting child node
            AbstractMCTSNode<M, R> child = DoMakeMove(move);

            // Add the child node to the list of child nodes
            children.Add(child);

            // Remove the move from the list of untried moves
            untriedMoves.Remove(move);

            // Return the child node which results of performing this move
            return child;
        }
    }
}