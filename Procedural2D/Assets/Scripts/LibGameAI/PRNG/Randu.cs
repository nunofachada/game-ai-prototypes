using System;

namespace LibGameAI.PRNG
{
    public class Randu : Random
    {
        private int state;

        private const int A = 65539;
        private const int M = 0x7FFFFFFF; // 2^31
        private const double DOUBLE_UNIT = 1.0 / (1L << 53);

        public Randu() : this(Environment.TickCount)
        {
        }

        public Randu(int seed)
        {
            // RANDU doesn't work with seed == 0
            if (seed == 0) seed = int.MaxValue / 2;
            state = seed;
        }

        protected override double Sample()
        {
            return (((long)(Next()) << 27) + Next()) * DOUBLE_UNIT;
        }

        public override int Next()
        {
            state = (A * state) % M;
            return state;
        }

        private double GetSampleForLargeRange()
        {
            int result = Next();
            bool negative = Next() % 2 == 0;
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
