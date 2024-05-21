/* Copyright (c) 2018-2024 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

namespace LibGameAI.PCG
{
    /// <summary>
    /// Interface for discrete 2D CA rules.
    /// </summary>
    public interface ICA2DRule
    {
        /// <summary>
        /// Process rule.
        /// </summary>
        /// <param name="ca">The cellular automaton.</param>
        /// <param name="x">Cell horizontal position.</param>
        /// <param name="y">Cell vertical position.</param>
        /// <returns>
        /// The new cell value after applying the rule.
        /// </returns>
        int ProcessRule(CA2D ca, int x, int y);
    }
}