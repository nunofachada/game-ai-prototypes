/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System.Collections.Generic;

namespace LibGameAI.PathFinding
{
    public interface IPathFinder
    {
        IEnumerable<int> OpenNodes { get; }

        IEnumerable<int> ClosedNodes { get; }

        IEnumerable<IConnection> FindPath(IGraph graph, int start, int goal);
    }
}
