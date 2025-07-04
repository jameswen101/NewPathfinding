using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierPlacer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathFinder pathFinder;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject healthBarPrefab;

    private GameObject ghostSoldier;
    private UnitType currentUnitType;
    private Vector2Int gridOffset = Vector2Int.zero;
    private ArmyData currentArmy;

    public void SetArmyData(ArmyData army)
    {
        currentArmy = army;
    }

    void Update()
    {
        if (ghostSoldier == null) return;

        HandleArrowKeys();

        Vector3 worldPos = GetMouseWorldPosition();
        GridNode node = gridManager.GetNodeFromWorldPosition(worldPos);
        if (!gridManager.IsValidCoordinate((int)node.WorldPosition.x, (int)node.WorldPosition.z)) return;

        GridNode targetNode = gridManager.GetNodeFromWorldPosition(node.WorldPosition + new Vector3(gridOffset.x, 0, gridOffset.y));
        if (!gridManager.IsValidCoordinate((int)targetNode.WorldPosition.x, (int)targetNode.WorldPosition.z)) return;

        ghostSoldier.transform.position = targetNode.WorldPosition;

        // Optional: Validity check for placement (eg. grid occupied)
        bool validPlacement = IsValidPlacement(targetNode);
        SetGhostColor(validPlacement ? Color.green : Color.red);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            PlaceSoldier(targetNode);
        }
        if (Input.GetMouseButtonDown(1)) CancelPlacement();
    }

    public void StartPlacingSoldier(UnitType unitType)
    {
        currentUnitType = unitType;

        // Spawn ghost object
        ghostSoldier = Instantiate(unitType.unitPrefab);

        // Get UnitInstance
        UnitInstance unitInstance = ghostSoldier.GetComponent<UnitInstance>();
        if (unitInstance != null)
        {
            // Initialize the UnitInstance
            unitInstance.Initialize(
                pathFinder,
                currentArmy.TeamMaterial,
                gridManager,
                unitType,
                Vector2Int.zero, currentArmy, currentArmy.ArmyID
            );

            // Create health bar
            GameObject hbObj = Instantiate(healthBarPrefab);

            // Get HealthBar component
            HealthBar hb = hbObj.GetComponent<HealthBar>();
            if (hb != null)
            {
                hb.Initialize(
                    ghostSoldier.transform,     // Target transform to follow
                    unitInstance,               // IHasHealth reference (make sure UnitInstance implements IHasHealth)
                    mainCamera                    // Camera to convert world to screen
                );
            }
            else
            {
                Debug.LogError("HealthBar prefab is missing HealthBar script!");
            }
        }
        else
        {
            Debug.LogError("The prefab does not have a UnitInstance component!");
        }

    }



    private void HandleArrowKeys()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) gridOffset += Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) gridOffset += Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) gridOffset += Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) gridOffset += Vector2Int.right;
    }

    private void PlaceSoldier(GridNode node)
    {
        GameObject unitInstance = Instantiate(currentUnitType.unitPrefab, node.WorldPosition, Quaternion.identity);

        UnitInstance unitComponent = unitInstance.GetComponent<UnitInstance>();
        if (unitComponent == null)
        {
            Debug.LogError("Placed unit does not have a UnitInstance component.");
        }
        else if (currentArmy != null)
        {
            currentArmy.Units.Add(unitComponent);
        }
        else
        {
            Debug.LogWarning("No army assigned to this unit placement.");
        }

        Destroy(ghostSoldier);
    }


    private void CancelPlacement() => Destroy(ghostSoldier);

    private bool IsValidPlacement(GridNode node)
    {
        return node.Walkable; // You can expand this logic
    }

    private void SetGhostColor(Color color)
    {
        var renderer = ghostSoldier.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) return hit.point;
        return Vector3.zero;
    }
}

