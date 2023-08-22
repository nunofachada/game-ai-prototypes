/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;

namespace LibGameAI.PRNG
{
    /// <summary>
    /// Example of a bad random number generator for testing purposes.
    /// </summary>
    /// <remarks>
    /// Useful links:
    /// <list type="bullet">
    /// <item><description>
    /// <seealso href="http://en.wikipedia.org/wiki/RANDU">
    /// RANDU - Wikipedia
    /// </seealso>
    /// </description></item>
    /// <item><description>
    /// <seealso href="https://scruss.com/wordpress/wp-content/uploads/2013/06/randu.c">
    /// A bad implementation in C of a bad algorithm
    /// </seealso>
    /// </description></item>
    /// <item><description>
    /// <seealso href="https://github.com/fakenmc/pphpc/blob/master/java/src/org/laseeb/pphpc/RanduRNG.java">
    /// Implementation of the RANDU random number generator in Java
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
    public class Randu : Random
    {
        private ulong state;

        private const ulong A = 65539;
        private const ulong M = 0x7FFFFFFF; // 2^31

        public Randu() : this(Environment.TickCount)
        {
        }

        public Randu(int seed)
        {
            // RANDU doesn't work with seed == 0
            if (seed == 0) seed = int.MaxValue / 2;
            state = (ulong)seed;
        }

        protected override double Sample()
        {
            return InternalSample() * (1.0 / int.MaxValue);
        }

        private int InternalSample()
        {
            state = (A * state) % M;
            return (int)(state & M);
        }

        public override int Next()
        {
            return InternalSample();
        }

        private double GetSampleForLargeRange()
        {
            int result = InternalSample();
            bool negative = InternalSample() % 2 == 0;
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
                buffer[i] = (byte)(Next() % (byte.MaxValue + 1));
            }
        }
    }
}
