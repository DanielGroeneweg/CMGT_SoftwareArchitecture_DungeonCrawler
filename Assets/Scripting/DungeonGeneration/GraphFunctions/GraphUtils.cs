using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class GraphUtils
{
    /// <summary>
    /// Calculates the costs for the entire graph starting at the start node
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    public static Dictionary<RectInt, float> GetCosts(RectInt start, Graph<RectInt> graph)
    {
        HashSet<RectInt> discovered = new HashSet<RectInt>();
        Dictionary<RectInt, RectInt> nodeParents = new Dictionary<RectInt, RectInt>();
        Dictionary<RectInt, float> nodeCosts = new Dictionary<RectInt, float>();
        discovered.Clear();

        nodeCosts.Add(start, 0);

        List<(RectInt node, float priority)> priorityQueue = new List<(RectInt node, float priority)>();
        priorityQueue.Add((start, 0));

        while (priorityQueue.Count > 0)
        {
            RectInt node = priorityQueue[priorityQueue.Count - 1].node;
            discovered.Add(node);
            priorityQueue.RemoveAt(priorityQueue.Count - 1);

            foreach (RectInt neighbor in graph.GetNeighbors(node))
            {
                if (nodeParents.ContainsKey(neighbor))
                {
                    float currentCost = nodeCosts[neighbor];
                    float newPossibleCost = nodeCosts[node] + 1;

                    if (newPossibleCost < currentCost)
                    {
                        nodeCosts[neighbor] = newPossibleCost;
                        nodeParents[neighbor] = node;
                    }
                }

                else
                {
                    if (discovered.Contains(neighbor)) continue;

                    nodeParents.Add(neighbor, node);
                    nodeCosts.Add(neighbor, nodeCosts[node] + 1);
                    priorityQueue.Add((neighbor, nodeCosts[neighbor]));
                }
            }

            priorityQueue = priorityQueue.OrderByDescending(p => p.priority).ToList();
        }

        return nodeCosts;
    }
    /// <summary>
    /// Creates a path from start till end using the AStar searching method
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="graph"></param>
    /// <param name="cellSize"></param>
    /// <returns></returns>
    public static List<RectInt> FindPath(RectInt start, RectInt end, Graph<RectInt> graph, Vector2 cellSize)
    {
        HashSet<RectInt> discovered = new HashSet<RectInt>();
        Dictionary<RectInt, RectInt> nodeParents = new Dictionary<RectInt, RectInt>();
        Dictionary<RectInt, float> nodeCosts = new Dictionary<RectInt, float>();
        discovered.Clear();

        nodeCosts.Add(start, 0);

        List<(RectInt node, float priority)> priorityQueue = new List<(RectInt node, float priority)>();
        priorityQueue.Add((start, 0));

        while (priorityQueue.Count > 0)
        {
            RectInt node = priorityQueue[priorityQueue.Count - 1].node;
            discovered.Add(node);
            priorityQueue.RemoveAt(priorityQueue.Count - 1);

            if (node == end) return ReconstructPath(nodeParents, start, end);

            foreach (RectInt neighbor in graph.GetNeighbors(node))
            {
                if (nodeParents.ContainsKey(neighbor))
                {
                    float currentCost = nodeCosts[neighbor];
                    float newPossibleCost = nodeCosts[node] + 1;

                    if (newPossibleCost < currentCost)
                    {
                        nodeCosts[neighbor] = newPossibleCost;
                        nodeParents[neighbor] = node;
                    }
                }

                else
                {
                    if (discovered.Contains(neighbor)) continue;

                    nodeParents.Add(neighbor, node);
                    nodeCosts.Add(neighbor, nodeCosts[node] + 1);
                    priorityQueue.Add((neighbor, nodeCosts[neighbor] + Heuristic(neighbor, end, cellSize)));
                }
            }

            priorityQueue = priorityQueue.OrderByDescending(p => p.priority).ToList();
        }

        return new List<RectInt>();
    }
    private static float Heuristic(RectInt from, RectInt to, Vector2 cellSize)
    {
        return Vector2.Distance(from.center / cellSize, to.center / cellSize);
    }
    private static List<RectInt> ReconstructPath(Dictionary<RectInt, RectInt> parentMap, RectInt start, RectInt end)
    {
        List<RectInt> path = new List<RectInt>();
        RectInt currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = parentMap[currentNode];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }
}
