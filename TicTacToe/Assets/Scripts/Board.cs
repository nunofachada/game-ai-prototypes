using UnityEngine;
using System.Collections.Generic;

public class Board : IBoard
{
    private CellState[,] board;

    public Board()
    {
        board = new CellState[3, 3];
    }

    public CellState GetStateAt(Vector2Int pos) => board[pos.x, pos.y];

    public void SetStateAt(Vector2Int pos, CellState state)
    {
        board[pos.x, pos.y] = state;
    }

    public void Reset()
    {
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                board[x, y] = CellState.Undecided;
    }

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
