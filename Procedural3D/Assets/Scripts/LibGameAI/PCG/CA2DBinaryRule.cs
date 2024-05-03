using System;
using System.Collections.Generic;


namespace LibGameAI.PCG
{
    public class CA2DBinaryRule : ICA2DRule
    {
        private const char ruleSep = '/';
        private const char rangeSep = '-';
        private const char inRuleSep = ',';

        public NeighborhoodType NeighborhoodType { get; private set; }
        public int Radius { get; private set; }
        public ISet<int> BirthRules { get; private set; }
        public ISet<int> SurvivalRules { get; private set; }

        public CA2DBinaryRule(string ruleString)
        {
            ParseRuleString(ruleString);
        }

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

        private void ParseRuleString(string rule)
        {
            string[] parts = rule.Split(ruleSep);
            ParseNeighborhoodAndRadius(parts[0]);
            SurvivalRules = ParseRuleValues(parts[1]);
            BirthRules = ParseRuleValues(parts[2]);
        }

        private void ParseNeighborhoodAndRadius(string part)
        {
            string[] nrParts = part.Split(inRuleSep);
            NeighborhoodType = ParseNeighborhoodType(nrParts[0]);
            Radius = int.Parse(nrParts[1]);
        }

        private NeighborhoodType ParseNeighborhoodType(string code)
        {
            return code switch
            {
                "M" => NeighborhoodType.Moore,
                "V" => NeighborhoodType.VonNeumann,
                "H" => NeighborhoodType.Hexagonal,
                _ => throw new ArgumentException("Unknown neighborhood type")
            };
        }

        private int MaxNeighbors()
        {
            return NeighborhoodType switch
            {
                NeighborhoodType.Moore => (2 * Radius + 1) * (2 * Radius + 1) - 1,
                NeighborhoodType.VonNeumann => 2 * Radius * (1 + Radius),
                NeighborhoodType.Hexagonal => 3 * Radius * (Radius + 1),
                _ => throw new ArgumentException("Unknown neighborhood type")
            };
        }

        private HashSet<int> ParseRuleValues(string ruleValues)
        {
            HashSet<int> values = new();
            if (string.IsNullOrEmpty(ruleValues)) return values;

            foreach (string part in ruleValues.Split(inRuleSep))
            {
                if (part.Contains(rangeSep))
                {
                    string[] rangeParts = part.Split(rangeSep);
                    int start = string.IsNullOrEmpty(rangeParts[0]) ? 0 : int.Parse(rangeParts[0]);
                    int end = string.IsNullOrEmpty(rangeParts[1]) ? MaxNeighbors() : int.Parse(rangeParts[1]);
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
