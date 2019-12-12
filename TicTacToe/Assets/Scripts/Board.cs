/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;
using System.Collections.Generic;

// Represents a TicTacToe board
public class Board : IBoard
{
    // 2D array containing board
    private CellState[,] board;

    // Create a new empty board
    public Board()
    {
        board = new CellState[3, 3];
    }

    // Get board state at specified position
    public CellState GetStateAt(Vector2Int pos) => board[pos.x, pos.y];

    // Set board state at specified position
    public void SetStateAt(Vector2Int pos, CellState state)
    {
        board[pos.x, pos.y] = state;
    }

    // Reset board
    public void Reset()
    {
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                board[x, y] = CellState.Undecided;
    }

    /// <summary>
    /// Board winner status.
    /// </summary>
    /// <returns>
    /// This method can return the following values:
    /// * `null` - Game in progress
    /// * `CellState.Undecided` - Game over, there is a draw.
    /// * `CellState.X` - Game over, X wins
    /// * `CellState.O` - Game over, O wins
    /// </returns>
    public CellState? Status()
    {
        // All possible winning positions
        IEnumerable<Vector2Int[]> winPositions = new List<Vector2Int[]>()
        {
            new Vector2Int[] {
                new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0)},
            new Vector2Int[] {
                new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1)},
            new Vector2Int[] {
                new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2)},
            new Vector2Int[] {
                new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2)},
            new Vector2Int[] {
                new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2)},
            new Vector2Int[] {
                new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2)},
            new Vector2Int[] {
                new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2)},
            new Vector2Int[] {
                new Vector2Int(0, 2), new Vector2Int(1, 1), new Vector2Int(2, 0)}
        };

        // Check win status for X and O
        foreach (CellState state in new CellState[] { CellState.X, CellState.O })
        {
            foreach (Vector2Int[] wp in winPositions)
            {
                if (board[wp[0].x, wp[0].y] == state
                    && board[wp[1].x, wp[1].y] == state
                    && board[wp[2].x, wp[2].y] == state)
                {
                    return state;
                }
            }
        }

        // Is board not full return null, meaning game is not over yet
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                if (board[x, y] == CellState.Undecided)
                    return null;

        // Game is over with a draw
        return CellState.Undecided;
    }
}
