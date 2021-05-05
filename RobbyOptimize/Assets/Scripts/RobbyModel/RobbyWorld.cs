
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Text;
using System.Collections.Generic;

namespace AIUnityExamples.RobbyOptimize.RobbyModel
{
    public class RobbyWorld
    {
        private const int WALL_BUMP_PENALTY = 5;
        private const int PICK_UP_PENALTY = 1;
        private const int PICK_UP_BONUS = 10;

        private readonly Tile[,] world;
        private readonly Tile[] situation;
        private readonly Random random;
        private readonly float trashCov;
        private readonly StringBuilder logger;

        private static readonly int numActions;

        private int score;

        public Tile this[int row, int col] => world[row, col];

        public int Score => score;
        public string Log => logger.ToString();

        public (int row, int col) RobbyPos { get; private set; }

        public RobbyWorld(int rows, int cols, float trashCov, int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
            world = new Tile[rows, cols];
            situation = new Tile[TileUtil.NUM_NEIGHBORS];
            this.trashCov = trashCov;
            logger = new StringBuilder();
            Reset();
        }

        static RobbyWorld()
        {
            numActions = Enum.GetValues(typeof(Reaction)).Length;
        }

        public IList<Reaction> GenerateRandomRules(IList<Reaction> outRules = null)
        {
            IList<Reaction> rules = outRules ?? new Reaction[TileUtil.numRules];

            System.Diagnostics.Debug.Assert(rules.Count == TileUtil.numRules);

            for (int i = 0; i < TileUtil.numRules; i++)
            {
                rules[i] = (Reaction)random.Next(numActions);
            }

            return rules;
        }

        public void Reset()
        {
            score = 0;
            RobbyPos = (0, 0);
            logger.Clear();
            for (int i = 0; i < world.GetLength(0); i++)
            {
                for (int j = 0; j < world.GetLength(1); j++)
                {
                    if (random.NextDouble() < trashCov)
                    {
                        world[i, j] = Tile.Empty;
                    }
                    else
                    {
                        world[i, j] = Tile.Trash;
                    }
                }
            }
        }

        public int FullRun(int iterations, IList<Reaction> rules)
        {
            for (int i = 0; i < iterations; i++)
            {
                NextTurn(rules);
            }

            logger.AppendFormat(
                "Robby@({0},{1}), Score={2} [FINISHED]",
                RobbyPos.row, RobbyPos.col, score);

            return score;
        }

        public void NextTurn(IList<Reaction> rules)
        {
            System.Diagnostics.Debug.Assert(rules.Count == TileUtil.numRules);

            int ruleIndex;
            Reaction action;

            GetSituationAt(RobbyPos, situation);

            ruleIndex = TileUtil.ToDecimal(situation);

            action = rules[ruleIndex];

            logger.AppendFormat(
                "Robby@({0},{1}), Score={2}, Situation=({3})[{4}] -> Action={5}\n",
                RobbyPos.row, RobbyPos.col, score, ruleIndex,
                TileUtil.ToString(situation), action);

            if (action == Reaction.MoveRandom)
            {
                action = (Reaction)random.Next(TileUtil.NUM_NEIGHBORS);
            }

            switch (action)
            {
                case Reaction.MoveNorth: Move(-1, 0); break;
                case Reaction.MoveSouth: Move(1, 0); break;
                case Reaction.MoveEast: Move(0, 1); break;
                case Reaction.MoveWest: Move(0, -1); break;
                case Reaction.PickUpTrash: PickUpTrash(); break;
            }
        }

        public void GetSituationAt((int row, int col) pos, Tile[] situation)
        {
            System.Diagnostics.Debug.Assert(
                situation.Length == TileUtil.NUM_NEIGHBORS);

            Tile north, south, east, west, current;

            if (pos.row == 0) north = Tile.Wall;
            else north = world[pos.row - 1, pos.col];

            if (pos.row == world.GetLength(0) - 1) south = Tile.Wall;
            else south = world[pos.row + 1, pos.col];

            if (pos.col == world.GetLength(1) - 1) east = Tile.Wall;
            else east = world[pos.row, pos.col + 1];

            if (pos.col == 0) west = Tile.Wall;
            else west = world[pos.row, pos.col - 1];

            current = world[pos.row, pos.col];

            situation[0] = north;
            situation[1] = south;
            situation[2] = east;
            situation[3] = west;
            situation[4] = current;
        }

        private void Move(int dRow, int dCol)
        {
            int newRow = RobbyPos.row + dRow;
            int newCol = RobbyPos.col + dCol;
            if (newRow < 0 || newRow >= world.GetLength(0))
            {
                score -= WALL_BUMP_PENALTY;
            }
            else if (newCol < 0 || newCol >= world.GetLength(1))
            {
                score -= WALL_BUMP_PENALTY;
            }
            else
            {
                RobbyPos = (newRow, newCol);
            }
        }

        private void PickUpTrash()
        {
            if (world[RobbyPos.row, RobbyPos.col] == Tile.Trash)
            {
                score += PICK_UP_BONUS;
                world[RobbyPos.row, RobbyPos.col] = Tile.Empty;
            }
            else
            {
                score -= PICK_UP_PENALTY;
            }
        }
    }
}