/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls a game of TicTacToe in Unity
public class GameController : MonoBehaviour
{
    // Who's turn is to play?
    public CellState Turn { get; private set; }

    // What's the state of the board?
    public CellState? Status => gameBoard.Status();

    // The TicTacToe players
    public IPlayer PlayerX { get; set; }
    public IPlayer PlayerO { get; set; }

    // Reference to the game board
    private Board gameBoard = null;

    // Public property for accessing the game board
    public IBoard GameBoard => gameBoard;

    // Is the game in auto play mode?
    public bool AutoPlay { get; set; }

    // Initialize
    private void Awake()
    {
        // Instantiate new board
        gameBoard = new Board();
        // Get a reference to the AI
        iA = GetComponent<IPlayer>();
        // Reset board
        NewGame();
    }

    // Start new game
    public void NewGame()
    {
        // Auto play is initially disabled
        AutoPlay = false;
        // X is first to play
        Turn = CellState.X;
        // Clear the board
        gameBoard.Reset();
    }

    // Update called once per frame
    private void Update()
    {
        // If it's the AI turn, schedule it for play in half a second
        if (!IsInvoking("DoAutoPlay") && Status == null && AutoPlay)
        {
            Invoke("DoAutoPlay", 0.5f);
        }
    }

    // Make an automatic move
    private void DoAutoPlay()
    {
        // Choose move
        Vector2Int pos = Turn == CellState.X
            ? PlayerX.Play(gameBoard, Turn)
            : PlayerO.Play(gameBoard, Turn);

        // Perform the actual move
        Move(pos);
    }

    // Perform a move
    private void Move(Vector2Int pos)
    {
        gameBoard.SetStateAt(pos, Turn);
        Turn = Turn == CellState.X ? CellState.O : CellState.X;
    }

    // Returns a string describing the state of the game
    public override string ToString()
    {
        if (Status == null)
        {
            // Game is ongoing
            return Turn +
                $" : It's {(Turn == CellState.X ? PlayerX : PlayerO)} turn";
        }
        else
        {
            // Game is finished
            return "Game Over : " +
                (Status != CellState.Undecided
                    ? $"{Status} ({(Status == CellState.X ? PlayerX : PlayerO)}) wins"
                    : "It's a draw");
        }
    }
}
