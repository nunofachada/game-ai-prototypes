/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace LibGameAI.QRNG
{
    public class Halton : Random
    {
        private int basePrime;
        private int index;

        public Halton(int basePrime, bool skipFirstN = true)
        {
            // if (!IsPrime(basePrime))
            // {
            //     throw new ArgumentException(
            //         $"{nameof(basePrime)} must be a prime number.");
            // }
            this.basePrime = basePrime;

            if (skipFirstN)
                index = basePrime + 1;
            else
                index = 0;
        }

        public static double Sequence(int basePrime, int index)
        {
            // if (!IsPrime(basePrime))
            // {
            //     throw new ArgumentException(
            //         $"{nameof(basePrime)} must be a prime number.");
            // }
            double result = 0;
            int denominator = 1;

            while (index > 0)
            {
                denominator *= basePrime;
                result += (index % basePrime) / (double)denominator;
                index = index / basePrime;
            }

            return result;
        }

        // Implements the random number generation algorithm
        protected override double Sample()
        {
            return Sequence(basePrime, index++);
        }

        public override int Next()
        {
            return (int)(Sample() * int.MaxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    $"{nameof(minValue)} cannot be larger than ${nameof(maxValue)}");
            }


            long range = (long)maxValue - minValue;

            if (range <= int.MaxValue)
            {
                return (int)(Sample() * range) + minValue;
            }
            else
            {
                return (int)(Sample() * range / 2.0 + Sample() * range / 2.0)
                    + minValue;
            }
        }

        public override void NextBytes(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(
                    $"{nameof(buffer)} cannot be null");
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)Next();
            }
        }
    }
}
