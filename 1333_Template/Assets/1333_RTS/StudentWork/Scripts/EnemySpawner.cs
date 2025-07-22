using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform[] spawnPoints; // array of positions to pick from
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private UnitType unitType; // Reference to the UnitType asset
    [SerializeField] private PathFinder pathFinder; // Reference to the PathFinder component
    [SerializeField] private ArmyMaterialSelector armyMaterialSelector; // Reference to the ArmyMaterialSelector component

    private int spawnCount = 0; // Track how many enemies spawned
    [SerializeField] private int maxSpawnCount = 13;
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
        if (armyMaterialSelector == null)
        {
            Debug.LogError("ArmyMaterialSelector reference not assigned in EnemySpawner!");
        }
        else
        {
            armyMaterialSelector.OnArmiesReady += SetupSpawning;
            Debug.Log("ArmyMaterialSelector is assigned.");
        }
        if (finalWaveArmy == null)
        {
            Debug.LogError("FinalWaveArmy reference not assigned in EnemySpawner!");
        }
        else
        {
            finalWaveArmy.OnFinalWaveStart += StartFinalWave;
        }          
        StartCoroutine(StartSpawningWhenReady());
        Debug.Log("EnemySpawner is waiting for materials to be selected.");
    }

    private void SetupSpawning()
    {
        StartCoroutine(SpawnLoop());
        Debug.Log("EnemySpawner: started spawning.");
    }

    void StartFinalWave(ArmyData triggeringArmy)
    {
        StartCoroutine(SpawnFinalWaveForArmy(triggeringArmy));
    }

    IEnumerator SpawnFinalWaveForArmy(ArmyData army)
    {
        GameObject castleObj = army.Buildings
            .FirstOrDefault(b => b.Data.buildingName == "Castle")?.gameObject;

        if (castleObj == null)
        {
            Debug.LogWarning("Final wave: no castle found!");
            yield break;
        }

        Transform castlePoint = castleObj.transform;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject unitObj = Instantiate(enemyPrefab, castlePoint.position, Quaternion.identity);
            var unit = unitObj.GetComponent<UnitInstance>();
            // Initialize unit (pathfinder, materials…)
            // ...

            Vector3 dest = spawnPoints[i].position;
            unit.SetDestination(dest);
            army.Units.Add(unit);
            yield return new WaitForSeconds(0.3f);
        }

        StartCoroutine(FinalWaveAI(army));
    }

    IEnumerator FinalWaveAI(ArmyData army)
    {
        while (army.Units.Count > 0)
        {
            foreach (var unit in army.Units.ToList())
            {
                if (unit.IsDead) continue;

                var enemyUnits = AllArmiesManager.Instance
                                    .AllArmies
                                    .Where(a => a != army)
                                    .SelectMany(a => a.Units)
                                    .Where(u => !u.IsDead);

                UnitInstance target = enemyUnits
                    .OrderBy(u => Vector3.Distance(unit.transform.position, u.transform.position))
                    .ThenBy(u => u.CurrentHealth)
                    .FirstOrDefault();

                if (target != null)
                    unit.Attack(target);
            }
            yield return new WaitForSeconds(1f);
        }
    }


    private IEnumerator SpawnLoop()
    {
        while (spawnCount < maxSpawnCount)
        {
            SpawnEnemy();
            spawnCount++;
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("Reached max spawn count. Stopping spawner.");
    }

    private void SpawnEnemy()
    {
        Debug.Log($"SpawnPoints array is {(spawnPoints == null ? "NULL" : spawnPoints.Length.ToString())}");
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned — aborting spawn.");
            return;
        }

        // Pick a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2Int spawnPoint2 = new Vector2Int ((int)spawnPoint.position.x, (int)spawnPoint.position.z);

        // Instantiate the enemy prefab
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Initialize UnitInstance (or your own component)
        UnitInstance unit = enemyObj.GetComponent<UnitInstance>();
        if (unit != null)
        {
            // Always assign ArmyID 1
            unit.ArmyID = 1;

            // Try to find the ArmyData with ID 1
            ArmyData armyData = FindObjectsOfType<ArmyData>()
                .FirstOrDefault(a => a.ArmyID == 1);

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

            unit.Initialize(pathFinder, armyData.TeamMaterial, gridManager, unitType, spawnPoint2, armyData, 1);
            unit.MaxHealth = unitType.MaxHp;
            unit.CurrentHealth = unitType.MaxHp;

            // Calculate path
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

        // Optionally create and attach a health bar
        if (healthBarPrefab != null)
        {
            GameObject hbObj = Instantiate(healthBarPrefab);
            HealthBar hb = hbObj.GetComponent<HealthBar>();
            if (hb != null)
            {
                hb.Initialize(
                    enemyObj.transform,
                    unit,
                    mainCamera,
                    new Vector3(0, 2, 0) // Offset for the health bar
                );
            }
            else
            {
                Debug.LogError("Spawned health bar prefab has no HealthBar component!");
            }
        }
        else
        {
            Debug.LogWarning("Health bar prefab was null, skipping health bar.");
        }
    }

}
