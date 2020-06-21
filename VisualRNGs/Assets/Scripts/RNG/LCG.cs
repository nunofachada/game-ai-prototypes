using System;

namespace LibGameAI.RNG
{
    public class LCG : Random
    {
        private long state;

        public LCG() : this(System.DateTime.Now.Ticks)
        {
        }

        public LCG(int seed) : this((long)seed << 32 | (long)seed)
        {

        }
        public LCG(long seed)
        {
            state = seed;
        }

        protected override double Sample()
        {
            return (((long)Next(0, 26) << 27) + Next(0, 27))
                / (double)(1L << 53);
        }

        public override int Next()
        {
            int bits = 32;
            state = (state * 0x5DEECE66DL + 0xBL) & ((1L << 48) - 1);
            long next = state >> (48 - bits);
            return (int)(next & 0x7FFFFFFF);
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
                // Bug! Stackoverflow!
                return (int)(Sample() * range) + minValue;
            }
            else
            {
                // Bug! Stackoverflow!
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
