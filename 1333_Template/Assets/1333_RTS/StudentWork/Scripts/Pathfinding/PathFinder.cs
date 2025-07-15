using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    public enum PathfindingType
    {
        Unweighted,
        Weighted,
        BruteForce,
        Naive,
    }

    public GridNode StartNode;
    public GridNode EndNode;

    List<Vector2Int> openSet = new List<Vector2Int>();
    HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
    Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
    Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float>();
    Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();


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

    public List<Vector3> CalculatePath(GridNode StartNode, GridNode EndNode)
    {
        List<Vector3> searchedNodes = new List<Vector3>(); //starts with nothing
        List<Vector3> nodesToSearch = new List<Vector3> { StartNode.WorldPosition }; //starts with all
        List<Vector3> finalPath = new List<Vector3>(); //start with nothing

        /*
        StartNode.gCost = 0;
        StartNode.hCost = GetDistance(StartNode, EndNode);
        StartNode.fCost = GetDistance(StartNode, EndNode);
        */

        Vector3 vector3 = Vector3.zero;
        Vector3 straightBack = Vector3.back; //moves back by 1
        Vector3 straightForward = Vector3.forward; //moves forward by 1
        Vector3 straightLeft = Vector3.left; //moves left by 1
        Vector3 straightRight = Vector3.right; //moves right by 1

        Vector3 startPosition = StartNode.WorldPosition;
        Vector3 endPosition = EndNode.WorldPosition;
        Vector3 currentPosition = startPosition;
        Vector3 posDiff = endPosition - currentPosition;

        while (currentPosition != endPosition) 
        {
            if (Mathf.Abs(posDiff.z) > Mathf.Abs(posDiff.x)) //make it absolute
            {
                if (posDiff.z < 0)
                {
                    currentPosition += straightBack;
                }
                else if (posDiff.z > 0)
                {
                    currentPosition += straightForward;
                }
            }
            else
            {
                if (posDiff.x < 0)
                {
                    currentPosition += straightLeft;
                }
                else if (posDiff.x > 0)
                {
                    currentPosition += straightRight;
                }
            }
            finalPath.Add(currentPosition);
            posDiff = endPosition- currentPosition;

        }
        //return the path/list the positions in the path
        return finalPath;
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
