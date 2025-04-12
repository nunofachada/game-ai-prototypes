/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;

namespace LibGameAI.Util
{
    /// <summary>
    /// Useful math functions.
    /// </summary>
    public static class MMath
    {
        /// <summary>
        /// Check whether an integer is prime or not.
        /// </summary>
        /// <param name="n">Integer to check for primality.</param>
        /// <returns>True if integer is prime, false otherwise.</returns>
        /// <remarks>
        /// Basic implementation as described in
        /// <seealso href="https://en.wikipedia.org/wiki/Primality_test">
        /// Wikipedia.
        /// </seealso>
        /// </remarks>
        public static bool IsPrime(int n)
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

        /// <summary>
        /// Normalize a vector between the specified limits.
        /// </summary>
        /// <param name="vector">Vector to normalize, in-place.</param>
        /// <param name="min">Minimum normalization range.</param>
        /// <param name="max">Maximum normalization range.</param>
        public static void Normalize(float[] vector, float min, float max)
        {
            float localMin = vector[0];
            float localMax = vector[0];
            float origRange, newRange;

            // Determine minimum and maximum heights
            foreach (float h in vector)
            {
                if (h < localMin) localMin = h;
                else if (h > localMax) localMax = h;
            }

            // Determine original and new ranges
            origRange = localMax - localMin;
            newRange = max - min;

            // Perform actual normalization
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] = (vector[i] - localMin) * newRange / origRange + min;
            }
        }

        /// <summary>
        /// Return cumulative sum of given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>
        /// A new vector with the cumulative sum of the input vector.
        /// </returns>
        public static float[] CumSum(float[] vector)
        {
            float total = 0.0f;
            float acumul = 0.0f;
            float[] csum = new float[vector.Length];

            for (int i = 0; i < vector.Length; i++)
            {
                total += vector[i];
            }

            for (int i = 0; i < csum.Length; i++)
            {
                acumul += vector[i] / total;
                csum[i] = acumul;
            }

            return csum;
        }


        public static int Round(float f) => (int)MathF.Round(f);
    }
}