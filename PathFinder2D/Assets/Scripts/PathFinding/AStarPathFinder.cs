/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LibGameAI.PathFinding
{
    public class AStarPathFinder : IPathFinder
    {
        // Auxiliary collections
        private List<NodeRecord> open, closed;
        private IDictionary<int, NodeRecord> nodeRecords;
        private Stack<IConnection> path;

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

        /// <summary>
        /// Create a new A* path finder.
        /// </summary>
        /// <param name="heuristics">Heuristic function to use.</param>
        public AStarPathFinder(Func<int, float> heuristics)
        {
            // Keep reference to the heuristic function
            this.heuristics = heuristics;

            // Initialize the auxiliary collections
            open = new List<NodeRecord>();
            closed = new List<NodeRecord>();
            nodeRecords = new Dictionary<int, NodeRecord>();
            path = new Stack<IConnection>();

        }

        public IEnumerable<int> FillOpen()
        {
            foreach (NodeRecord nr in open)
            {
                yield return nr.Node;
            }
        }

        public IEnumerable<int> FillClosed()
        {
            foreach (NodeRecord nr in closed)
            {
                yield return nr.Node;
            }
        }

        /// <summary>
        /// Find a path between start and goal nodes.
        /// </summary>
        /// <param name="graph">Graph where to perform search.</param>
        /// <param name="start">Start node.</param>
        /// <param name="goal">Goal node.</param>
        /// <param name="heuristics">Heuristics for each node.</param>
        /// <returns>
        /// An enumerable containing the connections that constitute
        /// a path from start to goal.
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
                open.Sort();
                current = open[0].Node;

                // If it is end node, break out of node processing loop
                // if (current == goal) break;

                // Otherwise get the node outgoing connections
                foreach (IConnection conn in graph.GetConnections(current))
                {
                    // Index of node record in the open and closed lists
                    int nrIndex;

                    // Function to find specific node in a list
                    Predicate<NodeRecord> findNodePred =
                        new Predicate<NodeRecord>(nr => nr.Node == conn.ToNode);

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
                    else if (conn.ToNode == goal)
                    {
                        nodeRecords[conn.ToNode] = new NodeRecord(conn.ToNode);
                        nodeRecords[conn.ToNode].CostSoFar = toNodeCost;
                        nodeRecords[conn.ToNode].Connection = conn;
                        nodeRecords[conn.ToNode].EstimatedTotalCost =
                            toNodeCost + heuristics(conn.ToNode);
                        UnityEngine.Debug.Log("Early Exit Found!");
                        return DeterminePath(conn.ToNode, start, goal);
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

            return DeterminePath(current, start, goal);
        }

        private Stack<IConnection> DeterminePath(int node, int start, int goal)
        {
            if (node != goal)
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
                while (node != start)
                {
                    path.Push(nodeRecords[node].Connection);
                    node = nodeRecords[node].Connection.FromNode;
                }

                // Return the path
                return path;
            }
        }
    }
}
