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
        if (inGame) GameGUI();
        else MenuGUI();
    }

    // Game UI
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
        if (centerLabelTextStyle == null)
        {
            centerLabelTextStyle = new GUIStyle(GUI.skin.label);
            centerLabelTextStyle.alignment = TextAnchor.MiddleCenter;
        }

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


    // Menu UI
    private void MenuGUI()
    {
        int buttonWidth = Screen.width / 7;
        int buttonHeight = Screen.height / 16;

        // Player X
        DropdownMenu(
            new Rect(
                Screen.width / 8,
                Screen.height / 5,
                buttonWidth,
                buttonHeight),
            playableIAs,
            ref expandDropdownX,
            ref indexPlayerX);

        // Player O
        DropdownMenu(
            new Rect(
                7 * Screen.width / 8 - buttonWidth,
                Screen.height / 5,
                buttonWidth,
                buttonHeight),
            playableIAs,
            ref expandDropdownO,
            ref indexPlayerO);

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

    private void DropdownMenu(
        Rect rect, IList<object> options, ref bool show, ref int index,
        string selString = "Select")
    {
        Vector2 scrollViewVector = Vector2.zero;
        GUIStyle italicTextStyle = new GUIStyle(GUI.skin.button);
        italicTextStyle.fontStyle = FontStyle.Italic;

        if(GUI.Button(
            new Rect(rect.x, rect.y, rect.width, rect.height),
            show ? selString : options[index].ToString(), italicTextStyle))
        {
            show = !show;
        }

        if(show)
        {
            scrollViewVector = GUI.BeginScrollView(
                new Rect(
                    rect.x, rect.y + rect.height, rect.width, rect.height * options.Count),
                scrollViewVector,
                new Rect(
                    0,
                    0,
                    rect.width,
                    rect.height * options.Count));

            GUI.Box(
                new Rect(0, 0, rect.width, rect.height * options.Count), "");

            for(int i = 0; i < options.Count; i++)
            {
                if(GUI.Button(
                    new Rect(0, i * rect.height, rect.width, rect.height),  options[i].ToString()))
                {
                    show = false;
                    index = i;
                }
            }

            GUI.EndScrollView();
        }
    }
}
