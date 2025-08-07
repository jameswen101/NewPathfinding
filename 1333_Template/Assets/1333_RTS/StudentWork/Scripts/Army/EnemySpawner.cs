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
    private bool cleanupDone = false;

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
            armyMaterialSelector.OnArmiesReady += SetupSpawning; //calls spawn the 3rd time
            Debug.Log("ArmyMaterialSelector is assigned.");
        }
       
        Debug.Log("EnemySpawner is waiting for materials to be selected.");
    }

    private void SetupSpawning()
    {
        SpawnWave(1); // Start with wave 1
        Debug.Log("EnemySpawner: started spawning.");
    }

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
                if (wave2SpawnPoints == null || wave2SpawnPoints.Length == 0)
                {
                    Debug.LogError("Wave 2 spawn points are not assigned or empty!");
                    return;
                }
                cleanupDone = false; // Reset cleanupDone for the new wave
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
                if (wave3SpawnPoints == null || wave3SpawnPoints.Length == 0)
                {
                    Debug.LogError("Wave 3 spawn points are not assigned or empty!");
                    return;
                }
                cleanupDone = false; // Reset cleanupDone for the new wave
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

        StartCoroutine(SpawnLoop()); //calls spawn the 4th time
    }

    private IEnumerator SpawnLoop()
    {
        Debug.Log($"Starting spawn loop for wave {waveCount} with max spawn count {maxSpawnCount}.");
        foreach (Transform spawnPoint in currentSpawnPoints)
        {
            SpawnEnemy(spawnPoint);
            spawnCount++;
            for (int i = 0; i < currentSpawnPoints.Count; i++)
            {
                Debug.Log($"Spawn Point {i}: {currentSpawnPoints[i].position}");
            }
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("All enemies in this wave spawned.");

        if (waveCount == 1)
        {
                //remove every 2nd soldier
                GameObject armyGO = GameObject.Find("AM (1)");
                if (armyGO == null)
                {
                    Debug.LogError("Army GameObject 'AM (1)' not found!");
                }

                ArmyData armyData = armyGO.GetComponent<ArmyData>();
                if (armyData == null)
                {
                    Debug.LogError("'AM (1)' does not have an ArmyData component!");
                }

                if (armyData != null && !cleanupDone)
            {
                    List<UnitInstance> reordered = new();
                //add the 12 units we wish to keep
                // Keep every 2nd unit from 0 to 22 -> stops at index 22
                for (int i = 0; i <= armyData.Units.Count - 2; i += 2)
                {
                    var unit = armyData.Units[i];
                    if (unit != null)
                    {
                        reordered.Add(unit);
                        Debug.Log($"Keeping Units[{i}] -> reordered[{reordered.Count - 1}]");
                    }
                }


                //add the 12 units we wish to remove
                for (int i = armyData.Units.Count - 1; i >= 1; i -= 2)
                {
                    var unit = armyData.Units[i];
                    if (unit != null)
                    {
                        reordered.Add(unit);
                        Debug.Log($"Unit[{i}] now in reordered[{reordered.Count - 1}]. These units will be removed.");
                    }
                }
                for (int i = reordered.Count - 1; i >= 12; i--)
                {
                    var unit = reordered[i];
                    if (unit != null)
                    {
                        armyData.Units.Remove(unit);
                        Debug.Log($"Removed unit {unit.name} from the reordered list.");
                    }
                }
                // Destroy GameObjects not in reordered[0-11]
                for (int i = 12; i < reordered.Count; i++)
                {
                    var unit = reordered[i];
                    if (unit != null)
                    {
                        Destroy(unit.gameObject);
                    }
                }

                // Then trim the Units list by clearing and adding only the first 12
                armyData.Units.Clear();
                for (int i = 0; i < 12 && i < reordered.Count; i++)
                {
                    armyData.Units.Add(reordered[i]);
                }

                cleanupDone = true; // Set cleanupDone to true after cleanup
                Debug.Log("Cleaned up ArmyData.Units to keep only the first 12.");
            }
                else
                {
                    Debug.LogError("Could not find ArmyData with ArmyID 1!");
                }

        }
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
