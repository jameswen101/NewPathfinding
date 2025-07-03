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
    private void OnEnable()
    {
        DebugLogConsole.AddCommand("HelloWorld", "Prints a message to the console", HelloWorld);

        DebugLogConsole.AddCommand<string, int, string, int>(
    "ArmySpawn",
    "Spawns an army. Usage: ArmySpawn [unitTypeName] [armyId] [x,z] [colorIndex]",
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

        ArmySpawn(unitTypeName, armyId, x, z, colorIndex);
    }
);


        DebugLogConsole.AddCommand<string, int>(
            "PlaceBuilding",
            "Places a building on the grid. Usage: PlaceBuilding [buildingTypeName] [armyId]",
            PlaceBuilding
        );

        DebugLogConsole.AddCommand<int, int>("SpawnMachine", "Spawns a machine for your army to attack. " +
            "Usage: SpawnMachine [int, int]", SpawnMachine);
    }

    private void HelloWorld()
    {
        Debug.Log("Hello world");
    }

    private void ArmySpawn(string unitTypeName, int armyId, float x, float z, int colorIndex) //remove ColorIndex parameter
    {
        UnitType unitType = null;
        for (int i = 0; i < armyUnits.AvailableUnits.Count; i++)
        {
            if (string.Equals(armyUnits.AvailableUnits[i].unitTypeName, unitTypeName, StringComparison.OrdinalIgnoreCase))
            {
                unitType = armyUnits.AvailableUnits[i];
                break;
            }
        }

        if (unitType == null)
        {
            Debug.LogError($"unitTypeName '{unitTypeName}' not found in AvailableUnits!");
            return;
        }


        if (!allArmiesManager.TryGetArmy(armyId, out armyData)) //change to ArmyData?
        {
            Debug.LogError($"No army registered with ID {armyId}");
            return;
        }
        Debug.Log($"Army found: {armyData}");

        if (armyPathfindingTester == null)
        {
            Debug.LogError("ArmyPathfindingTester is not assigned!");
            return;
        }

        var data = ScriptableObject.CreateInstance<UnitData>();
            data.UnitType = unitType;
            data.Position = new Vector3(x, 0, z);
            data.Health = unitType.MaxHp;
            data.ArmyId = armyId;
            // Validate color index
            if (colorIndex < 0 || colorIndex >= armyPathfindingTester.armyMaterials.Count) //change to TeamMaterialsCollection
            {
                Debug.LogError($"Invalid color index {colorIndex}. Must be between 0 and {armyPathfindingTester.armyMaterials.Count - 1}.");
                return;
            }
            data.TeamMaterial = armyPathfindingTester.armyMaterials[colorIndex];

            armyData.SpawnUnit(unitType, data.Position, data.TeamMaterial);
            Debug.Log($"Spawned {unitTypeName} at ({x}, {z}) in Army {armyId} with color index {colorIndex}");
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

        Debug.Log($"Trying to get army with ID {armyId}");
        if (!allArmiesManager.TryGetArmy(armyId, out ArmyData army))
        {
            Debug.LogError($"No army registered with ID {armyId}");
            return;
        }

        Debug.Log($"allArmiesManager = {allArmiesManager}");
        Debug.Log($"army = {army}");
        Debug.Log($"armyId = {armyId}");

        buildingPlacer.SetArmyData(army);        //if this is null, check if this class has ArmyData defined properly      
        buildingPlacer.StartPlacing(buildingData);     // Already spawns ghost + arrow movement
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
