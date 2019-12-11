using UnityEngine;

public interface ITicTacToeIA
{
    Vector2Int Play(Board gameBoard, CellState turn);
}
