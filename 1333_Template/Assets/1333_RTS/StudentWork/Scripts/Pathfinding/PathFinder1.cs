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

    public GridNode1 StartNode; //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    public GridNode1 EndNode; //currently MonoBehavior class gridnode1; change back to struct gridnode if not working

    public List<GridNode1> openSet = new List<GridNode1>(); //should we also have a closedSet? 
    //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    public List<GridNode1> closedSet = new List<GridNode1>(); 
    //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    public Dictionary<GridNode1, int> costSoFar = new Dictionary<GridNode1, int>(); //gCost (starts at 0) 
    //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    public Dictionary<GridNode1, int> estimatedTotalCost = new Dictionary<GridNode1, int>(); //fCost; costSoFar + Heuristic = estimatedTotalCost?
    //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    public Dictionary<GridNode1, GridNode1> cameFrom = new Dictionary<GridNode1, GridNode1>(); //path reconstruction
    //currently MonoBehavior class gridnode1; change back to struct gridnode if not working


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
    private void Start()
    {
        
    }

    public List<GridNode1> CalculatePath(GridNode1 StartNode, GridNode1 EndNode) //once we changed to this version, the line disappeared
                                                                              //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    {
        openSet.Add(StartNode);
        costSoFar[StartNode] = 0; //adds StartNode to dictionary
        estimatedTotalCost[StartNode] = Heuristic(StartNode, EndNode); //adds StartNode to dictionary
        //adds StartNode to dictionary
        cameFrom[StartNode] = StartNode; // This works for marking the root

        Debug.Log($"Finding path from ({StartNode.WorldPosition.x}, {StartNode.WorldPosition.z}) to ({EndNode.WorldPosition.x}, {EndNode.WorldPosition.z})");

        while (openSet.Count > 0)
        {
            //find code with lowest f-score
            GridNode1 current = openSet[0];
            foreach (GridNode1 node in openSet) //does openSet only have 1 node?
                                                //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
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
            List<GridNode1> path = new List<GridNode1>();
        GridNode1 pathNode = EndNode; //currently MonoBehavior class gridnode1; change back to struct gridnode if not working

        if (!cameFrom.ContainsKey(EndNode))
            return path;
        while (!pathNode.Equals(StartNode)) //not the 1st node
        {
            path.Add(pathNode);
            pathNode = cameFrom[pathNode]; //key not found
        }
        path.Add(StartNode);
        path.Reverse();
        return path;
    }

    private int Heuristic(GridNode1 a, GridNode1 b) //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    {
        float dx = Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x);
        float dz = Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z);
        return Mathf.RoundToInt(dx + dz);
    }


    private List<GridNode1> GetNeighbors(GridNode1 node) //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    {
        List<GridNode1> neighbors = new List<GridNode1>();
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

    protected int GetDistance(GridNode1 nodeA, GridNode1 nodeB) //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    {
        int dstX = (int)Mathf.Abs(nodeA.WorldPosition.x - nodeB.WorldPosition.x); //x-difference between source node + destination node
        int dstZ = (int)Mathf.Abs(nodeA.WorldPosition.z - nodeB.WorldPosition.z); //z-difference between source node + destination node

        int lowest = Mathf.Min(dstX, dstZ);
        int highest = Mathf.Max(dstX, dstZ);
        int horizontalMovesRequired = highest - lowest;

        return lowest * 14 + horizontalMovesRequired * 10; //should it be sqrt(200) instead?
    }



}
