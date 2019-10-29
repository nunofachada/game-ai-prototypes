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

    [SerializeField] private Vector2Int wSize = new Vector2Int(10, 10);
    [SerializeField] [Range(0f, 1f)] private float passageRatio = 0.1f;

    // Player and goal positions
    private Vector2Int playerPos;
    private Vector2Int goalPos;

    // Goal reached?
    private bool goalReached = false;

    // How long between each movement
    private float gameStep = 0.5f;

    // Matrix of game world state
    GameObject[,] worldObjects;

    private void Awake()
    {
        // Offset for all tiles
        Vector2 offset;

        // Array of game objects to clone for each column
        GameObject[] columnToClone = new GameObject[wSize.y];

        // References to camera game object and camera component
        GameObject cameraGameObj = GameObject.FindWithTag("MainCamera");
        Camera cameraComponent = cameraGameObj.GetComponent<Camera>();

        // Width must be odd
        if (wSize.x % 2 == 0) wSize.x++;

        // Adjust camera to game world
        cameraComponent.orthographicSize = Mathf.Max(wSize.x, wSize.y) / 2;

        // Initialize matrix of game world elements (by default will be
        // all empties)
        worldObjects = new GameObject[wSize.x, wSize.y];

        // Determine tile offsets
        offset = new Vector2(wSize.x / 2f - 0.5f, wSize.y / 2f - 0.5f);

        // Cycle through columns
        for (int i = 0; i < wSize.x; i++)
        {
            // Number of passages in current column (by default all)
            int numEmpty = wSize.y;

            // For current column determine which tiles are empty and which are
            // blocked
            for (int j = 0; j < wSize.y; j++)
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
                columnToClone[Random.Range(0, wSize.y)] = emptyTile;
            }

            // Instantiate tiles for current column
            for (int j = 0; j < wSize.y; j++)
            {
                worldObjects[i, j] = Instantiate(columnToClone[j], transform);
                worldObjects[i, j].transform.position =
                    new Vector3(i - offset.x, j - offset.y, 0);
            }
        }

        // Place player anywhere in the world, as long as it's an empty tile
        while (true)
        {
            playerPos = new Vector2Int(
                Random.Range(0, wSize.x), Random.Range(0, wSize.y));
            if (worldObjects[playerPos.x, playerPos.y].tag == "Empty")
            {
                player = Instantiate(player);
                player.transform.position = new Vector3(
                    playerPos.x - offset.x, playerPos.y - offset.y, -1);
                break;
            }
        }

        Debug.Log($"Player placed at {playerPos}");

        // Place goal anywhere in the world, as long as it's an empty tile
        while (true)
        {
            goalPos = new Vector2Int(
                Random.Range(0, wSize.x), Random.Range(0, wSize.y));
            if (worldObjects[goalPos.x, goalPos.y].tag == "Empty")
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
        // Pre-alocate array of nodes (to maximum possible)
        Vector2Int[] nodesPosition = new Vector2Int[wSize.x * wSize.y];

        // Pre-alocate adjacency list for path finding
        IList<IEnumerable<IConnection>> nodes =
            new List<IEnumerable<IConnection>>(wSize.x * wSize.y);

        // Begin player movement steps
        StartCoroutine(MovementStep());
    }

    // Update is called once per frame
    private void Update()
    {
        //
    }

    // This co-routine implements the player movement steps
    private IEnumerator MovementStep()
    {
        // Coroutine will be called again in gameStep seconds
        YieldInstruction wfs = new WaitForSeconds(gameStep);

        // Node number
        int node = 0;

        // Start player movement loop
        while (playerPos != goalPos)
        {
            // Convert level into a graph
            for (int i = 0; i < wSize.x; i++)
            {
                for (int j = 0; i < wSize.y; i++)
                {
                    if (worldObjects[i, j].tag == "Empty")
                    {
                        //nodesPosition[node] = new Vector2Int(i, j);

                    }
                }
            }
            // Perform path finding

            // Return again after some fixed amount of time
            yield return wfs;
        }

        // If we got here, it means the goal was reached!
        goalReached = true;
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
}
