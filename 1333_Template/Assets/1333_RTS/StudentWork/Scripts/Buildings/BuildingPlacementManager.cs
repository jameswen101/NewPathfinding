using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class BuildingPlacementManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private LayerMask placementMask;
    [SerializeField] private BuildingTypes buildingTypes;

    public static BuildingPlacementManager instance;

    public BuildingData pendingBuilding;
    //private BuildingData matchingBuilding;
    private GameObject ghostObject;
    private MeshRenderer[] ghostRenderers;
    private ArmyData playerArmy;

    private void Start()
    {
        

        //matchingBuilding = buildingTypes.Buildings.FirstOrDefault(b => b.buildingName == pendingBuilding.buildingName); //returns null if no matching buildings
    }

    public void BeginPlacement(BuildingData buildingData, ArmyData playerArmy) //why armydata?
    {
        //matchingBuilding = buildingData;
        this.playerArmy = playerArmy;

        if (ghostObject != null) Destroy(ghostObject);
        ghostObject = Instantiate(buildingData.buildingPrefab);
        ghostObject.SetActive(true);

        ghostRenderers = ghostObject.GetComponentsInChildren<MeshRenderer>();
    }

    private void Update()
    {
        if (pendingBuilding == null || ghostObject == null) return;

        // Raycast from mouse to grid
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out var hit, 100f, placementMask)) return;

        Vector2Int snappedCoords = gridManager.GetNodeFromWorldPosition(hit.point).Coordinates;
        Vector3 snappedWorld = gridManager.GetNode(snappedCoords.x, snappedCoords.y).WorldPosition;

        ghostObject.transform.position = snappedWorld;

        bool canPlace = gridManager.CanPlaceBuilding(buildingTypes, snappedCoords);
        SetGhostMaterial(canPlace ? validMaterial : invalidMaterial);

        if (canPlace && Mouse.current.leftButton.wasPressedThisFrame)
        {
            playerArmy.SpawnBuilding(buildingTypes, snappedWorld); //if pendingBuilding were a BuildingTypes -> pendingBuilding[?]
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
        pendingBuilding = null;
        if (ghostObject != null)
            Destroy(ghostObject);
    }
}
