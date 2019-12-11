using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGUI : MonoBehaviour
{

    [SerializeField] private int cellSize = 40;
    [SerializeField] private int cellSpacing = 4;

    private Vector2Int topLeftCellPos;

    private GameController game;

    private void Awake()
    {
        topLeftCellPos = new Vector2Int(
            Screen.width / 2 - (cellSize + cellSpacing + cellSize / 2),
            Screen.height / 2 - (cellSize + cellSpacing + cellSize / 2));
        game = GetComponent<GameController>();
    }

    private void OnGUI()
    {
        string turnLabel = game.Status == null
            ? (game.IsHumanTurn
                ? game.Turn + " : " + "It's the weak human turn"
                : game.Turn + " : " + "It's the awesome IA turn")
            : "Game Over: " + game.Status;
        Vector2Int currPos = topLeftCellPos;

        GUI.color = Color.white;
        GUI.backgroundColor = Color.black;

        for (int x = 0; x < 3; x++)
        {
            currPos.y = topLeftCellPos.y;
            for (int y = 0; y < 3; y++)
            {
                CellState state =
                    game.GameBoard.GetStateAt(new Vector2Int(x, y));
                if (state == CellState.X)
                {
                    GUI.Label(
                        new Rect(currPos.x, currPos.y, cellSize, cellSize),
                        "X");
                }
                else if (state == CellState.O)
                {
                    GUI.Label(
                        new Rect(currPos.x, currPos.y, cellSize, cellSize),
                        "O");
                }
                else if (game.IsHumanTurn && game.Status == null)
                {
                    if (GUI.Button(
                        new Rect(currPos.x, currPos.y, cellSize, cellSize), ""))
                    {
                        game.PlayTurn(new Vector2Int(x, y));
                    }
                }
                currPos.y += cellSize + cellSpacing;
            }
            currPos.x += cellSize + cellSpacing;
        }

        GUI.color = Color.yellow;
        GUI.Label(new Rect(10, 10, 200, 20), turnLabel);
    }
}
