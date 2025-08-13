using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierPlacer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathFinder pathFinder;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private ArmyData currentArmy;

    private GameObject ghostSoldier;
    private UnitType currentUnitType;
    private Vector2Int gridOffset = Vector2Int.zero;

    //public void SetArmyData(ArmyData army)
    //{
    //    currentArmy = army;
    //}

    private void Start()
    {
        if (audioManager == null)
        {
            Debug.LogError("AudioManager is NULL! No audio will play.");
        }
        else
        {
            Debug.Log("AudioManager exists.");
        }
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

        ghostSoldier.transform.position = targetNode.WorldPosition + currentUnitType.PlacementOffset;

        // Optional: Validity check for placement (eg. grid occupied)
        bool validPlacement = IsValidPlacement(targetNode);
        SetGhostColor(validPlacement ? Color.green : Color.red);
        if (validPlacement)
        {
            Debug.Log("Placement is valid- color is green.");
        }
        else
        {
            Debug.Log("Placement is invalid- color is red.");
        }

        // Confirm placement
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            if (validPlacement)
            {
                PlaceSoldier(targetNode);
            }
            else
            {
                if (audioManager == null)
                {
                    Debug.LogError("AudioManager is NULL! No audio will play.");
                }
                else
                {
                    Debug.Log("AudioManager exists.");
                }

                audioManager.PlaySFX("Wrong Answer");
            }
        }
        // Cancel placement with right-click
        if (Input.GetMouseButtonDown(1)) CancelPlacement();

        if (audioManager != null)
        {
            audioManager.PlaySFX("Wrong Answer");
        }

    }

    public void StartPlacingSoldier(UnitType unitType)
    {
        currentUnitType = unitType;

        // Spawn ghost object
        ghostSoldier = Instantiate(unitType.unitPrefab);

        audioManager.PlaySFX("Right Answer"); // Play placement sound

        // Get UnitInstance
        UnitInstance unitInstance = ghostSoldier.GetComponent<UnitInstance>();
        if (unitInstance != null)
        {

         Debug.Log($"{unitType.name}'s currentArmy = {(currentArmy == null ? "NULL" : currentArmy.name)}");


            // Initialize the UnitInstance
            unitInstance.Initialize(
                pathFinder,
                currentArmy.TeamMaterial,
                gridManager,
                unitType,
                Vector2Int.zero, currentArmy, currentArmy.ArmyID
            );
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
        GameObject unitInstance = Instantiate(
    currentUnitType.unitPrefab,
    node.WorldPosition + currentUnitType.PlacementOffset,
    Quaternion.identity
);
        Debug.Log($"Placing {currentUnitType.name} at {node.WorldPosition} + offset {currentUnitType.PlacementOffset}");

        UnitInstance unitComponent = unitInstance.GetComponent<UnitInstance>();
        if (unitComponent == null)
        {
            Debug.LogError("Placed unit does not have a UnitInstance component.");
        }
        else if (currentArmy != null)
        {
            currentArmy.Units.Add(unitComponent); //adds unit to army
            //unitComponent.Army = currentArmy; // sets the army reference
            // Initialize the UnitInstance
            unitComponent.Initialize(
                pathFinder,
                currentArmy.TeamMaterial,
                gridManager,
                currentUnitType,
                Vector2Int.zero, currentArmy, currentArmy.ArmyID
            );
            Debug.Log($"{unitComponent.name}'s army = {(unitComponent.Army == null ? "NULL" : unitComponent.Army.name)}");
        }
        else
        {
            Debug.LogWarning("No army assigned to this unit placement.");
        }
        if (audioManager == null)
        {
            Debug.LogError("AudioManager is NULL! No audio will play.");
        }
        else
        {
            Debug.Log("AudioManager exists.");
        }

        audioManager.PlaySFX("Bike Bell"); // Play placement sound
        Destroy(ghostSoldier);
        currentArmy.hasAddedUnits = true;
    }


    private void CancelPlacement() => Destroy(ghostSoldier);

    private bool IsValidPlacement(GridNode node)
    {
        return node.Walkable; // You can expand this logic
    }

    private void SetGhostColor(Color color) //change that to material
    {
        var renderers = ghostSoldier.GetComponentsInChildren<MeshRenderer>();
        foreach(Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) return hit.point;
        return Vector3.zero;
    }
}

