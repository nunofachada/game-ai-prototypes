/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Text;
using System.Collections.Generic;
using UnityEngine;
using AIUnityExamples.RobbyOptimize.RobbyModel;

namespace AIUnityExamples.RobbyOptimize
{
    public class Controller : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            //OneTurn();
            FullRun();
        }

        private void FullRun()
        {
            RobbyWorld world = new RobbyWorld(10, 10, 0.5f);

            IList<Action> rules = world.GenerateRandomRules();

            int score = world.FullRun(200, rules);

            Debug.Log(world.Log);
            Debug.Log($"Final score = {score}");
        }

        private void OneTurn()
        {
            (int row, int col) pos1, pos2;
            Tile[] situation = new Tile[TileUtil.NUM_NEIGHBORS];

            RobbyWorld world = new RobbyWorld(10, 10, 0.5f);

            IList<Action> rules = world.GenerateRandomRules();

            StringBuilder sb = new StringBuilder();

            int ruleIndex;

            pos1 = world.RobbyPos;
            world.GetSituationAt(pos1, situation);
            ruleIndex = TileUtil.ToDecimal(situation);

            sb.AppendFormat(
                "Robby's position: ({0},{1})\n", pos1.row, pos1.col);
            sb.AppendFormat(
                "Robby's situation: {0} [ {1} ]\n",
                ruleIndex, TileUtil.ToString(situation));
            sb.AppendFormat(
                "Robby's action to take for situation {0}: {1}\n",
                ruleIndex, rules[ruleIndex]);
            sb.AppendFormat("Robby's original score: {0}", world.Score);

            sb.AppendLine("\nActing up...");
            world.NextTurn(rules);

            pos2 = world.RobbyPos;

            sb.AppendFormat(
                "Robby's new position: ({0},{1})\n", pos2.row, pos2.col);
            sb.AppendFormat(
                "Situation in original position: {0}\n",
                world[pos1.row, pos1.col]);
            sb.AppendFormat("Robby's new score: {0}", world.Score);

            Debug.Log(sb.ToString());
        }

        // Update is called once per frame
        private void Update()
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}