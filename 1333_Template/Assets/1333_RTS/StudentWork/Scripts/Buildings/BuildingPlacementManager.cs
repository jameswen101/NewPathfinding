using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacementManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private LayerMask placementMask;

    private static BuildingPlacementManager _instance;

    private BuildingType _pendingBuilding;
    private GameObject ghostObject;
    private MeshRenderer[] ghostRenderers;
    private ArmyData playerArmy;

    public void BeginPlacement(BuildingType buildingType, ArmyData playerArmy)
    {
        _pendingBuilding = buildingType;
        _playerArmy = playerArmy;

        if (ghostObject != null) Destroy(ghostObject);
        ghostObject = Instantiate(buildingType.Prefab.gameObject);
        ghostObject.SetActive(true);

        ghostRenderers = ghostObject.GetComponentsInChildren<MeshRenderer>();
    }

    private void Update()
    {
        if (_pendingBuilding == null || ghostObject == null) return;

        // Raycast from mouse to grid
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out var hit, 100f, placementMask)) return;

        Vector2Int snappedCoords = gridManager.GetNodeFromWorldPosition(hit.point).Coordinates;
        Vector3 snappedWorld = gridManager.GetNode(snappedCoords.x, snappedCoords.y).WorldPosition;

        ghostObject.transform.position = snappedWorld;

        bool canPlace = gridManager.CanPlaceBuilding(_pendingBuilding, snappedCoords);
        SetGhostMaterial(canPlace ? validMaterial : invalidMaterial);

        if (canPlace && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _playerArmy.SpawnBuilding(_pendingBuilding, snappedWorld);
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
        _pendingBuilding = null;
        if (ghostObject != null)
            Destroy(ghostObject);
    }
}
