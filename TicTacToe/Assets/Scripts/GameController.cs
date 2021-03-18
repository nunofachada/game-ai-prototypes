/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace AIUnityExamples.TicTacToe
{
    /// <summary>
    /// Controls a game of TicTacToe in Unity.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        // Who's turn is to play?
        public CellState Turn { get; private set; }

        // Override for game result
        private CellState overrideWinner;

        // What's the state of the board?
        public CellState? Status => overrideWinner != CellState.Undecided
            ? overrideWinner : gameBoard.Status();

        // The TicTacToe players
        public IPlayer PlayerX { get; set; }
        public IPlayer PlayerO { get; set; }

        // Reference to the game board
        private Board gameBoard = null;

        // Public property for accessing the game board
        public Board GameBoard => gameBoard;

        // Is a game taking place?
        public bool IsGameOn { get; set; }

        // Is it time for a human to play?
        public bool IsHumanTurn =>
            (Turn == CellState.X && PlayerX is HumanPlayer)
            ||
            (Turn == CellState.O && PlayerO is HumanPlayer);

        // Initialize
        private void Awake()
        {
            // Instantiate new board
            gameBoard = new Board();
            // Reset board
            NewGame();
            // Game not on right now
            IsGameOn = false;
            // We do not override the winner by the default
            overrideWinner = CellState.Undecided;
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
            // If it's the AI turn, schedule it for play in half a second
            if (!IsInvoking(nameof(DoAutoPlay))
                && !IsHumanTurn && IsGameOn && Status == null)
            {
                Invoke(nameof(DoAutoPlay), 0.5f);
            }
        }

        // Make an automatic move
        private void DoAutoPlay()
        {
            // Choose move
            Vector2Int pos = Turn == CellState.X
                ? PlayerX.Play(gameBoard, Turn)
                : PlayerO.Play(gameBoard, Turn);

            // Is this an invalid move?
            if (gameBoard.GetStateAt(pos) != CellState.Undecided)
            {
                overrideWinner = Turn.Other();
                return;
            }

            // Perform the actual move
            Move(pos);
        }

        // Perform a move
        public void Move(Vector2Int pos)
        {
            gameBoard.SetStateAt(pos, Turn);
            Turn = Turn.Other();
        }

        // Returns a string describing the state of the game
        public override string ToString()
        {
            string xName = PlayerX.GetType().Name;
            string oName = PlayerO.GetType().Name;

            if (Status == null)
            {
                // Game is ongoing
                return Turn +
                    $" : It's {(Turn == CellState.X ? xName : oName)} turn";
            }
            else
            {
                // Game is finished

                // Did someone make an illegal move?
                if (overrideWinner != CellState.Undecided)
                {
                    return string.Format(
                        "Game Over : {0} wins since {1} made illegal move",
                        Status == CellState.X ? xName : oName,
                        Status != CellState.X ? xName : oName);
                }

                // Game ended within the rules
                return "Game Over : " +
                    (Status != CellState.Undecided
                        ? $"{Status} ({(Status == CellState.X ? xName : oName)}) wins"
                        : "It's a draw");
            }
        }
    }
}