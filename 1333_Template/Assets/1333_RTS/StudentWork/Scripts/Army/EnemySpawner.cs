using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefabL1; // Reference to the level 1 enemy prefab
    [SerializeField] private GameObject enemyPrefabL2; // Reference to the level 2 enemy prefab
    [SerializeField] private GameObject enemyPrefabL3; // Reference to the level 3 enemy prefab
    private GameObject currentEnemyPrefab; // Current enemy prefab to spawn
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform[] startingSpawnPoints; // array of positions to pick from
    [SerializeField] private Transform[] wave2SpawnPoints; // array of positions to pick from
    [SerializeField] private Transform[] wave3SpawnPoints; // array of positions to pick from
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject healthBarPrefab; //goal: pass camera to HealthBar component
    [SerializeField] private UnitType unitTypeL1; // Reference to the UnitType asset
    [SerializeField] private UnitType unitTypeL2; // Reference to the UnitType asset
    [SerializeField] private UnitType unitTypeL3; // Reference to the UnitType asset
    private UnitType currentUnitType; // Current unit type to spawn
    //you need 3 diff UnitTypes for 3 levels of enemies
    [SerializeField] private PathFinder pathFinder; // Reference to the PathFinder component
    [SerializeField] private ArmyMaterialSelector armyMaterialSelector; // Reference to the ArmyMaterialSelector component

    private int spawnCount = 0; // Track how many enemies spawned
    private int waveCount = 0; // Track the current wave
    [SerializeField] private List<Transform> currentSpawnPoints; // Current spawn points based on wave
    [SerializeField] private int maxSpawnCount; //number will be bigger in later waves
    [SerializeField] private ArmyData finalWaveArmy;

    private IEnumerator StartSpawningWhenReady()
    {
        // Wait until both player and enemy are set
        yield return new WaitUntil(() => armyMaterialSelector.materialsSelected);
        // now safe to spawn
        StartCoroutine(SpawnLoop());
        Debug.Log("Enemy spawner started spawning enemies.");
    }

    private void Start()
    {
        currentUnitType = unitTypeL1; // Set the initial unit type to level 1
        currentEnemyPrefab = enemyPrefabL1; // Set the initial enemy prefab to level 1
        for (int i = 0; i < startingSpawnPoints.Length; i++)
        {
            currentSpawnPoints.Add(startingSpawnPoints[i]);
            Debug.Log($"Added starting spawn point {startingSpawnPoints[i].position} to current spawn points.");
        }
        if (armyMaterialSelector == null)
        {
            Debug.LogError("ArmyMaterialSelector reference not assigned in EnemySpawner!");
        }
        else
        {
            armyMaterialSelector.OnArmiesReady += SetupSpawning;
            Debug.Log("ArmyMaterialSelector is assigned.");
        }
        //if (finalWaveArmy == null)
        //{
        //    Debug.LogError("FinalWaveArmy reference not assigned in EnemySpawner!");
        //}
        //else
        //{
        //    finalWaveArmy.OnFinalWaveStart += StartFinalWave;
        //}          
        StartCoroutine(StartSpawningWhenReady());
        Debug.Log("EnemySpawner is waiting for materials to be selected.");
    }

    private void SetupSpawning()
    {
        StartCoroutine(SpawnLoop());
        Debug.Log("EnemySpawner: started spawning.");
    }

    //void StartFinalWave(ArmyData triggeringArmy)
    //{
    //    StartCoroutine(SpawnFinalWaveForArmy(triggeringArmy));
    //}

    //IEnumerator SpawnFinalWaveForArmy(ArmyData army)
    //{
    //    GameObject castleObj = army.Buildings
    //        .FirstOrDefault(b => b.Data.buildingName == "Castle")?.gameObject;

    //    if (castleObj == null)
    //    {
    //        Debug.LogWarning("Final wave: no castle found!");
    //        yield break;
    //    }

    //    Transform castlePoint = castleObj.transform;
    //    for (int i = 0; i < startingSpawnPoints.Length; i++)
    //    {
    //        GameObject unitObj = Instantiate(enemyPrefab, castlePoint.position, Quaternion.identity);
    //        var unit = unitObj.GetComponent<UnitInstance>();
    //        // Initialize unit (pathfinder, materials…)
    //        // ...

    //        Vector3 dest = startingSpawnPoints[i].position;
    //        unit.SetDestination(dest);
    //        army.Units.Add(unit);
    //        yield return new WaitForSeconds(0.3f);
    //    }

    //    StartCoroutine(FinalWaveAI(army));
    //}

    //IEnumerator FinalWaveAI(ArmyData army)
    //{
    //    while (army.Units.Count > 0)
    //    {
    //        foreach (var unit in army.Units.ToList())
    //        {
    //            if (unit.IsDead) continue;

    //            var enemyUnits = AllArmiesManager.Instance
    //                                .AllArmies
    //                                .Where(a => a != army)
    //                                .SelectMany(a => a.Units)
    //                                .Where(u => !u.IsDead);

    //            UnitInstance target = enemyUnits
    //                .OrderBy(u => Vector3.Distance(unit.transform.position, u.transform.position))
    //                .ThenBy(u => u.CurrentHealth)
    //                .FirstOrDefault();

    //            if (target != null)
    //                unit.Attack(target);
    //        }
    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    public void SpawnWave(int waveNumber)
    {
        Debug.Log($"Spawning wave {waveNumber}");
        waveCount = waveNumber;
        spawnCount = 0;

        switch (waveNumber)
        {
            case 1:
                maxSpawnCount = currentSpawnPoints.Count;
                break;
            case 2:
                for (int i = 0; i < wave2SpawnPoints.Length; i++)
                {
                    currentSpawnPoints.Add(wave2SpawnPoints[i]);
                    Debug.Log($"Added starting spawn point {wave2SpawnPoints[i].position} to current spawn points.");
                }
                currentUnitType = unitTypeL2; // Change to level 2 unit type
                currentEnemyPrefab = enemyPrefabL2; // Change to level 2 enemy prefab
                maxSpawnCount = currentSpawnPoints.Count;
                break;
            case 3:
                for (int i = 0; i < wave3SpawnPoints.Length; i++)
                {
                    currentSpawnPoints.Add(wave3SpawnPoints[i]);
                    Debug.Log($"Added starting spawn point {wave3SpawnPoints[i].position} to current spawn points.");
                }
                currentUnitType = unitTypeL3; // Change to level 3 unit type
                currentEnemyPrefab = enemyPrefabL3; // Change to level 3 enemy prefab
                maxSpawnCount = currentSpawnPoints.Count;
                break;
            default:
                Debug.LogWarning("No setup for this wave!");
                break;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        foreach (Transform spawnPoint in currentSpawnPoints)
        {
            SpawnEnemy(spawnPoint);
            spawnCount++;
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("All enemies in this wave spawned.");
    }

    private void SpawnEnemy(Transform spawnPoint)
    {
        Debug.Log($"SpawnPoints array is {(startingSpawnPoints == null ? "NULL" : startingSpawnPoints.Length.ToString())}");
        if (startingSpawnPoints == null || startingSpawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned — aborting spawn.");
            return;
        }

        //foreach loop starts here

        // Pick a random spawn point
        Vector2Int spawnPoint2 = new Vector2Int ((int)spawnPoint.position.x, (int)spawnPoint.position.z);

        // Instantiate the enemy prefab
        GameObject enemyObj = Instantiate(currentEnemyPrefab, spawnPoint.position, Quaternion.identity); //which enemyPrefab?

        // Initialize UnitInstance (or your own component)
        UnitInstance unit = enemyObj.GetComponent<UnitInstance>();
        if (unit != null)
        {
            GameObject armyGO = GameObject.Find("AM (1)");
            if (armyGO == null)
            {
                Debug.LogError("Army GameObject 'AM (1)' not found!");
                return;
            }

            ArmyData armyData = armyGO.GetComponent<ArmyData>();
            if (armyData == null)
            {
                Debug.LogError("'AM (1)' does not have an ArmyData component!");
                return;
            }

            if (armyData != null)
            {
                unit.Army = armyData; // if you want the reference
                armyData.Units.Add(unit);
                Debug.Log($"Added unit {unit.name} to ArmyID 1.");
                Debug.Log($"Units count now: {armyData.Units.Count}");
            }
            else
            {
                Debug.LogError("Could not find ArmyData with ArmyID 1!");
            }

            unit.Initialize(pathFinder, armyData.TeamMaterial, gridManager, currentUnitType, spawnPoint2, armyData, 1);
            unit.MaxHealth = currentUnitType.MaxHp;
            unit.CurrentHealth = currentUnitType.MaxHp;

            // Calculate path
            // too early to calculate path at start
            GridNode startNode = gridManager.GetNodeFromWorldPosition(spawnPoint.position);
            GridNode endNode = gridManager.EndNode;

            List<Vector3> path = gridManager.pathFinder.CalculatePath(startNode, endNode);
            unit.SetPath(path);

            Debug.Log($"Spawned enemy with ArmyID 1 and path from {startNode.Name} to {endNode.Name}");
        }
        else
        {
            Debug.LogError("Spawned enemy prefab is missing UnitInstance component.");
        }
    }

}
