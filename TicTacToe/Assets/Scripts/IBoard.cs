using UnityEngine;

public interface IBoard
{
    CellState GetStateAt(Vector2Int pos);
    //void SetStateAt(Vector2Int pos, CellState state);
}
