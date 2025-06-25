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

    private void OnEnable()
    {
        DebugLogConsole.AddCommand("HelloWorld", "Prints a message to the console", HelloWorld);

        DebugLogConsole.AddCommand<string, int, float, float>(
            "ArmySpawn",
            "Spawns an army. Usage: ArmySpawn [unitTypeName] [armyId] [x] [z]",
            ArmySpawn
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

    private void ArmySpawn(string unitTypeName, int armyId, float x, float z)
    {
        var unitType = Resources.Load<UnitType>($"UnitTypes/{unitTypeName}");
        if (unitType == null)
        {
            Debug.LogError($"Invalid unit type: {unitTypeName}");
            return;
        }

        if (!allArmiesManager.TryGetArmy(armyId, out var army))
        {
            Debug.LogError($"No army registered with ID {armyId}");
            return;
        }

        if (armyPathfindingTester == null)
        {
            Debug.LogError("ArmyPathfindingTester is not assigned!");
            return;
        }

        var data = new UnitData
        {
            UnitType = unitType,
            Position = new Vector3(x, 0, z),
            Health = unitType.MaxHp,
            ArmyId = armyId,
            TeamMaterial = armyPathfindingTester.armyMaterials[army.ArmyID % armyPathfindingTester.armyMaterials.Count],
        };

        army.SpawnUnit(data);

        Debug.Log($"Spawned {unitTypeName} at ({x}, {z}) in Army {armyId}");
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
