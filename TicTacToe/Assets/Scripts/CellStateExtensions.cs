/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

// Extension methods for the CellState enumeration
public static class CellStateExtensions
{
    // Get the opponent of this player
    public static CellState Other(this CellState state)
    {
        // Who is this player?
        switch (state)
        {
            // If current player is O, the other player is X
            case CellState.O:
                return CellState.X;
            // If current player is X, the other player is O
            case CellState.X:
                return CellState.O;
            // Otherwise, throw an exception
            default:
                throw new InvalidOperationException(
                    $"Unable to get inverse state of {state}. Can only " +
                    "do it for {CellState.X} or {CellState.O} states");
        }
    }
}
