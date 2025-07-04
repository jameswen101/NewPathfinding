using System.Collections.Generic;
using UnityEngine;
public class ArmyPathfindingTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathFinder pathfinder;
    [SerializeField] private List<ArmyComposition> armyCompositions = new();
    public List<Material> armyMaterials;
    public TeamMaterialsCollection teamMaterials;
    [Header("Patrol & AI Settings")]
    [SerializeField] private int patrolRadius = 8;
    [SerializeField] private float detectionDistance = 4f;
    private readonly List<ArmyData> armies = new();
    private enum UnitState { Patrol, Follow }
    private readonly Dictionary<UnitInstance, UnitState> unitStates = new();
    private readonly Dictionary<UnitInstance, Vector3[]> patrolPoints = new();
    private readonly Dictionary<UnitInstance, int> patrolTargetIndex = new();
    private readonly Dictionary<UnitInstance, UnitInstance> followTargets = new();
    private readonly Dictionary<UnitInstance, Vector3> lastKnownEnemyPos = new();
    public void Initialize()
    {
        armies.Clear();
        for (int i = 0; i < armyCompositions.Count; i++)
        {
            var army = new ArmyData { ArmyID = i + 1, GridManager = gridManager };
            SpawnArmy(army, armyCompositions[i]);
            armies.Add(army);
        }
    }
    private void SpawnArmy(ArmyData army, ArmyComposition composition)
    {
        foreach (var entry in composition.entries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                Vector3 spawnPos = FindRandomWalkableSpot(entry.unitTypePrefab.unitType.Width, entry.unitTypePrefab.unitType.Height); //currently null
                if (spawnPos == Vector3.zero) continue;
                Vector2Int spawnGridPos = new Vector2Int(Mathf.RoundToInt(spawnPos.x), Mathf.RoundToInt(spawnPos.z));
                GameObject go = Instantiate(entry.unitTypePrefab.prefab, spawnPos, Quaternion.identity);
                UnitInstance unit = go.GetComponent<UnitInstance>();
                Material armyMat = armyMaterials[army.ArmyID % armyMaterials.Count]; //change to TeamMaterialsCollection
                unit.Initialize(pathfinder, armyMat, gridManager, unit.UnitType, spawnGridPos, army, army.ArmyID); //add UnitType + convert spawnPos to Vector2Int
                army.Units.Add(unit);
                unitStates[unit] = UnitState.Patrol;
                // Assign initial patrol points
                patrolPoints[unit] = new Vector3[2] {
                    GetRandomPatrolPoint(spawnPos, unit.Width, unit.Height),
                    GetRandomPatrolPoint(spawnPos, unit.Width, unit.Height) };
                patrolTargetIndex[unit] = 0;
                // Send unit to first patrol point
                unit.SetDestination(patrolPoints[unit][0]);
            }
        }
    }
    private void Update()
    {
        foreach (ArmyData ownArmy in armies)
        {
            List<UnitInstance> enemyUnits = new();
            foreach (var army in armies)
            {
                if (army != ownArmy)
                    enemyUnits.AddRange((IEnumerable<UnitInstance>)army.Units);
            }
            UpdateArmyUnits(ownArmy, enemyUnits);
        }
    }
    private void UpdateArmyUnits(ArmyData ownArmy, List<UnitInstance> enemyUnits)
    {
        foreach (UnitInstance unit in ownArmy.Units)
        {
            if (unit == null) continue;
            UnitState state = unitStates[unit];
            switch (state)
            {
                case UnitState.Patrol: PatrolBehavior(unit); break;
                case UnitState.Follow: FollowBehavior(unit); break;
            }
        }
    }
    private void PatrolBehavior(UnitInstance unit)
    { //currently null reference
        // Switch patrol point when close enough
        Vector3[] points = patrolPoints[unit];
        int idx = patrolTargetIndex[unit];
        Vector3 target = points[idx];
        if (Vector3.Distance(unit.transform.position, target) < 0.2f)
        {
            idx = 1 - idx;
            // switch to other patrol point
            patrolTargetIndex[unit] = idx;
            // Pick a new patrol point for the new slot
            points[idx] = GetRandomPatrolPoint(unit.transform.position, unit.Width, unit.Height);
            unit.SetDestination(points[idx]);
        }
    }
    private void FollowBehavior(UnitInstance unit)
    {
        // Optional: implement your enemy-following logic later.
    }
    private Vector3 FindRandomWalkableSpot(int width, int height)
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            int x = Random.Range(0, gridManager.GridSettings.GridSizeX - width + 1);
            int y = Random.Range(0, gridManager.GridSettings.GridSizeY - height + 1);
            if (IsRegionWalkable(x, y, width, height))
                return gridManager.GetNode(x, y).WorldPosition;
        }
        Debug.LogWarning("No valid spawn location found.");
        return Vector3.zero;
    }
    private bool IsRegionWalkable(int x, int y, int width, int height)
    {
        for (int dx = 0; dx < width; dx++)
            for (int dy = 0; dy < height; dy++)
                if (!gridManager.GetNode(x + dx, y + dy).Walkable)
                    return false;
        return true;
    }
    private Vector3 GetRandomPatrolPoint(Vector3 origin, int width, int height)
    {
        var node = gridManager.GetNodeFromWorldPosition(origin);
        float size = gridManager.GridSettings.NodeSize;
        int nodeX = Mathf.RoundToInt(node.WorldPosition.x / size);
        int nodeY = Mathf.RoundToInt(node.WorldPosition.z / size);
        int x = Mathf.Clamp(Random.Range(nodeX - patrolRadius, nodeX + patrolRadius), 0, gridManager.GridSettings.GridSizeX - 1);
        int y = Mathf.Clamp(Random.Range(nodeY - patrolRadius, nodeY + patrolRadius), 0, gridManager.GridSettings.GridSizeY - 1);
        return gridManager.GetNode(x, y).WorldPosition;
    }
}