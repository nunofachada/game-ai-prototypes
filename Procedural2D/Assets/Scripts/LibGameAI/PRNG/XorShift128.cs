/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace LibGameAI.PRNG
{
    public class XorShift128 : Random
    {
        private uint x, y, z, w;

        public XorShift128() : this(System.DateTime.Now.Ticks)
        {
        }

        public XorShift128(int seed) : this((long)seed << 32 | (long)seed)
        {

        }
        public XorShift128(long seed)
        {
            if (seed == 0) seed = (long)-1;
            x = unchecked((uint)(0xFFFFFFFF & seed));
            y = unchecked((uint)(0xFFFFFFFF & (seed >> 16)));
            z = unchecked((uint)(0xFFFFFFFF & (seed >> 32)));
            w = unchecked((uint)(0xFFFFFFFF & (seed >> 48)));
        }

        private void UpdateState()
        {
            uint t = x ^ (x << 11);
            x = y;
            y = z;
            z = w;
            w = w ^ (w >> 19) ^ (t ^ (t >> 8));
        }

        // Implements the random number generation algorithm
        protected override double Sample()
        {
            UpdateState();
            return (double)(((ulong)z << 32) | (ulong)w) / ulong.MaxValue;
        }

        public override int Next()
        {
            UpdateState();
            return unchecked((int)w);
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
