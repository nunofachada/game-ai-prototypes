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
    // Who plays what
    [SerializeField] private PlayerType playerX = default;
    [SerializeField] private PlayerType playerO = default;

    // Who's turn is to play?
    public CellState Turn { get; private set; }

    // Player type for X and O: IA or Human
    public PlayerType PlayerX => playerX;
    public PlayerType PlayerO => playerO;

    // What's the state of the board?
    public CellState? Status => gameBoard.Status();

    // The current IA
    private ITicTacToeIA iA;

    // Is the human's turn?
    public bool IsHumanTurn =>
        (Turn == CellState.X && playerX == PlayerType.Human) ||
        (Turn == CellState.O && playerO == PlayerType.Human);

    // Reference to the game board
    private Board gameBoard = null;

    // Public property for accessing the game board
    public IBoard GameBoard => gameBoard;

    // Initialize
    private void Awake()
    {
        // Instantiate new board
        gameBoard = new Board();
        // Get a reference to the AI
        iA = GetComponent<ITicTacToeIA>();
        // Reset board
        NewGame();
    }

    // Start new game
    public void NewGame()
    {
        // X is first to play
        Turn = CellState.X;
        // Clear the board
        gameBoard.Reset();
    }

    // Update called once per frame
    private void Update()
    {
        // If it's the AI turn, schedule it for play in one second
        if (!IsHumanTurn && !IsInvoking("IAPlay") && Status == null)
        {
            Invoke("IAPlay", 1f);
        }
    }

    // Make a IA move
    private void IAPlay()
    {
        // Chose move
        Vector2Int iaMove = iA.Play(gameBoard, Turn);
        // Perform move
        PlayTurn(iaMove);
    }

    // Perform a move
    public void PlayTurn(Vector2Int pos)
    {
        gameBoard.SetStateAt(pos, Turn);
        Turn = Turn == CellState.X ? CellState.O : CellState.X;
    }

}
