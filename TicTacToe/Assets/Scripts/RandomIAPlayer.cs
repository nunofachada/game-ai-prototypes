/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;
using System.Collections.Generic;

public class RandomAIPlayer : IPlayer
{
    public Vector2Int Play(IBoard gameBoard, CellState turn)
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
