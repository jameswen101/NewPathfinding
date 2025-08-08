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

    private float lastSpawnTime;

    private void Start()
    {
        spawnPoint = this.transform;
        spawnGridPosition = new Vector2Int(Mathf.RoundToInt(spawnPoint.position.x), Mathf.RoundToInt(spawnPoint.position.z));
        gridManager = GameObject.Find("GridM").GetComponent<GridManager>(); //go to the house prefab -> get GridManager component
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found on PeasantSpawner.");
        }
        pathFinder = house.GetComponent<BuildingInstance>().pathFinder; //get PathFinder from the house instance
        if (pathFinder == null)
        {
            Debug.LogError("PathFinder not found on PeasantSpawner.");
        }
        armyData = house.GetComponent<BuildingInstance>().Army; //get ArmyData from the house instance
        if (armyData == null)
        {
            Debug.LogError("ArmyData not found on PeasantSpawner.");
        }
    }

    void Update()
    {
        if (Time.time >= lastSpawnTime + spawnCooldown)
        {
            var go = Instantiate(peasantPrefab, spawnPoint.position, Quaternion.identity);
            var peasantUnit = go.GetComponent<PeasantUnit>(); // the instance
            peasantUnit.Initialize(pathFinder, armyData.TeamMaterial, gridManager,
                                   peasantUnitType, spawnGridPosition, armyData, armyData.ArmyID);
            peasantUnit.SetHouse(house); // Set the house reference for the peasant unit
            Debug.Log($"Spawner set {peasantUnit.name}'s house to {house?.name}");

        }
    }

}
