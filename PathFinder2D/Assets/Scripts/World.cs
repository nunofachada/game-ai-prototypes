using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibGameAI.PathFinding;

public class World : MonoBehaviour
{

    // Goal reached?
    public bool GoalReached { get; private set; }

    // These should contain the prefabs that represent the game objects
    [SerializeField] private GameObject emptyTile = null;
    [SerializeField] private GameObject blockedTile = null;
    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject goal = null;

    // The world size
    [SerializeField] private Vector2Int worldSize = new Vector2Int(10, 10);

    // Ratio of passages in each column
    [SerializeField] [Range(0f, 1f)] private float passageRatio = 0.1f;

    // How long between each movement
    [SerializeField] private float moveDuration = 0.5f;

    // Path finding algorithm to use
    [SerializeField] private PathFindingType pathFindingType = default;

    // Enumeration that represents the differerent path finding strategies
    private enum PathFindingType { Dijkstra, AStar }

    // Player and goal positions
    private Vector2Int playerPos;
    private Vector2Int goalPos;

    // Current path
    private IEnumerable<IConnection> path = null;

    // Matrix of game world state
    private TileTypeBehaviour[,] world;

    // Offset for all tiles
    private Vector2 offset;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // References to camera game object and camera component
        GameObject cameraGameObj = GameObject.FindWithTag("MainCamera");
        Camera cameraComponent = cameraGameObj.GetComponent<Camera>();

        // Width must be odd
        if (worldSize.x % 2 == 0) worldSize.x++;

        // Adjust camera to game world
        cameraComponent.orthographicSize =
            Mathf.Max(worldSize.x, worldSize.y) / 2;

        // Initialize matrix of game world elements (by default will be
        // all empties)
        world = new TileTypeBehaviour[worldSize.x, worldSize.y];

        // Determine tile offsets
        offset = new Vector2(worldSize.x / 2f - 0.5f, worldSize.y / 2f - 0.5f);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Goal hasn't been reached yet
        GoalReached = false;

        // Generate level
        GenerateLevel();

        // Start path finding process
        StartCoroutine(FindPath());
    }

    // Restart path finding
    internal void Restart()
    {
        // Delete world tiles
        for (int i = 0; i < worldSize.x; i++)
        {
            for (int j = 0; j < worldSize.y; j++)
            {
                Destroy(world[i, j].gameObject);
            }
        }
        // Delete player and goal
        Destroy(player);
        Destroy(goal);

        // Start again
        Start();
    }

    // Procedurally generate level
    private void GenerateLevel()
    {
        // Array of game objects to clone for each column
        GameObject[] columnToClone = new GameObject[worldSize.y];

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

    // This method is used as an heuristic for A*
    // It's the euclidean distance heuristic
    private float HeuristicForAStar(int node)
    {
        Vector2 nodeVec = (Vector2)Ind2Vec(node, world.GetLength(0));
        Vector2 destVec = (Vector2)goalPos;
        return Vector2.Distance(nodeVec, destVec);
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
            if (pathFindingType == PathFindingType.Dijkstra)
            {
                // Use Dijkstra algorithm
                path = Dijkstra.GetShortestPath(
                    graph,
                    Vec2Ind(playerPos, world.GetLength(0)),
                    Vec2Ind(goalPos, world.GetLength(0)));
            }
            else
            {
                // Use AStar algorithm
                path = AStar.GetPath(
                    graph,
                    Vec2Ind(playerPos, world.GetLength(0)),
                    Vec2Ind(goalPos, world.GetLength(0)),
                    HeuristicForAStar);
            }

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
        GoalReached = true;
    }

    // Co-routine that performs the player movement animation
    private IEnumerator MovePlayer(int toNode, float duration)
    {
        // Final position of player sprite
        Vector3 finalPos;
        // Initial position of player sprite
        Vector3 initPos = player.transform.position;
        // Animation start time
        float startTime = Time.time;
        // Animation end time
        float endTime = startTime + duration;

        // Set player tile position to its final position
        playerPos = Ind2Vec(toNode, world.GetLength(0));

        // Determine player sprite final position
        finalPos = new Vector3(
            playerPos.x - offset.x,
            playerPos.y - offset.y,
            player.transform.position.z);

        // Perform animation
        while (Time.time < endTime)
        {
            // Get current time
            float currTime = Time.time;
            // Determine movement length
            float toMove = Mathf.InverseLerp(startTime, endTime, currTime);
            // Determine new sprite position for current frame
            Vector3 newPos = Vector3.Lerp(initPos, finalPos, toMove);
            // Move player sprite to new position
            player.transform.position = newPos;
            // Give control back to main loop, this loop will continue on the
            // next frame
            yield return null;
        }

        // Make sure player sprite ends up in the final position
        player.transform.position = finalPos;
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
        Gizmos.color = Color.red;
        if (path != null)
        {
            bool first = true;
            // Cycle through all connections
            foreach (IConnection conn in path)
            {
                // Don't draw first connection, it doesn't look very nice
                if (first)
                {
                    first = false;
                }
                else
                {
                    // Get start tile for current connection
                    Vector2Int start = Ind2Vec(conn.FromNode, world.GetLength(0));
                    // Get end tile for current connection
                    Vector2Int end = Ind2Vec(conn.ToNode, world.GetLength(0));
                    // Get on-screen position for start of current connection
                    Vector3 startPos = new Vector3(
                        start.x - offset.x, start.y - offset.y, -3);
                    // Get on-screen position for end of current connection
                    Vector3 endPos = new Vector3(
                        end.x - offset.x, end.y - offset.y, -3);
                    // Draw line for current connection
                    Gizmos.DrawLine(startPos, endPos);
                }
            }
        }
    }

    // Convert index (graph bode) to int vector (world position)
    public static Vector2Int Ind2Vec(int index, int xdim) =>
        new Vector2Int(index % xdim, index / xdim);
    // Convert int vector (world position) to index (graph bode)
    public static int Vec2Ind(Vector2Int vec, int xdim) =>
        vec.y * xdim + vec.x;

}
