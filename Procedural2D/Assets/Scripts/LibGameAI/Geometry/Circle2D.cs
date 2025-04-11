/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

namespace LibGameAI.Geometry
{
    public readonly struct Circle2D
    {
        public float X { get; }
        public float Y { get; }
        public float Radius { get; }

        public Circle2D(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

    }
}