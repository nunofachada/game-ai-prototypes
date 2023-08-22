/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;

namespace GameAIPrototypes.TicTacToe
{
    /// <summary>
    /// Interface for a TicTacToe player.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Ask the AI player to make a move.
        /// </summary>
        /// <param name="gameBoard">The current board.</param>
        /// <param name="log">
        /// Optional log for AI players to produce debugging information.
        /// </param>
        /// <returns>The move to make.</returns>
        Pos Play(Board gameBoard, ref string log);
    }
}