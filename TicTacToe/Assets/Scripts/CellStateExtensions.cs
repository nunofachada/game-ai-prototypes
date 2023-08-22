/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace GameAIPrototypes.TicTacToe
{
    /// <summary>
    /// Extension methods for the CellState enumeration.
    /// </summary>
    public static class CellStateExtensions
    {
        /// <summary>
        /// Get the opponent of the given player.
        /// </summary>
        /// <param name="state">A given player.</param>
        /// <returns>The opponent of the given player.</returns>
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

        /// <summary>
        /// Simplified short string representing a board cell state.
        /// </summary>
        /// <param name="state">The state of a board cell.</param>
        /// <returns>
        /// Returns the strings "X", "O" and "-" for the X, O and Undecided
        /// board states, respectively.
        /// </returns>
        public static string ToShortString(this CellState state)
        {
            return state == CellState.Undecided ? "-" : state.ToString();
        }
    }
}