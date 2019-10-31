/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

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
