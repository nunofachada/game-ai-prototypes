/* Copyright (c) 2018-2024 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;
using LibGameAI.Util;

namespace LibGameAI.PCG
{
    /// <summary>
    /// A transition rule for discrete binary deterministic 2D cellular automata.
    /// </summary>
    /// <remarks>
    /// A rule is defined as a string as follows:
    ///
    /// `<N,R/{values-for-survival}/{values-for-birth}>`
    ///
    /// Where:
    ///
    /// - `N` represents the neighborhood type (`M` for Moore, `V` for Von
    ///   Neumann, `H` for Hexagonal).
    /// - `R` indicates the radius of the neighborhood.
    /// -  `{values-for-survival}` and `{values-for-birth}` are comma-separated
    ///    lists where values can be individual numbers or ranges denoted by a
    ///    start and end number separated by a hyphen (`-`). A hyphen at the
    ///    start or end indicates a range extending from zero or up to the
    ///    maximum possible number of neighbors, respectively.
    ///
    /// Examples:
    ///
    /// - `M,1/2-5/3,6-8`
    ///   - Neighborhood: Moore
    ///   - Radius: 1
    ///   - Survival: A cell survives if it has between 2 and 5 live neighbors.
    ///   - Birth: A cell is born if it has exactly 3 live neighbors or between
    ///     6 and 8 live neighbors.
    /// - `V,2/-/1-4
    ///   - Neighborhood: Von Neumann
    ///   - Radius: 2
    ///   - Survival: No specific survival rules; the hyphen implies all
    ///     possibilities are valid, so live cells will always survive.
    ///   - Birth: A cell is born if it has between 1 and 4 live neighbors.
    /// - `H,3/0,2,4-6/10-`
    ///   - Neighborhood: Hexagonal
    ///   - Radius: 3
    ///   - Survival: A cell survives if it has 0, 2, or between 4 and 6 live
    ///     neighbors.
    ///   - Birth: Cells are born if they have 10 or more live neighbors.
    /// - `M,2/1,10-/-3,7-9`
    ///   - Neighborhood: Moore
    ///   - Radius: 2
    ///   - Survival: A cell survives if it has exactly 1 live neighbor or
    ///     more than 10 live neighbors.
    ///   - Birth: A cell is born if it has between 0 and 3 live neighbors or
    ///     between live 7 and 9 neighbors.
    /// <remarks>
    public class CA2DBinaryRule : ICA2DRule
    {
        // Constants defining rule string separators
        private const char ruleSep = '/';
        private const char rangeSep = '-';
        private const char inRuleSep = ',';

        // Maximum number of neighbors for the specified neighborhood type.
        private readonly int maxNeighbors;

        /// <summary>
        /// Neighborhood type.
        /// </summary>
        public Neighborhood NeighborhoodType { get; }

        /// <summary>
        /// Neighborhood radius.
        /// </summary>
        public int Radius { get; }

        /// <summary>
        /// Survival rules.
        /// </summary>
        public ISet<int> SurvivalRules { get; }

        /// <summary>
        /// Birth rules.
        /// </summary>
        public ISet<int> BirthRules { get; }

        /// <summary>
        /// Create a new transition rule defined in the given string.
        /// </summary>
        /// <param name="ruleString">
        /// A string defining the rule.
        /// </param>
        public CA2DBinaryRule(string ruleString)
        {
            string[] parts = ruleString.Split(ruleSep);
            (NeighborhoodType, Radius) = ParseNeighborhoodAndRadius(parts[0]);
            maxNeighbors = NeighborhoodType.MaxNeighbors(Radius);
            SurvivalRules = ParseRuleValues(parts[1]);
            BirthRules = ParseRuleValues(parts[2]);
        }

        /// <summary>
        /// Process the specified CA cell with this rule.
        /// </summary>
        /// <param name="ca">The cellular automaton.</param>
        /// <param name="x">Horizontal cell position.</param>
        /// <param name="y">Vertical cell position.</param>
        /// <returns>
        /// New value for the specified CA cell after applying this rule.
        /// </returns>
        public int ProcessRule(CA2D ca, int x, int y)
        {
            int newState = 0;

            int numNeighs = ca.CountNeighbors(x, y, Radius, neighType: NeighborhoodType);
            if (ca[x, y] == 0 && BirthRules.Contains(numNeighs)
                || ca[x, y] == 1 && SurvivalRules.Contains(numNeighs))
            {
                newState = 1;
            }
            return newState;
        }

        // Parse the neighborhood type and radius component of the rule string
        private (Neighborhood, int) ParseNeighborhoodAndRadius(string part)
        {
            string[] nrParts = part.Split(inRuleSep);
            return (ParseNeighborhoodType(nrParts[0]), int.Parse(nrParts[1]));
        }

        // Parse the neighborhood type
        private Neighborhood ParseNeighborhoodType(string code)
        {
            return code switch
            {
                "M" => Neighborhood.Moore,
                "V" => Neighborhood.VonNeumann,
                "H" => Neighborhood.Hexagonal,
                _ => throw new ArgumentException("Unknown neighborhood type")
            };
        }

        // Parse the survival and birth rule string components
        private ISet<int> ParseRuleValues(string ruleValues)
        {
            HashSet<int> values = new();
            if (string.IsNullOrEmpty(ruleValues)) return values;

            foreach (string part in ruleValues.Split(inRuleSep))
            {
                if (part.Contains(rangeSep))
                {
                    string[] rangeParts = part.Split(rangeSep);
                    int start = string.IsNullOrEmpty(rangeParts[0]) ? 0 : int.Parse(rangeParts[0]);
                    int end = string.IsNullOrEmpty(rangeParts[1]) ? maxNeighbors : int.Parse(rangeParts[1]);
                    for (int i = start; i <= end; i++)
                    {
                        values.Add(i);
                    }
                }
                else
                {
                    values.Add(int.Parse(part));
                }
            }
            return values;
        }
    }
}
