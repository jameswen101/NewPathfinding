using NUnit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PathFinder1 : MonoBehaviour
{
    [SerializeField] private GridManager1 gridManager;
    public enum PathfindingType
    {
        Unweighted,
        Weighted,
        BruteForce,
        Naive,
    }

    public GridNode StartNode;
    public GridNode EndNode;

    List<GridNode> openSet = new List<GridNode>(); //should we also have a closedSet?
    List<GridNode> closedSet = new List<GridNode>();
    Dictionary<GridNode, int> costSoFar = new Dictionary<GridNode, int>(); //gCost (starts at 0)
    Dictionary<GridNode, int> estimatedTotalCost = new Dictionary<GridNode, int>(); //fCost; costSoFar + Heuristic = estimatedTotalCost?
    Dictionary<GridNode, GridNode> cameFrom = new Dictionary<GridNode, GridNode>(); //path reconstruction


    [Header("Pathfinding Settings")]
    [SerializeField] private PathfindingType pathfindingType = PathfindingType.Unweighted;
    [SerializeField, Range(0, 100)] private int framesPerStep = 10;
    [SerializeField] private bool visualizePathfinding = true;

    [Header("Visualization Colors")]
    [SerializeField] private Color startNodeColor = Color.green;
    [SerializeField] private Color endNodeColor = Color.red;
    [SerializeField] private Color currentPathColor = Color.yellow;
    [SerializeField] private Color visitedNodeColor = new Color(0.5f, 0.5f, 1f, 0.5f);
    [SerializeField] private Color unvisitedNodeColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
    [SerializeField] private Color finalPathColor = Color.cyan;
    [SerializeField] private Color currentNodeColor = Color.magenta;
    [SerializeField] private Color currentNeighborColor = new Color(1f, 0.5f, 0f, 0.5f); //orange
    [SerializeField] private Color explorationLineColor = new Color(1f, 1f, 0f, 0.3f); //semi-transparent yellow

    [Header("Visualization Settings")]
    [SerializeField] private int currentSeed = 0;
    [SerializeField] private bool useSeededRandom = true;
    [SerializeField] private float minWeight = 1f;
    [SerializeField] private float maxWeight = 10f;

    /*
    protected void FindPath(GridNode StartNode, GridNode EndNode)
    {
        List<GridNode> searchedNodes = new List<GridNode>(); //starts with nothing
        List<GridNode> nodesToSearch = new List<GridNode> { StartNode }; //starts with all
        List<GridNode> finalPath = new List<GridNode>(); //start with nothing

        StartNode.gCost = 0;
        StartNode.hCost = GetDistance(StartNode, EndNode);
        StartNode.fCost = GetDistance(StartNode, EndNode);

        while (nodesToSearch.Count > 0)
        {
            //decide the node search order
            GridNode nodeToSearch = nodesToSearch[0];
            GridNode bestNeighbor = StartNode;
            foreach (GridNode node in nodesToSearch)
            {
                if (node.fCost < gridManager.AllNodes[cellToSearch].fCost ||
                    (node.fCost == cells[cellToSearch].fCost && node.hCost == cells[cellToSearch].hCost))
                    //compare among cell (x-1), cell (x+1), cell (z-1), and cell (z+1)
                    if (node.WorldPosition.x == StartNode.WorldPosition.x-1)
                    {
                        bestNeighbor = node;
                        nodesToSearch[1] = node;
                    }
                    if (node.WorldPosition.x < StartNode.WorldPosition.x-1)
                    {

                    }
            }

        }

    }
    */

    public List<GridNode> CalculatePath(GridNode StartNode, GridNode EndNode) //once we changed to this version, the line disappeared
    {
        openSet.Add(StartNode);
        cameFrom[StartNode] = StartNode; // This works for marking the root

        Debug.Log($"Finding path from ({StartNode.WorldPosition.x}, {StartNode.WorldPosition.z}) to ({EndNode.WorldPosition.x}, {EndNode.WorldPosition.z})");

        while (openSet.Count > 0)
        {
            //find code with lowest f-score
            GridNode current = openSet[0];
            foreach (GridNode node in openSet) //does openSet only have 1 node?
            {
                if (estimatedTotalCost[node] < estimatedTotalCost[current] || //keyNode not found
   (estimatedTotalCost[node] == estimatedTotalCost[current] &&
    Heuristic(node, EndNode) < Heuristic(current, EndNode)))
                {
                    current = node;
                }
            }
            if (current.Equals(EndNode)) //when end node reached, stop searching
            {
                break;
            }
            openSet.Remove(current); //remove current node from nodes to explore
            if (closedSet.Contains(current)) continue;
            closedSet.Add(current);
        }

            // Reconstruct path
            List<GridNode> path = new List<GridNode>();
        GridNode pathNode = EndNode;

        while (true)
        {
            path.Add(pathNode);
            pathNode = cameFrom[pathNode];
            if (!cameFrom.ContainsKey(pathNode))
            {
                Debug.LogError("Path reconstruction failed: node not found in cameFrom.");
                break;
            }

        }


        path.Reverse();
        return path;
    }

    private int Heuristic(GridNode a, GridNode b)
    {
        float dx = Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x);
        float dz = Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z);
        return Mathf.RoundToInt(dx + dz);
    }


    private List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new List<GridNode>();
        int gridSizeX = gridManager.GridSettings.GridSizeX;
        int gridSizeY = gridManager.GridSettings.GridSizeY;
        float nodeSize = gridManager.GridSettings.NodeSize;
        int nodeX = Mathf.RoundToInt(node.WorldPosition.x / nodeSize);
        int nodeY = Mathf.RoundToInt(node.WorldPosition.z / nodeSize);
        if (nodeY + 1 < gridSizeY) neighbors.Add(gridManager.GetNode(nodeX, nodeY + 1));
        if (nodeY - 1 >= 0) neighbors.Add(gridManager.GetNode(nodeX, nodeY - 1));
        if (nodeX + 1 < gridSizeX) neighbors.Add(gridManager.GetNode(nodeX + 1, nodeY));
        if (nodeX - 1 >= 0) neighbors.Add(gridManager.GetNode(nodeX - 1, nodeY));
        return neighbors;
    }

    protected int GetDistance(GridNode nodeA, GridNode nodeB)
    {
        int dstX = (int)Mathf.Abs(nodeA.WorldPosition.x - nodeB.WorldPosition.x); //x-difference between source node + destination node
        int dstZ = (int)Mathf.Abs(nodeA.WorldPosition.z - nodeB.WorldPosition.z); //z-difference between source node + destination node

        int lowest = Mathf.Min(dstX, dstZ);
        int highest = Mathf.Max(dstX, dstZ);
        int horizontalMovesRequired = highest - lowest;

        return lowest * 14 + horizontalMovesRequired * 10; //should it be sqrt(200) instead?
    }



}
