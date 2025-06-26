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

    private Vector2Int gridOffset = Vector2Int.zero;

    void Update()
    {
        if (ghostObject == null) return;

        HandleArrowKeyMovement(); // Handle arrow input

        Vector3 worldPos = GetMouseWorldPosition();
        GridNode node = gridManager.GetNodeFromWorldPosition(worldPos);

        if (!gridManager.IsValidCoordinate((int)node.WorldPosition.x, (int)node.WorldPosition.z)) return; //checks if the source node even exists or not

        GridNode targetNode = gridManager.GetNodeFromWorldPosition(node.WorldPosition + new Vector3(gridOffset.x, 0, gridOffset.y));
        if (!gridManager.IsValidCoordinate((int)targetNode.WorldPosition.x, (int)targetNode.WorldPosition.z)) return; //checks if the target node even exists or not

        // Snap ghost to grid
        ghostObject.transform.position = targetNode.WorldPosition;

        // Optional: Validity check for placement (eg. grid occupied)
        bool validPlacement = IsValidPlacement(targetNode);
        SetGhostColor(validPlacement ? Color.green : Color.red);

        // Confirm placement
        if (Input.GetMouseButtonDown(0) && validPlacement)
        {
            PlaceBuilding(targetNode);
        }

        // Cancel placement
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private void HandleArrowKeyMovement()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) gridOffset += Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) gridOffset += Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) gridOffset += Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) gridOffset += Vector2Int.right;
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

