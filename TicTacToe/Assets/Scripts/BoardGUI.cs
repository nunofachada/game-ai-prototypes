/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;

// A user interface for TicTacToe
public class BoardGUI : MonoBehaviour
{

    // Cell dimensions and spacing
    [SerializeField] private int cellSize = 40;
    [SerializeField] private int cellSpacing = 4;

    [SerializeField] private string[] stuff = { "um", "dois", "tres" };

    // Position in pixels of the top left cell
    private Vector2Int topLeftCellPos;

    // Reference to the game controller
    private GameController game;

    // Is a game occurring?
    private bool inGame;

    // A list of playable IAs
    private IList<Type> playableIAs;

    // Initialization done here
    private void Awake()
    {
        // Initially, we're not in game
        inGame = false;

        // Determine position in pixels of the top left cell
        topLeftCellPos = new Vector2Int(
            Screen.width / 2 - (cellSize + cellSpacing + cellSize / 2),
            Screen.height / 2 - (cellSize + cellSpacing + cellSize / 2));

        // Get reference to the game controller
        game = GetComponent<GameController>();

        // Search for playable IAs
        playableIAs = (from type in Assembly.GetExecutingAssembly().GetTypes()
                       where typeof(ITicTacToeIA).IsAssignableFrom(type)
                           && !type.IsInterface
                           && !type.IsAbstract
                       select type).ToArray();
    }

    // Draw GUI
    private void OnGUI()
    {
        if (inGame)
            GameGUI();
        else MenuGUI();
    }

    private void GameGUI()
    {
        // Determine what label to show
        string turnLabel = game.Status == null
            ? (game.IsHumanTurn
                ? game.Turn + " : " + "It's the weak human turn"
                : game.Turn + " : " + "It's the awesome IA turn")
            : ("Game Over: " +
                (game.Status != CellState.Undecided
                    ? game.Status + " wins"
                    : "It's a draw"));

        // Position of current cell to draw
        Vector2Int currPos = topLeftCellPos;

        // Configure a GUI style for label text centering purposes
        GUIStyle centerLabelTextStyle = new GUIStyle(GUI.skin.label);
        centerLabelTextStyle.alignment = TextAnchor.MiddleCenter;

        // Set text color for moves
        GUI.color = Color.white;

        // Draw board
        for (int x = 0; x < 3; x++)
        {
            // Reset the cell position in the y-axis
            currPos.y = topLeftCellPos.y;
            for (int y = 0; y < 3; y++)
            {
                // Determine the state of the board for current cell
                CellState state =
                    game.GameBoard.GetStateAt(new Vector2Int(x, y));

                // Draw empty cell or move depending on what was determined by
                // the previous instruction
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
                else if (game.IsHumanTurn && game.Status == null)
                {
                    // Nobody played this position, show a button
                    if (GUI.Button(
                        new Rect(currPos.x, currPos.y, cellSize, cellSize), ""))
                    {
                        // If button is pressed, make the move
                        game.PlayTurn(new Vector2Int(x, y));
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
                inGame = false;
            }
        }

        // Set text color for info label
        GUI.color = Color.yellow;

        // Show info label
        GUI.Label(
            new Rect(
                Screen.width / 2 - 3 * cellSize,
                Screen.height / 2 - 3 * cellSize,
                cellSize * 6,
                cellSize),
            turnLabel,
            centerLabelTextStyle);
    }


    private void MenuGUI()
    {
        // Player X
        // Player O
        // Start
        if (GUI.Button(
            new Rect(
                Screen.width / 2 - 2 * cellSize,
                Screen.height / 2 -  cellSize / 2,
                cellSize * 4,
                cellSize),
            "Start new game"))
        {
            game.NewGame();
            inGame = true;
        }
    }

    // private int DropdownMenu()
    // {
    //     return 0;
    // }
}
