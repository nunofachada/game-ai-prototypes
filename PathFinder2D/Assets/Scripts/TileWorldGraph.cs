using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibGameAI.PathFinding;
using static World;

public class TileWorldGraph : IGraph
{
    private TileTypeBehaviour[,] world;
    public IEnumerable<IConnection> GetConnections(int fromNode)
    {
        Vector2Int node = Ind2Vec(fromNode, world.GetLength(0));
        if (world[node.x, node.y].TileType != TileTypeEnum.Blocked)
        {
            Vector2Int up, down, left, right;
            int xdim = world.GetLength(0);

            up = node + Vector2Int.up;
            if (up.y < world.GetLength(1)
                    && world[up.x, up.y].TileType != TileTypeEnum.Blocked)
            {
                yield return new Connection(1f, fromNode, Vec2Ind(up, xdim));
            }

            down = node + Vector2Int.down;
            if (down.y >= 0
                    && world[down.x, down.y].TileType != TileTypeEnum.Blocked)
            {
                yield return new Connection(1f, fromNode, Vec2Ind(down, xdim));
            }

            left = node + Vector2Int.left;
            if (left.x >= 0
                    && world[left.x, left.y].TileType != TileTypeEnum.Blocked)
            {
                yield return new Connection(1f, fromNode, Vec2Ind(left, xdim));
            }

            right = node + Vector2Int.right;
            if (right.x < world.GetLength(0)
                    && world[right.x, right.y].TileType != TileTypeEnum.Blocked)
            {
                yield return new Connection(1f, fromNode, Vec2Ind(right, xdim));
            }
        }
    }

    public TileWorldGraph(TileTypeBehaviour[,] world)
    {
        this.world = world;
    }
}
