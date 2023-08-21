/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;

namespace GameAIPrototypes.PathFinder2D
{
    // Tile prefabs should contain this script
    public class TileBehaviour : MonoBehaviour
    {
        // Common tile data
        [SerializeField] private TileScriptableObject tileData = null;

        // Sprite colors for different situations
        private readonly Color colorDefault = Color.white;
        private readonly Color colorMouseOver = Color.yellow;
        private readonly Color colorOpenFill = new Color(0.75f, 0.75f, 0.75f, 1);
        private readonly Color colorClosedFill = new Color(0.5f, 0.5f, 0.5f, 1);

        // Tile type
        private TileTypeEnum tileType = default;
        // Reference to the sprite renderer
        private SpriteRenderer spriteRenderer;
        // Reference to the world script
        private World world;

        // Property that returns the tile type
        public TileTypeEnum TileType => tileType;

        // Was tile used in path finding?
        public Fill FillState { get; set; } = Fill.None;

        // Property that returns true if mouse actions can be performed on a tile
        private bool ActOnMouse =>
            Pos != world.PlayerPos && Pos != world.GoalPos && !world.GoalReached;

        // Is the mouse over this tile?
        private bool mouseOver = false;

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

        // Make sure there is no highlight when goal is reached
        private void Update()
        {
            if (world.GoalReached)
            {
                spriteRenderer.color = colorDefault;
            }
            else
            {
                spriteRenderer.color = mouseOver
                    ? colorMouseOver
                    : world.ShowFill && FillState == Fill.Open
                        ? colorOpenFill
                        : world.ShowFill && FillState == Fill.Closed
                            ? colorClosedFill
                            : colorDefault;
            }
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
            if (ActOnMouse)
            {
                if (tileType == TileTypeEnum.Empty)
                    SetAs(TileTypeEnum.Blocked);
                else if (tileType == TileTypeEnum.Blocked)
                    SetAs(TileTypeEnum.Empty);
            }
        }

        // Highlight tile when mouse enters tile
        private void OnMouseEnter()
        {
            if (ActOnMouse)
            {
                mouseOver = true;
            }
        }

        // Remove tile highlight when mouse leaves tile
        private void OnMouseExit()
        {
            mouseOver = false;
        }
    }
}