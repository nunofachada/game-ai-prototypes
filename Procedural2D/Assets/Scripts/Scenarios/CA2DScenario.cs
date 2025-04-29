/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using LibGameAI.PCG;
using LibGameAI.Util;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    public class CA2DScenario : StochasticScenario
    {
        private const string customRuleName = "<Custom>";
        private const string defaultCustomRule = "M,1/5-/5-";

        [SerializeField]
        [Dropdown(nameof(RuleNames))]
        [OnValueChanged(nameof(UpdateRuleString))]
        private string ruleName = customRuleName;

        [SerializeField]
        [EnableIf(nameof(IsCustom))]
        private string ruleString = defaultCustomRule;

        [SerializeField]
        [Range(0, 1)]
        private float initialFill = 0.5f;

        [SerializeField]
        private uint steps = 5;

        [SerializeField]
        private bool drawOutline = false;

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
            {customRuleName, defaultCustomRule},
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

        private readonly Color DEAD = Color.white;
        private readonly Color ALIVE = Color.gray;
        private readonly Color WALL = Color.red;

        public override IEnumerator Generate(Color[] pixels, int xDim, int yDim)
        {
            base.Generate(pixels, xDim, yDim);

            // Define cellular automata (CA) rule
            CA2DBinaryRule rule = new CA2DBinaryRule(ruleString);

            // Setup CA with specified rule and parameters
            CA2D ca = new CA2D(
                rule, xDim, yDim, toroidal,
                offGridBorderCellsAlive ? 1 : 0);

            // Initialize CA
            ca.InitRandom(
                new int[] { 0, 1 },
                new float[] { 1 - initialFill, initialFill },
                PRNG.NextFloat);

            // Run CA for the specified number of steps
            for (int i = 0; i < steps; i++)
            {
                ca.DoStep();
                yield return null;
            }

            // Convert CA to image colors and post-process borders for
            // visual effect
            for (int y = 0; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    Color pixelColor;

                    if (ca[x, y] == 0)
                    {
                        pixelColor = DEAD;
                    }
                    else // We assume it's ALIVE
                    {
                        pixelColor = ALIVE;

                        if (drawOutline)
                        {
                            // How many alive around here?
                            int numAlive = ca.CountNeighbors(
                                x, y, 1, neighValue: 1);

                            if (numAlive < 8)
                            {
                                // Put outline
                                pixelColor = WALL;
                            }
                        }
                    }
                    pixels[y * xDim + x] = pixelColor;
                }
            }
            yield break;
        }
    }
}