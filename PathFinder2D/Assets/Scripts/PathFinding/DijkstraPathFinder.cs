/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

namespace LibGameAI.PathFinding
{
    /// <summary>
    /// A path finder implemented with the Dijkstra algorithm. Always finds
    /// the shortest path.
    /// </summary>
    public class DijkstraPathFinder : IPathFinder
    {
        // Auxiliary collections
        private List<NodeRecord> open, closed;
        private IDictionary<int, NodeRecord> nodeRecords;
        private Stack<IConnection> path;

        // This private class is used to keep node records for the shortest
        // path algorithm.
        private class NodeRecord : IComparable<NodeRecord>
        {
            public int Node { get; }
            public IConnection Connection { get; set; }
            public float CostSoFar { get; set; }

            public NodeRecord(int node)
            {
                Node = node;
                Connection = null;
                CostSoFar = 0.0f;
            }

            public int CompareTo(NodeRecord other)
            {
                return Math.Sign(CostSoFar - other.CostSoFar);
            }
        }

        /// <summary>
        /// Create a new Dijkstra shortest path finder.
        /// </summary>
        public DijkstraPathFinder()
        {
            open = new List<NodeRecord>();
            closed = new List<NodeRecord>();
            nodeRecords = new Dictionary<int, NodeRecord>();
            path = new Stack<IConnection>();
        }

        /// <summary>
        /// Find shortest path between start and goal nodes.
        /// </summary>
        /// <param name="graph">Graph where to perform search.</param>
        /// <param name="start">Start node.</param>
        /// <param name="goal">Goal node.</param>
        /// <returns>
        /// An enumerable containing the connections that constitute
        /// the shortest path from start to goal.
        /// </returns>
        public IEnumerable<IConnection> FindPath(
            IGraph graph, int start, int goal)
        {
            // Current node
            int current;

            // Clear collections
            open.Clear();
            closed.Clear();
            nodeRecords.Clear();

            // Initialize the record for the start node
            nodeRecords[start] = new NodeRecord(start);

            // "Current" node is start node
            current = start;

            // Initialize the open list by adding the starting node
            open.Add(nodeRecords[start]);

            // Iterate through processing each node
            while (open.Count > 0)
            {

                // Find element with smallest cost so far in the open list
                open.Sort();
                current = open[0].Node;

                // If it is end node, break out of node processing loop
                if (current == goal) break;

                // Otherwise get the node outgoing connections
                foreach (IConnection conn in graph.GetConnections(current))
                {
                    // Index of node record in the open and closed lists
                    int nrIndex;

                    // The node record itself
                    NodeRecord nodeRec;

                    // Function to find specific node in a list
                    Predicate<NodeRecord> findNodePred =
                        new Predicate<NodeRecord>(nr => nr.Node == conn.ToNode);

                    // Get cost estimate for the "to node"
                    float toNodeCost =
                        nodeRecords[current].CostSoFar + conn.Cost;

                    // Skip if the node is closed
                    nrIndex = closed.FindIndex(findNodePred);
                    if (nrIndex >= 0) continue;

                    // If node is open...
                    nrIndex = open.FindIndex(findNodePred);
                    if (nrIndex >= 0)
                    {
                        // ...and we find a worse route, also skip
                        if (open[nrIndex].CostSoFar <= toNodeCost) continue;

                        // Otherwise, keep node record
                        nodeRec = open[nrIndex];
                    }
                    else
                    {
                        // If we're here we've got an unvisited node, so make
                        // a record for it
                        nodeRec = new NodeRecord(conn.ToNode);
                        nodeRecords[conn.ToNode] = nodeRec;

                        // And add it to the open list
                        open.Add(nodeRec);
                    }

                    // We're here if we need to update the node
                    // Update the cost and connection
                    nodeRec.CostSoFar = toNodeCost;
                    nodeRec.Connection = conn;
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
