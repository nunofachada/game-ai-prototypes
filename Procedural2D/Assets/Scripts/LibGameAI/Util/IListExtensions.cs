/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;

namespace LibGameAI.Util
{

    public static class IListExtensions
    {
        private static readonly Random innerRandom;

        static IListExtensions()
        {
            innerRandom = new Random();
        }

        // Fisher–Yates shuffling.
        // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        public static void Shuffle<T>(this IList<T> list, Random random = null)
        {
            random ??= innerRandom;
            for (int i = list.Count - 1; i >= 1; i--)
            {
                T aux;
                int j = random.Next(i + 1);
                aux = list[j];
                list[j] = list[i];
                list[i] = aux;
            }
        }

    }
}
