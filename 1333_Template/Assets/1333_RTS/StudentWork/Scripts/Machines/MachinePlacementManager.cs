using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MachinePlacementManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private LayerMask placementMask;

    private MachineType pendingMachine;
    private GameObject ghostObject;
    private MeshRenderer[] ghostRenderers;
    private ArmyData playerArmy;

    private void Update()
    {
        if (pendingMachine == null || ghostObject == null) return;

        // Raycast from mouse to grid
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out var hit, 100f, placementMask)) return;

        Vector2Int snappedCoords = gridManager.GetNodeFromWorldPosition(hit.point).Coordinates;
        Vector3 snappedWorld = gridManager.GetNode(snappedCoords.x, snappedCoords.y).WorldPosition;

        ghostObject.transform.position = snappedWorld;

        bool canPlace = gridManager.CanPlaceMachine(pendingMachine, snappedCoords);
        SetGhostMaterial(canPlace ? validMaterial : invalidMaterial);

        if (canPlace && Mouse.current.leftButton.wasPressedThisFrame)
        {
            playerArmy.SpawnMachine(pendingMachine, snappedWorld, playerArmy.TeamMaterial);
            CancelPlacement();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
        }
    }

    public void BeginPlacement(MachineType machineType, ArmyData army)
    {
        pendingMachine = machineType;
        playerArmy = army;

        if (ghostObject != null)
            Destroy(ghostObject);

        ghostObject = Instantiate(machineType.machinePrefab);
        ghostObject.SetActive(true);
        ghostRenderers = ghostObject.GetComponentsInChildren<MeshRenderer>();
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
        pendingMachine = null;
        if (ghostObject != null)
            Destroy(ghostObject);
    }
}

