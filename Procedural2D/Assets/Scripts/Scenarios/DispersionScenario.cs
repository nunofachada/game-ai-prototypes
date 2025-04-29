/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections;
using UnityEngine;
using LibGameAI.QRNG;
using Random = System.Random;

namespace GameAIPrototypes.Procedural2D.Scenarios
{
    public class DispersionScenario : AbstractScenario
    {
        public enum PRNG { System, Halton }

        [SerializeField]
        private int numberOfPoints = 1000;

        [SerializeField]
        private PRNG xRandomNumberGenerator = PRNG.System;

        [SerializeField]
        private int xSeed = 2;

        [SerializeField]
        private PRNG yRandomNumberGenerator = PRNG.System;

        [SerializeField]
        private int ySeed = 3;

        public override IEnumerator Generate(Color[] pixels, int xDim, int yDim)
        {
            // Instantiate random number generator for x coordinate
            Random xRand = xRandomNumberGenerator == PRNG.System
                ? new Random(xSeed)
                : new Halton(xSeed);

            // Instantiate random number generator for y coordinate
            Random yRand = yRandomNumberGenerator == PRNG.System
                ? new Random(ySeed)
                : new Halton(ySeed);

            // Fill image with white
            Fill(pixels, Color.white);

            // Place the specified number of points
            for (int i = 0; i < numberOfPoints; i++)
            {
                int x = xRand.Next(xDim);
                int y = yRand.Next(yDim);
                pixels[y * xDim + x] = Color.black;
            }
            return null;
        }
    }
}