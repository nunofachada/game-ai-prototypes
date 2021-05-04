
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

namespace AIUnityExamples.RobbyOptimize
{
    public class RobbyWorld
    {
        private const int MOVE_DIRECTIONS = 5; // N, S, E, W, Stay
        private const int WALL_BUMP_PENALTY = 5;
        private const int PICK_UP_PENALTY = 1;
        private const int PICK_UP_BONUS = 10;

        private readonly Tile[,] world;
        private readonly Random random;
        private readonly float trashCov;

        private int score;

        public Tile this[int row, int col] => world[row, col];

        public int Score => score;

        public (int row, int col) RobbyPos { get; private set; }

        public RobbyWorld(int rows, int cols, float trashCov)
        {
            random = new Random();
            world = new Tile[rows, cols];
            this.trashCov = trashCov;
            Reset();
        }

        public void Reset()
        {
            score = 0;
            RobbyPos = (0, 0);
            for (int i = 0; i < world.GetLength(0); i++)
            {
                for (int j = 0; j < world.GetLength(1); j++)
                {
                    if (random.NextDouble() < 0.5)
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

        public int FullRun(int iterations, IReadOnlyList<Action> rules)
        {
            for (int i = 0; i < iterations; i++)
            {
                NextTurn(rules);
            }
            return score;
        }

        public void NextTurn(IReadOnlyList<Action> rules)
        {
            Tile north, south, east, west, current;
            Situation situation;
            int ruleIndex;
            Action action;

            if (RobbyPos.row == 0) north = Tile.Wall;
            else north = world[RobbyPos.row - 1, RobbyPos.col];

            if (RobbyPos.row == world.GetLength(0) - 1) south = Tile.Wall;
            else south = world[RobbyPos.row + 1, RobbyPos.col];

            if (RobbyPos.col == world.GetLength(1) - 1) east = Tile.Wall;
            else east = world[RobbyPos.row, RobbyPos.col + 1];

            if (RobbyPos.col == 0) west = Tile.Wall;
            else west = world[RobbyPos.row, RobbyPos.col - 1];

            current = world[RobbyPos.row, RobbyPos.col];

            situation = new Situation(north, south, east, west, current);

            ruleIndex = situation.Index;

            action = rules[ruleIndex];

            if (action == Action.MoveRandom)
            {
                action = (Action)random.Next(MOVE_DIRECTIONS);
            }

            switch (action)
            {
                case Action.MoveNorth: Move(-1, 0); break;
                case Action.MoveSouth: Move(1, 0); break;
                case Action.MoveEast: Move(0, 1); break;
                case Action.MoveWest: Move(0, -1); break;
                case Action.PickUpTrash: PickUpTrash(); break;
            }
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