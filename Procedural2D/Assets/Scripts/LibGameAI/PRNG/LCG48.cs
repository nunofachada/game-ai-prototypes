using System;

namespace LibGameAI.PRNG
{
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
