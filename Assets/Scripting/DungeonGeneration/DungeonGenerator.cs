using UnityEngine;
using System.Collections.Generic;
using System;
using NaughtyAttributes;
[Serializable]
public class GridLayout
{
    public int rows;
    public int columns;
}
public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Stats")]
    [SerializeField] private int cellSize;
    [SerializeField] private GridLayout grid;
    [SerializeField] private int minPathCost;
    [SerializeField] private bool excludeVertical;
    [SerializeField] private bool excludeHorizontal;
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
    private List<RectInt> pathToEnd = new();
    private void GenerateDungeon()
    {
        graph = new Graph<RectInt>();
        if (!useSeed) seed = Environment.TickCount;
        random = new System.Random(seed);

        pathToEnd.Clear();
        costs.Clear();
        cells.Clear();
        GenerateBounds();
        GenerateCells();
        GenerateStartAndEnd();
    }
    private void GenerateBounds()
    {
        bounds = new RectInt(0, 0, cellSize, cellSize);
    }
    private void GenerateCells()
    {
        // Create all Cells
        for (int i = 0; i < grid.rows; i++)
        {
            for (int j = 0; j < grid.columns; j++)
            {
                RectInt cell = new RectInt(cellSize * i, cellSize * j, cellSize, cellSize);
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
                if ((cellB.position.x - cellA.position.x == cellSize && cellA.position.y == cellB.position.y) || 
                    (cellB.position.y - cellA.position.y == cellSize && cellA.position.x == cellB.position.x) ||
                    (cellA.position.x - cellB.position.x == cellSize && cellA.position.y == cellB.position.y) ||
                    (cellA.position.y - cellB.position.y == cellSize && cellA.position.x == cellB.position.x))
                    graph.AddNeighbor(cellA, cellB);
            }
        }
    }
    private void GenerateStartAndEnd()
    {
        int index = random.Next(cells.Count);
        startCell = cells[index];
        cells.RemoveAt(index);

        costs = GraphUtils.GetCosts(startCell, graph);

        List<RectInt> cellsCopy = cells;
        for(int i = cellsCopy.Count - 1; i >= 0; i--)
        {
            if (costs[cellsCopy[i]] < minPathCost) cellsCopy.Remove(cellsCopy[i]);
            else if (excludeHorizontal && cellsCopy[i].center.y == startCell.center.y) cellsCopy.Remove(cellsCopy[i]);
            else if (excludeVertical && cellsCopy[i].center.x == startCell.center.x) cellsCopy.Remove(cellsCopy[i]);
        }

        index = random.Next(cellsCopy.Count);
        endCell = cells[index];

        pathToEnd = GraphUtils.FindPath(startCell, endCell, graph, cellSize);
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
            float value = costs[cell] >= minPathCost ? 1 : 0;
            Color color = new Color(1-value, value, 0);

            if (excludeHorizontal && cell.center.y == startCell.center.y) AlgorithmsUtils.DebugRectInt(cell, Color.red);
            else if (excludeVertical && cell.center.x == startCell.center.x) AlgorithmsUtils.DebugRectInt(cell, Color.red);
            else AlgorithmsUtils.DebugRectInt(cell, color);
        }

        AlgorithmsUtils.DebugRectInt(bounds, Color.white);

        AlgorithmsUtils.DebugRectInt(startCell, Color.blue);

        AlgorithmsUtils.DebugRectInt(endCell, Color.magenta);

        for(int i = pathToEnd.Count - 1; i > 0; i--)
        {
            Debug.DrawLine(pathToEnd[i].center, pathToEnd[i-1].center, Color.yellow);
        }
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