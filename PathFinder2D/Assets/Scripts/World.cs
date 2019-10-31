using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibGameAI.PathFinding;

public class World : MonoBehaviour
{
    [SerializeField] private GameObject emptyTile = null;
    [SerializeField] private GameObject blockedTile = null;
    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject goal = null;

    [SerializeField] private Vector2Int worldSize = new Vector2Int(10, 10);
    [SerializeField] [Range(0f, 1f)] private float passageRatio = 0.1f;

    // How long between each movement
    [SerializeField] private float moveDuration = 0.5f;

    // Player and goal positions
    private Vector2Int playerPos;
    private Vector2Int goalPos;

    // Goal reached?
    private bool goalReached = false;

    // Current path
    private IEnumerable<IConnection> path;

    // Matrix of game world state
    private TileTypeBehaviour[,] world;

    // Offset for all tiles
    private Vector2 offset;

    private void Awake()
    {

        // Array of game objects to clone for each column
        GameObject[] columnToClone = new GameObject[worldSize.y];

        // References to camera game object and camera component
        GameObject cameraGameObj = GameObject.FindWithTag("MainCamera");
        Camera cameraComponent = cameraGameObj.GetComponent<Camera>();

        // Width must be odd
        if (worldSize.x % 2 == 0) worldSize.x++;

        // Adjust camera to game world
        cameraComponent.orthographicSize = Mathf.Max(worldSize.x, worldSize.y) / 2;

        // Initialize matrix of game world elements (by default will be
        // all empties)
        world = new TileTypeBehaviour[worldSize.x, worldSize.y];

        // Determine tile offsets
        offset = new Vector2(worldSize.x / 2f - 0.5f, worldSize.y / 2f - 0.5f);

        // Cycle through columns
        for (int i = 0; i < worldSize.x; i++)
        {
            // Number of passages in current column (by default all)
            int numEmpty = worldSize.y;

            // For current column determine which tiles are empty and which are
            // blocked
            for (int j = 0; j < worldSize.y; j++)
            {
                // By default, current position in column is empty
                columnToClone[j] = emptyTile;

                // However, if column is odd...
                if (i % 2 != 0)
                {
                    // ...and of by chance it's supposed to have a blocked tile
                    if (Random.value > passageRatio)
                    {
                        // ...then set a blocked tile
                        columnToClone[j] = blockedTile;

                        // Now we have on less empty space
                        numEmpty--;
                    }
                }
            }

            // Are there any empty tiles in current column?
            if (numEmpty == 0)
            {
                // If not, then randomly define one empty tile
                columnToClone[Random.Range(0, worldSize.y)] = emptyTile;
            }

            // Instantiate tiles for current column
            for (int j = 0; j < worldSize.y; j++)
            {
                GameObject tile = Instantiate(columnToClone[j], transform);
                tile.transform.position =
                    new Vector3(i - offset.x, j - offset.y, 0);
                world[i, j] = tile.GetComponent<TileTypeBehaviour>();
            }
        }

        // Place player anywhere in the world, as long as it's an empty tile
        while (true)
        {
            playerPos = new Vector2Int(
                Random.Range(0, worldSize.x), Random.Range(0, worldSize.y));
            if (world[playerPos.x, playerPos.y].TileType == TileTypeEnum.Empty)
            {
                player = Instantiate(player);
                player.transform.position = new Vector3(
                    playerPos.x - offset.x, playerPos.y - offset.y, -2);
                break;
            }
        }

        Debug.Log($"Player placed at {playerPos}");

        // Place goal anywhere in the world, as long as it's an empty tile
        while (true)
        {
            goalPos = new Vector2Int(
                Random.Range(0, worldSize.x), Random.Range(0, worldSize.y));
            if (world[goalPos.x, goalPos.y].TileType == TileTypeEnum.Empty)
            {
                goal = Instantiate(goal);
                goal.transform.position = new Vector3(
                    goalPos.x - offset.x, goalPos.y - offset.y, -1);
                break;
            }
        }

        Debug.Log($"Goal placed at {goalPos}");
    }

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(FindPath());
    }

    private class TileWorldGraph : IGraph
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

    // This co-routine performs the path finding and invokes another coroutine
    // to perform player movement animation
    private IEnumerator FindPath()
    {
        // Coroutine will be called again in moveDuration seconds
        YieldInstruction wfs = new WaitForSeconds(moveDuration);

        // The graph representation of our tile world
        TileWorldGraph graph = new TileWorldGraph(world);

        // Start player movement loop
        while (playerPos != goalPos)
        {
            // Perform path finding
            path = Dijkstra.GetShortestPath(
                graph,
                Vec2Ind(playerPos, world.GetLength(0)),
                Vec2Ind(goalPos, world.GetLength(0)));

            // Get an enumerator for the connection enumerable (it must be
            // disposed of, as such we put it in a using block)
            using (IEnumerator<IConnection> conns = path.GetEnumerator())
            {
                // Did the path finder return any connection at all?
                if (conns.MoveNext())
                {
                    // If so, move player towards the destination node in the
                    // first connection
                    StartCoroutine(MovePlayer(
                        conns.Current.ToNode, moveDuration / 2));
                }
            }

            // Return again after some fixed amount of time
            yield return wfs;
        }

        // If we got here, it means the goal was reached!
        goalReached = true;
    }

    // Co-routine that performs the player movement animation
    private IEnumerator MovePlayer(int toNode, float duration)
    {
        Vector3 finalPos;
        Vector3 initPos = player.transform.position;
        float startTime = Time.time;
        float endTime = startTime + duration;

        playerPos = Ind2Vec(toNode, world.GetLength(0));

        finalPos = new Vector3(
            playerPos.x - offset.x,
            playerPos.y - offset.y,
            player.transform.position.z);

        while (Time.time < endTime)
        {
            float currTime = Time.time;
            float toMove = Mathf.InverseLerp(startTime, endTime, currTime);
            Vector3 newPos = Vector3.Lerp(initPos, finalPos, toMove);
            player.transform.position = newPos;
            yield return null;
        }

        player.transform.position = finalPos;
    }

    // Draw a message when the goal is reached
    private void OnGUI()
    {
        if (goalReached)
        {
            GUI.contentColor = Color.yellow;
            GUI.Label(new Rect(20, 20, 200, 50), "Path found!");
        }
    }

    // Draw gizmos around player, goal and along found path
    private void OnDrawGizmos()
    {
        // Player gizmo
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(player.transform.position, new Vector3(1, 1, 1));

        // Goal gizmo
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(goal.transform.position, new Vector3(1, 1, 1));

        // Path gizmo
    }

    private static Vector2Int Ind2Vec(int index, int xdim) =>
        new Vector2Int(index % xdim, index / xdim);
    private static int Vec2Ind(Vector2Int vec, int xdim) =>
        vec.y * xdim + vec.x;

}
