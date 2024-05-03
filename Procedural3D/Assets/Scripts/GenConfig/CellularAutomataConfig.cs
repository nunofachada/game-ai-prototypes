/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using LibGameAI.PCG;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;

namespace GameAIPrototypes.ProceduralLandscape.GenConfig
{
    public class CellularAutomataConfig : StochasticGenConfig
    {
        private const string customRuleName = "<Custom>";

        [SerializeField]
        [Dropdown(nameof(RuleNames))]
        [OnValueChanged(nameof(UpdateRuleString))]
        private string ruleName;

        [SerializeField]
        [EnableIf(nameof(IsCustom))]
        private string ruleString;

        [SerializeField]
        [Range(0, 1)]
        private float initialFill = 0.5f;

        [SerializeField]
        private uint skipFirstNSteps = 1;
        [SerializeField]
        private uint steps = 5;
        [SerializeField]
        private bool toroidal = true;
        [SerializeField]
        [HideIf(nameof(toroidal))]
        private bool offGridBorderCellsAlive = false;

        private readonly IDictionary<string, string> rules = new Dictionary<string, string>()
        {
            {"MajR1N5", "M,1/5-/5-"},
            {"MajR2N13", "M,2/13-/13-"},
            {"MajR4N38", "M,4/38-/38-"},
            {"MajR4N39", "M,4/39-/39-"},
            {"MajR4N40", "M,4/40-/40-"},
            {"MajR4N41", "M,4/41-/41-"},
            {"MajR4N42", "M,4/42-/42-"},
            {"MajR4N43", "M,4/43-/43-"},
            {"CavesR1N5", "M,1/4-/5-"},
            {"CavesR2N13", "M,2/12-/13-"},
            {"WalledCities", "M,1/2-5/4-"},
            {"Diamoeba", "M,1/5-/3,5-"},
            {"Coral", "M,1/4-/3"},
            {"HighLife", "M,1/2,3/3,6"},
            {"GameOfLife", "M,1/2,3/3"},
            {"Serviettes", "M,1//2-4"},
            {"Flakes", "M,1/-/3"},
            {customRuleName, "" },
        };

        private bool IsCustom => ruleName == customRuleName;
        private string[] RuleNames => rules.Keys.ToArray();

        private void UpdateRuleString()
        {
            if (ruleName != customRuleName)
            {
                ruleString = rules[ruleName];
            }
        }


        public override float[,] Generate(float[,] prev_heights)
        {
            InitPRNG();

            int xdim = prev_heights.GetLength(0);
            int ydim = prev_heights.GetLength(1);

            CA2D ca = new(new CA2DBinaryRule(ruleString), xdim, ydim, toroidal, offGridBorderCellsAlive ? 1 : 0);
            ca.InitRandom(new int[] { 0, 1 }, new float[] { 1 - initialFill, initialFill }, () => (float)PRNG.NextDouble());

            float[,] ca_heights = new float[xdim, ydim];

            if (skipFirstNSteps == 0) AddLayer(ca_heights, ca);

            for (uint t = 1; t <= steps; t++)
            {
                ca.DoStep();

                if (t >= skipFirstNSteps)
                    AddLayer(ca_heights, ca);
            }

            return ca_heights;
        }

        private void AddLayer(float[,] heights, CA2D caLayer)
        {
            int xdim = heights.GetLength(0);
            int ydim = heights.GetLength(1);
            for (int y = 0; y < ydim; y++)
            {
                for (int x = 0; x < xdim; x++)
                {
                    heights[x, y] += caLayer[x, y];
                }
            }
        }
    }
}