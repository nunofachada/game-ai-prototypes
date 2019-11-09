using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tile prefabs should contain this script
public class TileTypeBehaviour : MonoBehaviour
{
    // Tile type
    [SerializeField] private TileTypeEnum tileType = default;

    // Property that returns the tile type
    public TileTypeEnum TileType => tileType;
}
