using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Camera mainCamera;

    private BuildingData selectedBuilding;
    private GameObject previewObject;
    private Vector3 previewPosition;

    private BuildingData currentBuildingData;
    private GameObject ghostObject;

    public void StartPlacing(BuildingData buildingData)
    {
        currentBuildingData = buildingData;

        // Instantiate ghost prefab (make sure your BuildingData has this reference)
        ghostObject = Instantiate(buildingData.buildingPrefab); //add a separate GhostPrefab in BuildingData?
    }

    void Update()
    {
        if (ghostObject == null) return;

        Vector3 worldPos = GetMouseWorldPosition();
        GridNode node = gridManager.GetNodeFromWorldPosition(worldPos);

        // Snap ghost to grid
        ghostObject.transform.position = node.WorldPosition;

        // Optional: Validity check for placement (eg. grid occupied)
        bool validPlacement = IsValidPlacement(node);
        SetGhostColor(validPlacement ? Color.green : Color.red);

        // Confirm placement
        if (Input.GetMouseButtonDown(0) && validPlacement)
        {
            PlaceBuilding(node);
        }

        // Cancel placement
        if (Input.GetMouseButtonDown(1))  // right-click to cancel
        {
            CancelPlacement();
        }
    }

    private void PlaceBuilding(GridNode node)
    {
        Instantiate(currentBuildingData.buildingPrefab, node.WorldPosition, Quaternion.identity);
        Destroy(ghostObject);
        currentBuildingData = null;
    }

    private void CancelPlacement()
    {
        Destroy(ghostObject);
        currentBuildingData = null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private bool IsValidPlacement(GridNode node)
    {
        return node.Walkable; // You can expand this logic
    }

    private void SetGhostColor(Color color)
    {
        var renderer = ghostObject.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}

