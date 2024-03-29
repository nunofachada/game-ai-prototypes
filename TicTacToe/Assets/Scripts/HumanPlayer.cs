/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

namespace GameAIPrototypes.TicTacToe
{
    /// <summary>
    /// A human TicTacToe player, doesn't do anything since the actual plays are
    /// delegated to the UI.
    /// </summary>
    public class HumanPlayer : IPlayer
    {
        public Pos Play(Board gameBoard, ref string log) => Board.NoMove;
    }
}