using System.Collections.Generic;
using UnityEngine;

public class Pathfinder2 : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    // The 4 possible directions for neighbors
    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // up
        new Vector2Int(1, 0),   // right
        new Vector2Int(0, -1),  // down
        new Vector2Int(-1, 0)   // left
    };

    public List<GridNode> FindPath(Vector3 startPos, Vector3 endPos)
    {
        if (!gridManager.IsInitialized)
            gridManager.InitializeGrid();

        Vector2Int start = WorldToGridIndex(startPos);
        Vector2Int goal = WorldToGridIndex(endPos);

        if (!IsValidIndex(start) || !IsValidIndex(goal))
            return null;

        // 1. Setup and data structures
        List<Vector2Int> openSet = new();
        HashSet<Vector2Int> closedSet = new();
        Dictionary<Vector2Int, float> gScore = new();
        Dictionary<Vector2Int, float> fScore = new();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();

        // 2. Initialize the Start Node
        openSet.Add(start);
        gScore[start] = 0f;
        fScore[start] = Heuristic(start, goal);

        // 3. Main A* loop
        while (openSet.Count > 0)
        {
            Vector2Int current = GetLowestFScore(openSet, fScore);

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;

                // Bounds check
                if (!IsValidIndex(neighbor))
                    continue;

                // Already processed
                if (closedSet.Contains(neighbor))
                    continue;

                // Walkability check
                GridNode node = gridManager.GetNode(neighbor.x, neighbor.y);
                if (!node.Walkable)
                    continue;

                // Tentative cost calculation
                float tentativeG = gScore[current] + node.Weight;

                // Only add neighbor to openSet if it's not already there
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeG >= gScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    continue;
                }

                // Record best path so far
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeG;
                fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);
            }
        }

        // No valid path found
        return null;
    }

    private Vector2Int GetLowestFScore(List<Vector2Int> openSet, Dictionary<Vector2Int, float> fScore)
    {
        Vector2Int best = openSet[0];
        float bestScore = fScore.GetValueOrDefault(best, float.MaxValue);

        foreach (Vector2Int node in openSet)
        {
            float score = fScore.GetValueOrDefault(node, float.MaxValue);
            if (score < bestScore)
            {
                best = node;
                bestScore = score;
            }
        }
        return best;
    }

    private List<GridNode> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<GridNode> path = new() { gridManager.GetNode(current.x, current.y) };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, gridManager.GetNode(current.x, current.y));
        }
        return path;
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private Vector2Int WorldToGridIndex(Vector3 worldPos)
    {
        var settings = gridManager.GridSettings;
        int x = Mathf.RoundToInt(worldPos.x / settings.NodeSize);
        int y = settings.UseXZPlane
            ? Mathf.RoundToInt(worldPos.z / settings.NodeSize)
            : Mathf.RoundToInt(worldPos.y / settings.NodeSize);
        return new Vector2Int(x, y);
    }

    private bool IsValidIndex(Vector2Int idx)
    {
        var settings = gridManager.GridSettings;
        return idx.x >= 0 && idx.x < settings.GridSizeX && idx.y >= 0 && idx.y < settings.GridSizeY;
    }
}