using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTypeBehaviour : MonoBehaviour
{
    [SerializeField] private TileTypeEnum tileType = default;

    public TileTypeEnum TileType => tileType;
}
