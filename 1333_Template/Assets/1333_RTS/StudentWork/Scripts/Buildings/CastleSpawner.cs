using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public UnitType unitType;  // Reference to your UnitType asset
    public int count;          // Number to spawn
}

public class CastleSpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnEntry> spawnList;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay = 0.5f;

    public void TriggerSpawn()
    {
        StartCoroutine(SpawnUnits());
    }

    private IEnumerator SpawnUnits()
    {
        foreach (var entry in spawnList)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnUnit(entry.unitType);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    private void SpawnUnit(UnitType type)
    {
        var prefab = type.unitPrefab;
        var obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        // Initialize your UnitInstance here
    }
}
