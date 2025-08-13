using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeasantSpawner : MonoBehaviour
{
    [SerializeField] private GameObject peasantPrefab;
    [SerializeField] private BuildingInstance house;
    private Transform spawnPoint;
    [SerializeField] private float spawnCooldown = 5f;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathFinder pathFinder;
    [SerializeField] private ArmyData armyData;
    [SerializeField] private UnitType peasantUnitType;
    private Vector2Int spawnGridPosition;
    private bool canInitializeUnits = false; // Flag to check if units can be initialized
    private bool canSpawn = false; //wait till enemy army starts to attack to instantiate peasant units

    private float lastSpawnTime;

    private void Start()
    {
        spawnPoint = this.transform;
        spawnGridPosition = new Vector2Int(Mathf.RoundToInt(spawnPoint.position.x), Mathf.RoundToInt(spawnPoint.position.z));
        gridManager = house.GetComponent<BuildingInstance>().gridManager; //go to the house prefab -> get GridManager component
        if (gridManager == null)
        {
            Debug.LogWarning("GridManager not found on PeasantSpawner.");
        }
        pathFinder = house.GetComponent<BuildingInstance>().pathFinder; //get PathFinder from the house instance
        if (pathFinder == null)
        {
            Debug.LogWarning("PathFinder not found on PeasantSpawner.");
        }
        armyData = house.GetComponent<BuildingInstance>().Army; //get ArmyData from the house instance
        if (armyData == null)
        {
            Debug.LogWarning("ArmyData not found on PeasantSpawner.");
        }
        if (pathFinder != null && armyData != null)
        {
            canInitializeUnits = true; // Set the flag to true if both pathFinder and armyData are not null
            Debug.Log("Ready to initialize peasant units when enemies start attacking.");
        }
        else
        {
            Debug.LogWarning("Cannot initialize peasant units: pathFinder or armyData is null.");
        }
    }

    void Update()
    {
        if (canInitializeUnits) 
        {
            if (armyData._units.Count >= 7 && armyData._buildings.Count >= 4)
            {
                canSpawn = true; // Start spawning peasants when the army has at least 7 units and 3 buildings
                Debug.Log("PeasantSpawner can now spawn peasants.");
            }
            if (canSpawn && Time.time >= lastSpawnTime + spawnCooldown)
            {
                //Make sure pathFinder + armyData are not null before calling Initialize
                var go = Instantiate(peasantPrefab, spawnPoint.position, Quaternion.identity);
                var peasantUnit = go.GetComponent<PeasantUnit>(); // the instance
                peasantUnit.Initialize(pathFinder, armyData.TeamMaterial, gridManager,
                                   peasantUnitType, spawnGridPosition, armyData, armyData.ArmyID); //use Debug.Logs to test what's null here
                if (peasantUnit == null)
                {
                    Debug.LogError("PeasantUnit component not found on the spawned prefab.");
                    return;
                }
                lastSpawnTime = Time.time; // Reset the spawn timer
                Debug.Log($"Spawned {peasantUnit.name} at {spawnPoint.position} with grid position {spawnGridPosition}");
                // Set the house reference for the peasant unit
                peasantUnit.SetHouse(house); // Set the house reference for the peasant unit
                Debug.Log($"Spawner set {peasantUnit.name}'s house to {house?.name}");
            }
        }
        else
        {
            Debug.LogWarning("Cannot instantiate or initialize PeasantUnit: pathFinder or armyData is null.");
        }
    }
}
