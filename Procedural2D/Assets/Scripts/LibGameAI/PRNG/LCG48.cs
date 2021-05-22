/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace LibGameAI.PRNG
{
    /// <summary>
    /// C# reimplementation of Java's PRNG, a linear congruential generator
    /// (LCG).
    /// </summary>
    /// <remarks>
    /// Useful links:
    /// <list type="bullet">
    /// <item><description>
    /// <seealso href="https://en.wikipedia.org/wiki/Linear_congruential_generator">
    /// Linear congruential generator - Wikipedia
    /// </seealso>
    /// </description></item>
    /// <item><description>
    /// <seealso href="https://github.com/fakenmc/cl_ops/blob/master/src/cl_ops/rng/clo_rng_lcg.cl">
    /// Implementation of this particular LCG in OpenCL
    /// </seealso>
    /// </description></item>
    /// <item><description>
    /// <seealso href="https://github.com/openjdk/jdk/blob/master/src/java.base/share/classes/java/util/Random.java">
    /// Java's implementation
    /// </seealso>
    /// </description></item>
    /// <item><description>
    /// <seealso href="https://docs.oracle.com/javase/8/docs/api/java/util/Random.html">
    /// Java's documentation of the Random class
    /// </seealso>
    /// </description></item>
    /// <item><description>
    /// <seealso href="https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Random.cs">
    /// .NET Core implementation of the Random class
    /// </seealso>
    /// </description></item>
    /// <item><description>
    /// <seealso href="https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/random.cs">
    /// .NET Framework implementation of the Random class
    /// </seealso>
    /// </description></item>
    /// <item><description>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.random?view=net-5.0#notes-to-inheritors">
    /// Documentation on how to extend the Random class
    /// </seealso>
    /// </description></item>
    /// </list>
    /// </remarks>
    public class LCG48 : Random
    {
        private long state;

        private const long multiplier = 0x5DEECE66DL;
        private const long addend = 0xBL;
        private const long mask = (1L << 48) - 1;

        private const double DOUBLE_UNIT = 1.0 / (1L << 53);


        public LCG48() : this(Environment.TickCount)
        {
        }

        public LCG48(int seed) : this((long)seed << 32 | (long)seed)
        {

        }
        public LCG48(long seed)
        {
            state = (seed ^ multiplier) & mask;
        }

        protected override double Sample()
        {
            return (((long)(InternalSample(26)) << 27) + InternalSample(27)) * DOUBLE_UNIT;
        }

        private int InternalSample(int bits)
        {
            state = (state * multiplier + addend) & mask;
            return (int)(state >> (48 - bits));
        }

        public override int Next()
        {
            return InternalSample(31);
        }

        private double GetSampleForLargeRange()
        {
            int result = InternalSample(31);
            bool negative = InternalSample(1) == 1;
            if (negative)
            {
                result = -result;
            }
            double d = result;
            d += int.MaxValue - 1;
            d /= 2 * (uint)int.MaxValue - 1;
            return d;
        }

        public override int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    $"{nameof(minValue)} cannot be larger than ${nameof(maxValue)}");
            }

            long range = (long)maxValue - minValue;

            if (range <= (long)int.MaxValue)
            {
                return (int)(Sample() * range) + minValue;
            }
            else
            {
                return (int)((long)(GetSampleForLargeRange() * range) + minValue);
            }
        }

        public override void NextBytes(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(
                    $"{nameof(buffer)} cannot be null");
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(InternalSample(31) % (byte.MaxValue + 1));
            }
        }
    }
}
