using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CellState Turn { get; private set; }

    private Board gameBoard = null;

    public IBoard GameBoard => gameBoard;

    private void Awake()
    {
        gameBoard = new Board();
    }

    // Start is called before the first frame update
    private void Start()
    {
        Turn = CellState.X;
        gameBoard.Reset();
    }

    public void PlayTurn(Vector2Int pos)
    {
        gameBoard.SetStateAt(pos, Turn);
        Turn = Turn == CellState.X ? CellState.O : CellState.X;
    }
}
