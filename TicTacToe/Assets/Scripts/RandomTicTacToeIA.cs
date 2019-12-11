using UnityEngine;
using System.Collections.Generic;

public class RandomTicTacToeIA : MonoBehaviour, ITicTacToeIA
{
    public Vector2Int Play(Board gameBoard, CellState turn)
    {
        IList<Vector2Int> emptyPositions = new List<Vector2Int>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                if (gameBoard.GetStateAt(pos) == CellState.Undecided)
                {
                    emptyPositions.Add(pos);
                }
            }
        }

        return emptyPositions[Random.Range(0, emptyPositions.Count)];
    }
}
