/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

namespace LibGameAI.PathFinding
{
    public interface IConnection
    {
        float Cost { get; }
        int FromNode { get; }
        int ToNode { get; }
    }
}
