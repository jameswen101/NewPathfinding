using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using UnityEngine;

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
    }

    private void HelloWorld()
    {
        Debug.Log("Hello world");
    }

    private void ArmySpawn(string unitTypeName, int armyId, float x, float z, int colorIndex) //armyUnits.AvailableUnits[i].unitTypeName
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
            if (colorIndex < 0 || colorIndex >= armyPathfindingTester.armyMaterials.Count)
            {
                Debug.LogError($"Invalid color index {colorIndex}. Must be between 0 and {armyPathfindingTester.armyMaterials.Count - 1}.");
                return;
            }
            data.TeamMaterial = armyPathfindingTester.armyMaterials[colorIndex];

            armyData.SpawnUnit(data);
            Debug.Log($"Spawned {unitTypeName} at ({x}, {z}) in Army {armyId} with color index {colorIndex}");
    }


    private void PlaceBuilding(string buildingName, int armyId)
    {
        // Always load the full BuildingTypes asset
        //var buildingTypes = Resources.Load<BuildingTypes>("BuildingTypes/DefaultBuildingTypes"); // adjust filename as needed
        if (buildingTypes == null)
        {
            Debug.LogError("BuildingTypes asset not found in Resources/BuildingTypes.");
            return;
        }

        var buildingData = buildingTypes.Buildings
    .FirstOrDefault(b => b.buildingName.Equals(buildingName, System.StringComparison.OrdinalIgnoreCase));

        if (buildingData == null)
        {
            Debug.LogError($"No building found with name: {buildingName}");
            return;
        }


        // Get the army
        
        //if (!AllArmiesManager.Instance.TryGetArmy(armyId, out var army))
        //if (armyManager.ArmyID == -1)
        //{
        //    Debug.LogError($"No army registered with ID {armyId}");
        //    return;
        //}

        buildingPlacementManager.BeginPlacement(buildingData, armyData); //how are you gonna convert from armyID to armyData?

    }


}
