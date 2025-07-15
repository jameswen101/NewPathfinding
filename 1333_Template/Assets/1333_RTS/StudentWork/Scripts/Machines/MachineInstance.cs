using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineInstance : MonoBehaviour
{
    public MachineType MachineType { get; private set; }
    public Vector2Int GridPosition { get; private set; }

    public bool IsDestroyed { get; private set; }

    private ArmyData owningArmy;

    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    public void Initialize(PathFinder pathfinder, Material teammaterial, GridManager gridManager, MachineType machineType, Vector2Int gridPos) //add TeamMaterial as parameter if needed
    {
        MachineType = machineType;
        GridPosition = gridPos;
        // You can add pathfinder setup, assign materials, etc.
    }

    public void AssignToArmy(ArmyData army)
    {
        owningArmy = army;
        GetComponentInChildren<MeshRenderer>().material = army.TeamMaterial;
        // Optional: set team materials, flags, etc.
    }

    public void DestroyMachine()
    {
        IsDestroyed = true;
        Destroy(gameObject);
    }
}
