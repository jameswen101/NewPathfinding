using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;

public class DebugCommands : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ArmyManager armyManager;
    [SerializeField] private ArmyPathfindingTester armyPathfindingTester;
    [SerializeField] private AllArmiesManager allArmiesManager;

    private void OnEnable()
    {
        //string name, string description, function name
        DebugLogConsole.AddCommand("HelloWorld", "Prints a message to the console", HelloWorld);
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


    private static void PlaceBuilding(string buildingTypeName, int armyId)
    {
        var buildingType = Resources.Load<BuildingTypes>($"BuildingTypes/{buildingTypeName}");
        if (buildingType == null)
        {
            Debug.LogError($"Invalid building type: {buildingTypeName}");
            return;
        }

        if (!AllArmiesManager.Instance.TryGetArmy(armyId, out var army))
        {
            Debug.LogError($"No army registered with ID {armyId}");
            return;
        }

        BuildingPlacementManager.Instance.BeginPlacement(buildingType, army);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
