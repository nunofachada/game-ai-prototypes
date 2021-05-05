/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using AIUnityExamples.RobbyOptimize.RobbyModel;

namespace AIUnityExamples.RobbyOptimize
{
    public class GeneticAlgorithm
    {
        private readonly int popCount;
        private readonly int sessionsPerEval;
        private readonly (IList<Reaction> rules, float fitness)[] popCurrent;
        private readonly (IList<Reaction> rules, float fitness)[] popNext;
        private readonly RobbyWorld world;

        private int step;
        private (IList<Reaction> rules, float fitness) bestSol;

        public GeneticAlgorithm(int popCount, int sessionsPerEval, RobbyWorld world)
        {
            this.popCount = popCount;
            this.sessionsPerEval = sessionsPerEval;
            this.world = world;

            popCurrent = new (IList<Reaction> rules, float fitness)[popCount];
            popNext = new (IList<Reaction> rules, float fitness)[popCount];

            for (int i = 0; i < popCount; i++)
            {
                popCurrent[i] = (new Reaction[TileUtil.numRules], float.NegativeInfinity);
                popNext[i] = (new Reaction[TileUtil.numRules], float.NegativeInfinity);
            }

            Reset();
        }

        public void Reset()
        {
            step = 0;
            for (int i = 0; i < popCount; i++)
            {
                world.GenerateRandomRules(popCurrent[i].rules);
                popCurrent[i].fitness = float.NegativeInfinity;
            }
        }

        public void Step()
        {
            EvalCurrentPop();
        }

        private void EvalCurrentPop()
        {

        }

    }
}