using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    public bool TryPlaceBuilding(BuildingInfo buildingInfo, Vector3 worldPosition)
    {
        List<GridNode> occupiedNodes = GetOccupiedNodes(buildingInfo, worldPosition);

        // Optional: check if any node is not walkable first
        foreach (var node in occupiedNodes)
        {
            if (!node.Walkable)
            {
                Debug.Log("Placement failed: occupied node not walkable");
                return false;
            }
        }

        for (int i = 0; i < occupiedNodes.Count; i++)
        {
            var node = occupiedNodes[i];
            node.Walkable = false;
            occupiedNodes[i] = node;  // write back to list
        }

        // Instantiate prefab (optional)
        Instantiate(buildingInfo.buildingPrefab, worldPosition, Quaternion.identity);

        return true;
    }

    private List<GridNode> GetOccupiedNodes(BuildingInfo buildingInfo, Vector3 worldPos)
    {
        List<GridNode> occupied = new List<GridNode>();
        int startX = Mathf.RoundToInt(worldPos.x / gridManager.GridSettings.NodeSize);
        int startY = Mathf.RoundToInt(worldPos.z / gridManager.GridSettings.NodeSize);

        for (int dx = 0; dx < buildingInfo.Width; dx++)
        {
            for (int dy = 0; dy < buildingInfo.Height; dy++)
            {
                int nodeX = startX + dx;
                int nodeY = startY + dy;
                if (nodeX >= 0 && nodeX < gridManager.GridSettings.GridSizeX &&
                    nodeY >= 0 && nodeY < gridManager.GridSettings.GridSizeY)
                {
                    occupied.Add(gridManager.GetNode(nodeX, nodeY));
                }
            }
        }
        return occupied;
    }
}

