/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

namespace LibGameAI.PathFinding
{
    public struct Connection : IConnection
    {
        public float Cost { get; }
        public int FromNode { get; }
        public int ToNode { get; }

        public Connection(float cost, int fromNode, int toNode)
        {
            Cost = cost;
            FromNode = fromNode;
            ToNode = toNode;
        }
    }
}
