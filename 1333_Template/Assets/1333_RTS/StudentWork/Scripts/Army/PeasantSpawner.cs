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
        gridManager = GameObject.Find("GridM").GetComponent<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found on PeasantSpawner.");
        }
        pathFinder = GameObject.Find("PathFinder").GetComponent<PathFinder>();
        if (pathFinder == null)
        {
            Debug.LogError("PathFinder not found on PeasantSpawner.");
        }
        armyData = GameObject.Find("AM").GetComponent<ArmyData>();
        if (armyData == null)
        {
            Debug.LogError("ArmyData not found on PeasantSpawner.");
        }
    }

    void Update()
    {
        if (Time.time >= lastSpawnTime + spawnCooldown)
        {
            Instantiate(peasantPrefab, spawnPoint.position, Quaternion.identity);
            PeasantUnit peasantUnit = peasantPrefab.GetComponent<PeasantUnit>();
            peasantUnit.Initialize(pathFinder, armyData.TeamMaterial, gridManager, peasantUnitType, spawnGridPosition, armyData, armyData.ArmyID);
            lastSpawnTime = Time.time;
            peasantUnit.linkedBuilding = house;
        }
    }

}
