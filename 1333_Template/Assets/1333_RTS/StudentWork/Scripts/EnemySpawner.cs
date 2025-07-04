using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform[] spawnPoints; // array of positions to pick from
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject healthBarPrefab;

    private int spawnCount = 0; // Track how many enemies spawned
    [SerializeField] private int maxSpawnCount = 4; // Limit for testing, will be removed later

    private void Start()
    {
        StartCoroutine(SpawnLoop());
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
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        // Pick a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the enemy prefab
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Initialize UnitInstance (or your own component)
        UnitInstance unit = enemyObj.GetComponent<UnitInstance>();
        if (unit != null)
        {
            // You could pick any node as start and end
            GridNode startNode = gridManager.GetNodeFromWorldPosition(spawnPoint.position);
            GridNode endNode = gridManager.EndNode;

            // Calculate path and assign it
            List<Vector3> path = gridManager.pathFinder.CalculatePath(startNode, endNode);
            unit.SetPath(path);

            Debug.Log($"Spawned enemy with path from {startNode.Name} to {endNode.Name}");
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
                    unit, // assuming UnitInstance implements IHasHealth
                    mainCamera
                );
            }
        }
    }
}
