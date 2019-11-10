using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tile prefabs should contain this script
public class TileBehaviour : MonoBehaviour
{
    // Common tile data
    [SerializeField] private TileScriptableObject tileData = null;

    // Tile type
    private TileTypeEnum tileType = default;
    // Reference to the sprite renderer
    private SpriteRenderer spriteRenderer;
    // Reference to the world script
    private World world;

    // Property that returns the tile type
    public TileTypeEnum TileType => tileType;

    // Position of this tile in the world
    internal Vector2Int Pos { get; set; }

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Get reference to sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Get reference to the world script
        world = GameObject.Find("World").GetComponent<World>();
    }

    // Set tile as empty or blocked
    public void SetAs(TileTypeEnum newTileType)
    {
        if (newTileType == TileTypeEnum.Empty)
        {
            spriteRenderer.sprite = tileData.Empty;
            tileType = TileTypeEnum.Empty;
        }
        else if (newTileType == TileTypeEnum.Blocked)
        {
            spriteRenderer.sprite = tileData.Blocked;
            tileType = TileTypeEnum.Blocked;
        }
    }

    // Swap tile type between blocked and empty
    private void OnMouseDown()
    {
        if (Pos != world.PlayerPos && Pos != world.GoalPos)
        {
            if (tileType == TileTypeEnum.Empty)
                SetAs(TileTypeEnum.Blocked);
            else if (tileType == TileTypeEnum.Blocked)
                SetAs(TileTypeEnum.Empty);
        }
    }

}
