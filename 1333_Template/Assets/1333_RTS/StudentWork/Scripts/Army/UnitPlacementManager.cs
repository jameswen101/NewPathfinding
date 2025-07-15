using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class UnitPlacementManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private LayerMask placementMask;
    [SerializeField] private AvailableTeamUnits availableTeamUnits;

    public static UnitPlacementManager instance;

    private UnitType pendingUnit;
    //private BuildingData matchingBuilding;
    private GameObject ghostObject;
    private Renderer[] ghostRenderers;
    private ArmyData playerArmy;

    private void Start()
    {
        

        //matchingBuilding = buildingTypes.Buildings.FirstOrDefault(b => b.buildingName == pendingBuilding.buildingName); //returns null if no matching buildings
    }

    public void BeginPlacement(UnitType unitType, ArmyData playerArmy)
    {
        this.pendingUnit = unitType;
        this.playerArmy = playerArmy;

        if (ghostObject != null) Destroy(ghostObject);
        ghostObject = Instantiate(pendingUnit.unitPrefab);
        ghostObject.SetActive(true);

        ghostRenderers = ghostObject.GetComponentsInChildren<Renderer>();
    }


    private void Update()
    {
        if (pendingUnit == null || ghostObject == null) return;

        // Raycast from mouse to grid
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out var hit, 100f, placementMask)) return;

        Vector2Int snappedCoords = gridManager.GetNodeFromWorldPosition(hit.point).Coordinates;
        Vector3 snappedWorld = gridManager.GetNode(snappedCoords.x, snappedCoords.y).WorldPosition;

        ghostObject.transform.position = snappedWorld;

        // Use pendingUnit here
        bool canPlace = gridManager.CanPlaceUnit(pendingUnit, snappedCoords);
        SetGhostMaterial(canPlace ? validMaterial : invalidMaterial);

        if (canPlace && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // This should be something like SpawnUnit, not SpawnBuilding:
            // (assuming you create a SpawnUnit method)
            playerArmy.SpawnUnit(pendingUnit, snappedWorld, playerArmy.TeamMaterial); //try getting that unit's existing army material, or whatever army material was pre-assigned
            CancelPlacement();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
        }
    }


    private void SetGhostMaterial(Material mat)
    {
        foreach (var r in ghostRenderers)
        {
            var materials = r.materials;
            for (int i = 0; i < materials.Length; i++)
                materials[i] = mat;
            r.materials = materials;
        }
    }

    private void CancelPlacement()
    {
        pendingUnit = null;
        if (ghostObject != null)
            Destroy(ghostObject);
    }
}
