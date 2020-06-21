using System;

namespace LibGameAI.RNG
{
    public class Halton : Random
    {
        private int basePrime;
        private int index;

        public Halton(int basePrime, bool skipFirstN = true)
        {
            if (!IsPrime(basePrime))
            {
                throw new ArgumentException(
                    $"{nameof(basePrime)} must be a prime number.");
            }
            this.basePrime = basePrime;

            if (skipFirstN)
                index = basePrime + 1;
            else
                index = 0;
        }

        // https://en.wikipedia.org/wiki/Primality_test
        private static bool IsPrime(int n)
        {
            if (n <= 3)
                return n > 1;
            else if (n % 2 == 0 || n % 3 == 0)
                return false;

            int i = 5;

            while (i * i <= n)
            {
                if (n % i == 0 || n % (i + 2) == 0)
                    return false;
                i += 6;
            }

            return true;
        }

        public static double Sequence(int basePrime, int index)
        {
            if (!IsPrime(basePrime))
            {
                throw new ArgumentException(
                    $"{nameof(basePrime)} must be a prime number.");
            }
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
