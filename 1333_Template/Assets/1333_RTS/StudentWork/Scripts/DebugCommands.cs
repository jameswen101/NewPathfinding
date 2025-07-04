using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DebugCommands : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    //[SerializeField] private ArmyManager armyManager;
    [SerializeField] private ArmyPathfindingTester armyPathfindingTester;
    [SerializeField] private AllArmiesManager allArmiesManager;
    [SerializeField] private BuildingPlacementManager buildingPlacementManager;
    [SerializeField] private BuildingTypes buildingTypes;
    [SerializeField] private ArmyData armyData;
    [SerializeField] private AvailableTeamUnits armyUnits;
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private MachineCollection machineTypes;
    [SerializeField] private TeamMaterialsCollection teamMaterials;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathFinder pathFinder;
    [SerializeField] private Camera mainCamera;
    private void OnEnable()
    {
        DebugLogConsole.AddCommand("HelloWorld", "Prints a message to the console", HelloWorld);

        DebugLogConsole.AddCommand<string, int, string, int>(
    "ArmySpawn",
    "Spawns an army. Usage: ArmySpawn [unitTypeName] [armyId] [x,z] [colorIndex]", //remove colorIndex, ID?
    (unitTypeName, armyId, position, colorIndex) =>
    {
        string[] coords = position.Split(',');
        if (coords.Length != 2 ||
            !float.TryParse(coords[0], out float x) ||
            !float.TryParse(coords[1], out float z))
        {
            Debug.LogError("Invalid position format. Use x,z (e.g. 5.5,2.3)");
            return;
        }

        ArmySpawn(unitTypeName);
    }
);


        DebugLogConsole.AddCommand<string, int>(
            "PlaceBuilding",
            "Places a building on the grid. Usage: PlaceBuilding [buildingTypeName] [armyId]",
            PlaceBuilding
        ); //remove armyID- all buildings spawned will have armyID 0

        DebugLogConsole.AddCommand<int, int>("SpawnMachine", "Spawns a machine for your army to attack. " +
            "Usage: SpawnMachine [int, int]", SpawnMachine);
    }

    private void HelloWorld()
    {
        Debug.Log("Hello world");
    }

    public void ArmySpawn(string unitTypeName)
    {
        // Find the UnitType by name
        UnitType unitType = armyUnits.AvailableUnits.Find(u => u.unitTypeName == unitTypeName);

        if (unitType == null)
        {
            Debug.LogError($"UnitType '{unitTypeName}' not found!");
            return;
        }
        // Instantiate the prefab
        GameObject unitObj = Instantiate(unitType.unitPrefab);

        // Get the UnitInstance component
        UnitInstance unit = unitObj.GetComponent<UnitInstance>();
        if (unit != null)
        {
            // Force ArmyID = 0
            unit.ArmyID = 0;
            // Optionally assign Army reference (if you use it)
            unit.Army = armyData;

            // Initialize with player material
            unit.Initialize(
                pathFinder,
                armyData.TeamMaterial,
                gridManager,
                unitType,
                Vector2Int.zero, armyData, unit.ArmyID // adjust grid origin if needed
            );

            Debug.Log($"Spawned {unitType.unitTypeName} for player army.");
        }
        else
        {
            Debug.LogError("Unit prefab is missing UnitInstance component.");
        }

        // Spawn health bar
        HealthBar hb = unitObj.GetComponentInChildren<HealthBar>();
        if (hb != null)
        {
            hb.Initialize(
                unitObj.transform,
                unit,
                mainCamera
            );
        }

    }


    private void PlaceBuilding(string buildingName, int armyId)
    {
        if (buildingTypes == null)
        {
            Debug.LogError("BuildingTypes asset not found.");
            return;
        }

        var buildingData = buildingTypes.Buildings
            .FirstOrDefault(b => b.buildingName.Equals(buildingName, System.StringComparison.OrdinalIgnoreCase));

        if (buildingData == null)
        {
            Debug.LogError($"No building found with name: {buildingName}");
            return;
        }

        Debug.Log($"Forcing ArmyID to 0 instead of {armyId}");
        if (!allArmiesManager.TryGetArmy(0, out ArmyData army))
        {
            Debug.LogError("No army registered with ID 0");
            return;
        }

        Debug.Log($"allArmiesManager = {allArmiesManager}");
        Debug.Log($"army = {army}");
        Debug.Log($"armyId (forced) = 0");



        buildingPlacer.SetArmyData(army);
        buildingPlacer.StartPlacing(buildingData);
    }


    public void SpawnMachine(int armyID, int machineIndex)
    {
        if (machineTypes == null)
        {
            Debug.LogError("MachineTypes is not assigned.");
            return;
        }

        if (machineIndex < 0 || machineIndex >= machineTypes.MachineList.Count)
        {
            Debug.LogError($"Machine index {machineIndex} out of range.");
            return;
        }

        if (!AllArmiesManager.Instance.TryGetArmy(armyID, out var armyData))
        {
            Debug.LogError($"Army with ID {armyID} not found!");
            return;
        }

        MachineType machineData = machineTypes.MachineList[machineIndex];
        Vector3 spawnPos = FindRandomValidPosition(machineData.width, machineData.height);

        armyData.SpawnMachine(machineData, spawnPos, armyData.TeamMaterial);
    }


    private Vector3 FindRandomValidPosition(int width, int height)
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            int x = Random.Range(0, armyData.GridManager.GridSettings.GridSizeX - width + 1);
            int y = Random.Range(0, armyData.GridManager.GridSettings.GridSizeY - height + 1);

            if (armyData.GridManager.IsRegionWalkable(x, y, width, height))
            {
                return armyData.GridManager.GetNode(x, y).WorldPosition;
            }
        }
        Debug.LogWarning("No valid spawn location found.");
        return Vector3.zero;
    }


}
