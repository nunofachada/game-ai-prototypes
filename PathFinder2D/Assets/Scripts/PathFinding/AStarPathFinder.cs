/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;

namespace LibGameAI.PathFinding
{
    /// <summary>
    /// A path finder implemented with the A* algorithm.
    /// </summary>
    /// <remarks>
    /// Optimizations to be done (some are code-related with others):
    /// TODO Use a heap/priority queue (priority heap) data structure for the
    /// open and closed nodes.
    /// TODO Either make NodeRecord structs (and update surrounding code
    /// appropriately) or use an object pool of NodeRecords.
    /// TODO Avoid always getting node record from the dictionary, just pull
    /// it once onto a local variable and use that.
    /// TODO Reuse the heuristic value from previously existing node records.
    /// </remarks>
    public class AStarPathFinder : IPathFinder
    {
        // Auxiliary collections
        private List<NodeRecord> open, closed;
        private IDictionary<int, NodeRecord> nodeRecords;
        private Stack<IConnection> path;

        // Current goal node
        private int currentGoal;

        // Node comparer
        private Comparison<NodeRecord> comparer;

        // Heuristic in use
        private Func<int, float> heuristics;

        /// <summary>
        /// This private class is used to keep node records for the path
        /// finding algorithm.
        /// </summary>
        private class NodeRecord : IComparable<NodeRecord>
        {
            public int Node { get; }
            public IConnection Connection { get; set; }
            public float CostSoFar { get; set; }
            public float EstimatedTotalCost { get; set; }

            public NodeRecord(int node)
            {
                Node = node;
                Connection = null;
                CostSoFar = 0.0f;
                EstimatedTotalCost = 0.0f;
            }

            public int CompareTo(NodeRecord other)
            {
                return Math.Sign(
                    EstimatedTotalCost - other.EstimatedTotalCost);
            }
        }

        private int CompareByETC(NodeRecord nr1, NodeRecord nr2)
        {
            return nr1.EstimatedTotalCost.CompareTo(nr2.EstimatedTotalCost);
        }

        private int CompareByGoalAndThenETC(NodeRecord nr1, NodeRecord nr2)
        {
            if (nr1.Node == currentGoal) return -1;
            if (nr2.Node == currentGoal) return 1;
            return nr1.EstimatedTotalCost.CompareTo(nr2.EstimatedTotalCost);
        }

        /// <summary>
        /// Create a new A* path finder.
        /// </summary>
        /// <param name="heuristics">Heuristic function to use.</param>
        /// <param name="earlyExit">
        /// Stop as soon as goal is found in open list with the possibility of
        /// getting a costlier path?
        /// </param>
        public AStarPathFinder(
            Func<int, float> heuristics, bool earlyExit = false)
        {
            // Node comparer to use
            if (earlyExit)
                comparer = CompareByGoalAndThenETC;
            else
                comparer = CompareByETC;

            // Keep reference to the heuristic function
            this.heuristics = heuristics;

            // Initialize the auxiliary collections
            open = new List<NodeRecord>();
            closed = new List<NodeRecord>();
            nodeRecords = new Dictionary<int, NodeRecord>();
            path = new Stack<IConnection>();
        }

        public IEnumerable<int> OpenNodes
        {
            get
            {
                foreach (NodeRecord nr in open)
                {
                    yield return nr.Node;
                }
            }
        }

        public IEnumerable<int> ClosedNodes
        {
            get
            {
                foreach (NodeRecord nr in closed)
                {
                    yield return nr.Node;
                }
            }
        }

        /// <summary>
        /// Find a path between start and goal nodes.
        /// </summary>
        /// <param name="graph">Graph where to perform search.</param>
        /// <param name="start">Start node.</param>
        /// <param name="goal">Goal node.</param>
        /// <returns>
        /// An enumerable containing the connections that constitute
        /// a path from start to goal.
        /// </returns>
        public IEnumerable<IConnection> FindPath(
            IGraph graph, int start, int goal)
        {
            // Current node
            int current;

            // Keep global reference to the current goal
            currentGoal = goal;

            // Clear collections
            open.Clear();
            closed.Clear();
            nodeRecords.Clear();

            // Initialize the record for the start node
            nodeRecords[start] = new NodeRecord(start);
            nodeRecords[start].EstimatedTotalCost = heuristics(start);

            // "Current" node is start node
            current = start;

            // Initialize the open list by adding the starting node
            open.Add(nodeRecords[start]);

            // Iterate through processing each node
            while (open.Count > 0)
            {

                // Find element with smallest estimated total cost so far in
                // the open list
                open.Sort(comparer);
                current = open[0].Node;

                // If it is end node, break out of node processing loop
                if (current == goal) break;

                // Otherwise get the node outgoing connections
                foreach (IConnection conn in graph.GetConnections(current))
                {
                    // Index of node record in the open and closed lists
                    int nrIndex;

                    // Function to find specific node in a list
                    Predicate<NodeRecord> findNodePred =
                        nr => nr.Node == conn.ToNode;

                    // Get cost estimate for the "to node"
                    float toNodeCost =
                        nodeRecords[current].CostSoFar + conn.Cost;

                    // Check if the node is in the closed list
                    nrIndex = closed.FindIndex(findNodePred);
                    if (nrIndex >= 0)
                    {
                        if (nodeRecords[conn.ToNode].CostSoFar <= toNodeCost)
                            continue;
                        closed.Remove(nodeRecords[conn.ToNode]);
                        open.Add(nodeRecords[conn.ToNode]);
                    }
                    else if ((nrIndex = open.FindIndex(findNodePred)) >= 0)
                    {
                        // If node is in the open list...
                        // ...and we find a worse route, also skip
                        if (open[nrIndex].CostSoFar <= toNodeCost)
                            continue;
                    }
                    else
                    {
                        // If we're here we've got an unvisited node, so make
                        // a record for it
                        nodeRecords[conn.ToNode] = new NodeRecord(conn.ToNode);
                    }

                    // We're here if we need to update the node
                    // Update the cost and connection
                    nodeRecords[conn.ToNode].CostSoFar = toNodeCost;
                    nodeRecords[conn.ToNode].Connection = conn;
                    nodeRecords[conn.ToNode].EstimatedTotalCost =
                        toNodeCost + heuristics(conn.ToNode);

                    // Add node to the open list if it's not already there
                    if ((nrIndex = open.FindIndex(findNodePred)) < 0)
                        open.Add(nodeRecords[conn.ToNode]);
                }

                // We've finished looking at the connections for the current
                // node, so add it to the closed list and remove it from the
                // open list
                open.Remove(nodeRecords[current]);
                closed.Add(nodeRecords[current]);
            }

            // We're here if we've either found the goal, or if we've no more
            // nodes to search

            if (current != goal)
            {
                // We've run out of nodes without finding the goal, so there's
                // no solution
                return null;
            }
            else
            {
                // Compile the list of connections in the path
                path.Clear();

                // Work back along the path, accumulating connections
                while (current != start)
                {
                    path.Push(nodeRecords[current].Connection);
                    current = nodeRecords[current].Connection.FromNode;
                }

                // Return the path
                return path;
            }
        }
    }
}
