using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using NaughtyAttributes;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Data;
[Serializable]
public class GridLayout
{
    public int columns;
    public int rows;
}
public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Stats")]
    [SerializeField] private Vector2Int dungeonSize;
    [SerializeField] private GridLayout grid;
    [SerializeField] private int minPathCost;
    [Header("Randomization")]
    [SerializeField] private bool useSeed = false;
    [ShowIf("useSeed", ComparisonTypes.Equals, true)]
    [SerializeField] private int seed;

    private List<RectInt> cells = new();
    private RectInt bounds;
    private RectInt startCell;
    private RectInt endCell;

    private System.Random random;
    private Graph<RectInt> graph;
    private Dictionary<RectInt, float> costs = new Dictionary<RectInt, float>();
    private void GenerateDungeon()
    {
        graph = new Graph<RectInt>();
        if (!useSeed) seed = Environment.TickCount;
        random = new System.Random(seed);

        costs.Clear();
        cells.Clear();
        GenerateBounds();
        GenerateCells();
        GenerateStartAndEnd();
    }
    private void GenerateBounds()
    {
        bounds = new RectInt(0, 0, dungeonSize.x, dungeonSize.y);
    }
    private void GenerateCells()
    {
        // Create all Cells
        int cellWidth = dungeonSize.x / grid.rows;
        int cellHeight = dungeonSize.y / grid.columns;
        for (int i = 0; i < grid.rows; i++)
        {
            for (int j = 0; j < grid.columns; j++)
            {
                RectInt cell = new RectInt(cellWidth * i, cellHeight * j, cellWidth, cellHeight);
                graph.AddNode(cell);
                cells.Add(cell);
            }
        }

        // Mark all Cell neighbors
        for (int i = 0; i < cells.Count; i++)
        {
            RectInt cellA = cells[i];
            for (int j = i + 1; j < cells.Count; j++)
            {
                RectInt cellB = cells[j];
                if ((cellB.position.x - cellA.position.x == cellWidth && cellA.position.y == cellB.position.y) || 
                    (cellB.position.y - cellA.position.y == cellHeight && cellA.position.x == cellB.position.x) ||
                    (cellA.position.x - cellB.position.x == cellWidth && cellA.position.y == cellB.position.y) ||
                    (cellA.position.y - cellB.position.y == cellHeight && cellA.position.x == cellB.position.x))
                    graph.AddNeighbor(cellA, cellB);
            }
        }
    }
    private void GenerateStartAndEnd()
    {
        int index = random.Next(cells.Count);
        startCell = cells[index];
        cells.RemoveAt(index);

        costs = Dijkstra(startCell);

        List<RectInt> cellsCopy = cells;
        for(int i = cellsCopy.Count - 1; i >= 0; i--)
        {
            if (costs[cellsCopy[i]] < minPathCost) cellsCopy.Remove(cellsCopy[i]);
        }

        index = random.Next(cellsCopy.Count);
        endCell = cells[index];
    }
    public Dictionary<RectInt, float> Dijkstra(RectInt start)
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

            //if (node == end) return ReconstructPath(nodeParents, start, end);

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
    List<RectInt> ReconstructPath(Dictionary<RectInt, RectInt> parentMap, RectInt start, RectInt end)
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
    private void Update()
    {
        foreach (RectInt cell in costs.Keys)
        {
            int maxDist = grid.columns - 1 + grid.rows - 1;
            float value = (1f / maxDist) * costs[cell];
            Color color = new Color(1-value, value, 0);
            AlgorithmsUtils.DebugRectInt(cell, color);
        }

        AlgorithmsUtils.DebugRectInt(bounds, Color.white);

        AlgorithmsUtils.DebugRectInt(startCell, Color.blue);

        AlgorithmsUtils.DebugRectInt(endCell, Color.magenta);
    }
    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void Generate()
    {
        GenerateDungeon();
    }
    private void OnValidate()
    {
        if (minPathCost > grid.columns / 2 + grid.rows / 2) minPathCost = grid.columns / 2 + grid.rows / 2;
    }
}