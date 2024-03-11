/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using UnityEngine;
using LibGameAI.PathFinding;

namespace GameAIPrototypes.PathFinder2D
{
    using static World;

    // Graph implementation for a simple tile-based world
    public class TileWorldGraph : IGraph
    {
        // The tile-based world
        private readonly TileBehaviour[,] world;

        // Get outgoing connections from given node
        public IEnumerable<IConnection> GetConnections(int fromNode)
        {
            // Convert node to tile-world position
            Vector2Int node = Ind2Vec(fromNode, world.GetLength(0));

            // If the position is not blocked, it's a node with possible outgoing
            // connections
            if (world[node.x, node.y].TileType != TileTypeEnum.Blocked)
            {
                // Possible destination nodes
                Vector2Int up, down, left, right;

                // Horizontal world position
                int xdim = world.GetLength(0);

                // Determine node above
                up = node + Vector2Int.up;
                // Is node within world limits and unblocked?
                if (up.y < world.GetLength(1)
                        && world[up.x, up.y].TileType != TileTypeEnum.Blocked)
                {
                    // If so, yield return connection to this node
                    yield return new Connection(1f, fromNode, Vec2Ind(up, xdim));
                }

                // Determine node below
                down = node + Vector2Int.down;
                // Is node within world limits and unblocked?
                if (down.y >= 0
                        && world[down.x, down.y].TileType != TileTypeEnum.Blocked)
                {
                    // If so, yield return connection to this node
                    yield return new Connection(1f, fromNode, Vec2Ind(down, xdim));
                }

                // Determine left node
                left = node + Vector2Int.left;
                // Is node within world limits and unblocked?
                if (left.x >= 0
                        && world[left.x, left.y].TileType != TileTypeEnum.Blocked)
                {
                    // If so, yield return connection to this node
                    yield return new Connection(1f, fromNode, Vec2Ind(left, xdim));
                }

                // Determine right node
                right = node + Vector2Int.right;
                // Is node within world limits and unblocked?
                if (right.x < world.GetLength(0)
                        && world[right.x, right.y].TileType != TileTypeEnum.Blocked)
                {
                    // If so, yield return connection to this node
                    yield return new Connection(1f, fromNode, Vec2Ind(right, xdim));
                }
            }
        }

        // Constructor
        public TileWorldGraph(TileBehaviour[,] world)
        {
            this.world = world;
        }
    }
}