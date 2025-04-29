/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;

namespace LibGameAI.Util
{
    public static class RandomExtensions
    {
        public static float NextFloat(this Random random) => (float)random.NextDouble();

        public static float Range(this Random random, float min, float max) =>
            (max - min) * (float)random.NextDouble() + min;
    }
}