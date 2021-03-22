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
    public abstract class AbstractMCTSNode<M, R>
    {
        private List<M> untriedMoves;

        private List<AbstractMCTSNode<M, R>> children;

        public abstract bool IsTerminal { get; }

        public abstract R Result { get; }

        public abstract R Turn { get; }

        public bool IsFullyExpanded => UntriedMoves.Count == 0;

        public M Move { get; }

        public int Wins { get; set; }

        public int Playouts { get; set; }

        public IEnumerable<AbstractMCTSNode<M, R>> Children
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsFullyExpanded);
                System.Diagnostics.Debug.Assert(!IsTerminal);
                return children;
            }
        }

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

        protected abstract IEnumerable<M> ValidMoves { get; }

        protected abstract AbstractMCTSNode<M, R> DoMakeMove(M move);

        public abstract R Playout(Func<IList<M>, M> strategy);

        public AbstractMCTSNode(M move)
        {
            Move = move;
            children = new List<AbstractMCTSNode<M, R>>();
        }

        public AbstractMCTSNode<M, R> MakeMove(M move)
        {
            System.Diagnostics.Debug.Assert(untriedMoves.Contains(move));
            AbstractMCTSNode<M, R> child = DoMakeMove(move);
            children.Add(child);
            untriedMoves.Remove(move);
            return child;
        }
    }
}