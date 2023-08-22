/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using System.Linq;
using System.Reflection;
using System;

namespace GameAIPrototypes.TicTacToe
{
    /// <summary>
    /// A user interface for TicTacToe.
    /// </summary>
    public class BoardGUI : MonoBehaviour
    {
        // Cell dimensions and spacing
        [SerializeField] private int cellSize = 40;
        [SerializeField] private int cellSpacing = 4;

        // Position in pixels of the top left cell
        private Vector2Int topLeftCellPos;

        // Reference to the game controller
        private GameController game;

        // Expand dropdown menu for X player selection?
        private bool expandDropdownX = false;
        // The index of X player type
        private int indexPlayerX = 0;

        // Expand dropdown menu for O player selection?
        private bool expandDropdownO = false;
        // The index of O player type
        private int indexPlayerO = 0;

        // An array of playable IAs
        private Type[] playableIAs;

        // GUI style for label text centering purposes
        private GUIStyle centerLabelTextStyle = null;

        // Helper class for rendering a dropdown menu
        private Dropdown dropdown;

        // Initialization done here
        private void Awake()
        {
            // Instantiate helper class for rendering a dropdown menu
            dropdown = new Dropdown();

            // Determine position in pixels of the top left cell
            topLeftCellPos = new Vector2Int(
                Screen.width / 2 - (cellSize + cellSpacing + cellSize / 2),
                Screen.height / 2 - (cellSize + cellSpacing + cellSize / 2));

            // Get reference to the game controller
            game = GetComponent<GameController>();

            // Search for playable IAs
            playableIAs = (from type in Assembly.GetExecutingAssembly().GetTypes()
                           where typeof(IPlayer).IsAssignableFrom(type)
                               && !type.IsInterface
                               && !type.IsAbstract
                           select type).ToArray();
        }

        // Start a game
        private void StartGame()
        {
            game.PlayerX =
                Activator.CreateInstance(playableIAs[indexPlayerX]) as IPlayer;
            game.PlayerO =
                Activator.CreateInstance(playableIAs[indexPlayerO]) as IPlayer;
            game.NewGame();
            game.IsGameOn = true;
        }

        // Stop a game
        private void StopGame()
        {
            game.IsGameOn = false;
        }

        // Draw GUI
        private void OnGUI()
        {
            if (game.IsGameOn) GameGUI();
            else MenuGUI();
        }

        // Game UI
        private void GameGUI()
        {
            // Position of current cell to draw
            Vector2Int currPos = topLeftCellPos;

            // Configure a GUI style for label text centering purposes
            if (centerLabelTextStyle == null)
            {
                centerLabelTextStyle = new GUIStyle(GUI.skin.label);
                centerLabelTextStyle.alignment = TextAnchor.MiddleCenter;
            }

            // Set text color for moves
            GUI.color = Color.white;

            // Draw board
            for (int r = 0; r < 3; r++)
            {
                // Reset the cell position in the y-axis
                currPos.y = topLeftCellPos.y;
                for (int c = 0; c < 3; c++)
                {
                    // Determine the state of the board for current cell
                    CellState state =
                        game.GameBoard.GetStateAt(new Pos(r, c));

                    // Draw empty cell or move depending on what was determined
                    // by the previous instruction
                    if (state == CellState.X)
                    {
                        // X played this position, show respective label
                        GUI.Label(
                            new Rect(currPos.x, currPos.y, cellSize, cellSize),
                            "X",
                            centerLabelTextStyle);
                    }
                    else if (state == CellState.O)
                    {
                        // O played this position, show respective label
                        GUI.Label(
                            new Rect(currPos.x, currPos.y, cellSize, cellSize),
                            "O",
                            centerLabelTextStyle);
                    }
                    else if (game.IsHumanTurn)
                    {
                        // Nobody played this position, show a button
                        if (GUI.Button(
                            new Rect(currPos.x, currPos.y, cellSize, cellSize), ""))
                        {
                            // If button is pressed, make the move
                            game.Move(new Pos(r, c));
                        }
                    }
                    // Update the cell position in the y-axis
                    currPos.y += cellSize + cellSpacing;
                }
                // Update the cell position in the x-axis
                currPos.x += cellSize + cellSpacing;
            }

            // If game is over, show restart button
            if (game.Status != null)
            {
                if (GUI.Button(
                    new Rect(
                        Screen.width / 2 - 2 * cellSize,
                        Screen.height / 2 + 2 * cellSize,
                        cellSize * 4,
                        cellSize),
                    "Back to menu"))
                {
                    StopGame();
                }
            }

            // Set text color for info label
            GUI.color = Color.yellow;

            // Show info label
            GUI.Label(
                new Rect(
                    Screen.width / 4,
                    Screen.height / 2 - 3 * cellSize,
                    Screen.width / 2,
                    cellSize),
                game.ToString(),
                centerLabelTextStyle);
        }

        // Menu UI
        private void MenuGUI()
        {
            int buttonWidth = Screen.width / 7;
            int buttonHeight = Screen.height / 16;

            // Player X
            dropdown.Show(
                new Rect(
                    Screen.width / 8,
                    Screen.height / 5,
                    buttonWidth,
                    buttonHeight),
                playableIAs.Select(type => (object)type.Name).ToList(),
                ref expandDropdownX,
                ref indexPlayerX);

            // Player O
            dropdown.Show(
                new Rect(
                    7 * Screen.width / 8 - buttonWidth,
                    Screen.height / 5,
                    buttonWidth,
                    buttonHeight),
                playableIAs.Select(type => (object)type.Name).ToList(),
                ref expandDropdownO,
                ref indexPlayerO);

            // Start
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 - 2 * cellSize,
                    Screen.height / 2 - cellSize / 2,
                    cellSize * 4,
                    cellSize),
                "Start new game"))
            {
                StartGame();
            }
        }
    }
}