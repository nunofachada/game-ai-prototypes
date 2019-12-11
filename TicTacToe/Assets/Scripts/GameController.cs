using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerType playerX = default;
    [SerializeField] private PlayerType playerO = default;

    public CellState Turn { get; private set; }

    public PlayerType PlayerX => playerX;
    public PlayerType PlayerO => playerO;

    public CellState? Status => gameBoard.Status();

    private ITicTacToeIA iA;

    public bool IsHumanTurn =>
        (Turn == CellState.X && playerX == PlayerType.Human) ||
        (Turn == CellState.O && playerO == PlayerType.Human);

    private Board gameBoard = null;

    public IBoard GameBoard => gameBoard;

    private void Awake()
    {
        gameBoard = new Board();
        iA = GetComponent<ITicTacToeIA>();
    }

    private void Start()
    {
        Turn = CellState.X;
        gameBoard.Reset();
    }

    private void Update()
    {
        if (!IsHumanTurn && !IsInvoking("IAPlay") && Status == null)
        {
            Invoke("IAPlay", 1f);
        }
    }

    private void IAPlay()
    {
        Vector2Int iaMove = iA.Play(gameBoard, Turn);
        PlayTurn(iaMove);
    }

    public void PlayTurn(Vector2Int pos)
    {
        gameBoard.SetStateAt(pos, Turn);
        Turn = Turn == CellState.X ? CellState.O : CellState.X;
    }

}
