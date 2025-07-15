using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;

public class MachinePlacer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ArmyData playerArmy;
    [SerializeField] private LayerMask placementMask;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    private MachineType pendingMachine;
    private GameObject ghostObject;
    private MeshRenderer[] ghostRenderers;
    private Vector2Int gridOffset = Vector2Int.zero;

    void Update()
    {
        if (pendingMachine == null || ghostObject == null)
            return;

        HandleArrowKeyMovement(); // Implement same as BuildingPlacer if needed

        Vector3 worldPos = GetMouseWorldPosition();
        GridNode node = gridManager.GetNodeFromWorldPosition(worldPos);
        if (!gridManager.IsValidCoordinate((int)node.WorldPosition.x, (int)node.WorldPosition.z))
            return;

        // Allow grid offset movement
        GridNode targetNode = gridManager.GetNodeFromWorldPosition(
            node.WorldPosition + new Vector3(gridOffset.x, 0, gridOffset.y)
        );

        if (!gridManager.IsValidCoordinate((int)targetNode.WorldPosition.x, (int)targetNode.WorldPosition.z))
            return;

        // Snap ghost
        ghostObject.transform.position = targetNode.WorldPosition;

        // Check if area is walkable and unoccupied
        bool canPlace = gridManager.IsRegionWalkable(
            (int)targetNode.WorldPosition.x,
            (int)targetNode.WorldPosition.z,
            pendingMachine.width,
            pendingMachine.height
        );

        // Visual feedback
        SetGhostMaterial(canPlace ? validMaterial : invalidMaterial);

        // Confirm placement
        if (canPlace && (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
        {
            playerArmy.SpawnMachine(pendingMachine,targetNode.WorldPosition,playerArmy.TeamMaterial
            );
            CancelPlacement();
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

    public void StartPlacingMachine(MachineType machineType, ArmyData army)
    {
        pendingMachine = machineType;
        playerArmy = army;

        if (ghostObject != null)
            Destroy(ghostObject);

        ghostObject = Instantiate(machineType.machinePrefab); // assumes you have a prefab reference in MachineType
        ghostObject.SetActive(true);

        ghostRenderers = ghostObject.GetComponentsInChildren<MeshRenderer>();
    }

    private void SetGhostMaterial(Material mat)
    {
        foreach (var renderer in ghostRenderers)
        {
            var materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
                materials[i] = mat;
            renderer.materials = materials;
        }
    }

    private void CancelPlacement()
    {
        pendingMachine = null;
        if (ghostObject != null)
            Destroy(ghostObject);
    }

    /// <summary>
    /// Immediate random placement (non-interactive)
    /// </summary>
    public void PlaceMachine(MachineType machineType)
    {
        Vector3 randomPos = GetRandomValidPosition(machineType.width, machineType.height);
        if (randomPos != Vector3.zero)
        {
            playerArmy.SpawnMachine(machineType, randomPos, playerArmy.TeamMaterial);
        }
        else
        {
            Debug.LogWarning("No valid position found to spawn machine.");
        }
    }

    private Vector3 GetRandomValidPosition(int width, int height)
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            int x = Random.Range(0, gridManager.GridSettings.GridSizeX - width + 1);
            int y = Random.Range(0, gridManager.GridSettings.GridSizeY - height + 1);

            if (gridManager.IsRegionWalkable(x, y, width, height))
                return gridManager.GetNode(x, y).WorldPosition;
        }
        return Vector3.zero;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementMask))
            return hit.point;

        return Vector3.zero;
    }


}
