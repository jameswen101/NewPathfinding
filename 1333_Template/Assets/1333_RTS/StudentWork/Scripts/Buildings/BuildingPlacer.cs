using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ArmyData armyData;
    [SerializeField] private GameObject healthBarPrefab;


    private BuildingData selectedBuilding;
    private GameObject previewObject;
    private Vector3 previewPosition;

    private BuildingData currentBuildingData;
    private GameObject ghostObject;
    private ArmyData currentArmy;

    public void SetArmyData(ArmyData army)
    {
        currentArmy = army;
    }


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
        if (validPlacement && (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
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
        // 1. Instantiate the actual building
        GameObject buildingInstance = Instantiate(
            currentBuildingData.buildingPrefab,
            node.WorldPosition,
            currentBuildingData.buildingPrefab.transform.rotation
        );

        // 2. Try to get the BuildingInstance (or BuildingBase)
        BuildingBase buildingComponent = buildingInstance.GetComponent<BuildingBase>();
        BuildingInstance buildingIns = buildingInstance.GetComponent<BuildingInstance>();

        IHasHealth healthComponent = buildingIns.GetComponent<IHasHealth>();

        if (buildingComponent == null)
        {
            Debug.LogError("Placed building does not have a BuildingBase component.");
        }
        else if (currentArmy != null)
        {
            currentArmy.Buildings.Add(buildingComponent);
        }
        else
        {
            Debug.LogWarning("No army assigned to this building placement.");
        }

        // 3. Spawn Health Bar if applicable
        if (buildingIns != null)
        {
            if (buildingIns.healthBar != null)
            {
                buildingIns.healthBar.Initialize(
                    buildingInstance.transform,
                    buildingIns, // assuming it implements IHasHealth
                    mainCamera
                );
            }
            else
            {
                Debug.LogWarning("Building prefab is missing a HealthBar reference!");
            }
        }


        // 4. Clean up
        Destroy(ghostObject);
        ghostObject = null;
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

