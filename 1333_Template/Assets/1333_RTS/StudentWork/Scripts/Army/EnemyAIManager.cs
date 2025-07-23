using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{
    [SerializeField] private ArmyData playerArmyData;
    [SerializeField] private ArmyData enemyArmyData;
    private List<UnitInstance> playerUnits = new();
    private List<UnitInstance> enemyUnits = new();
    private List <BuildingBase> playerBuildings = new();
    private List<BuildingBase> enemyBuildings = new();
    private bool delayTimerStarted = false;
    private float delayStartTime;

    private void Start()
    {
        // List of all player units
        List<UnitInstance> playerUnits = (List<UnitInstance>)playerArmyData.Units; //needs to get it from player's army data
        List<BuildingBase> playerBuildings = (List<BuildingBase>)playerArmyData.Buildings; //needs to get it from player's army data
        if (playerArmyData == null)
        {
            Debug.LogError("Player Army Data is not assigned in EnemyAIManager!");
            return;
        }

        else
        {
            if (playerArmyData.Units == null)
            {
                Debug.LogError("Player Army Data Units list is null!");
                return;
            }
            if (playerArmyData.Buildings == null)
            {
                Debug.LogError("Player Army Data Buildings list is null!");
                return;
            }
        }

            // List of all enemy units
            List<UnitInstance> enemyUnits = (List<UnitInstance>)enemyArmyData.Units;

        if (enemyArmyData == null)
        {
            Debug.LogError("Enemy Army Data is not assigned in EnemyAIManager!");
            return;
        }
        else
        {
            if (enemyArmyData.Units == null)
            {
                Debug.LogError("Enemy Army Data Units list is null!");
                return;
            }
            if (enemyArmyData.Buildings == null)
            {
                Debug.LogError("Enemy Army Data Buildings list is null!");
                return;
            }
        }

            InvokeRepeating(nameof(UpdateEnemyAI), 1f, 1f);
    }

    private void UpdateEnemyAI()
    {
        // How many player buildings exist?
        int numPlayerBuildings = playerBuildings.Count;

        // Require castle + 3 other buildings
        if (numPlayerBuildings < 4)
        {
            Debug.Log($"Player has only {numPlayerBuildings} buildings. Needs 4+ to trigger AI.");
            return;
        }

        // Check how many player units are alive
        int alivePlayerUnits = 0;
        foreach (var unit in playerArmyData.Units)
        {
            if (unit != null && !unit.IsDead)
                alivePlayerUnits++;
        }

        // Start timer when the 5th unit spawns
        if (alivePlayerUnits >= 5 && !delayTimerStarted && numPlayerBuildings >= 4)
        {
            delayTimerStarted = true;
            delayStartTime = Time.time;
            Debug.Log("5 player units detected, starting 5-second delay...");
        }

        // If timer has started, but not yet reached 5 seconds, wait
        if (delayTimerStarted && Time.time < delayStartTime + 5f)
        {
            Debug.Log("Waiting for 5-second delay before AI attacks...");
            return;
        }

        // If fewer than 2 player units, do nothing
        if (alivePlayerUnits < 5)
        {
            Debug.Log("Player doesn't have enough units to trigger AI attack.");
            return;
        }

        // All conditions met: attack
        //how to decide which one first?
        AttackBestTarget();
        //FindAndAttackClosestPlayerUnit();
        //FindAndAttackClosestPlayerBuilding();
        Debug.Log("Enemy AI is attacking player units now.");
    }

    //public void FindAndAttackClosestPlayerUnit() //how to merge with AttackBestTarget()?
    //{
    //    // Make sure you have enemies and players
    //    if (enemyUnits == null || playerUnits == null)
    //    {
    //        Debug.LogError("Enemy or Player units list not set.");
    //        return;
    //    }

    //    UnitInstance chosenEnemy = null;
    //    UnitInstance chosenTarget = null;
    //    float minOverallDistance = Mathf.Infinity;

    //    //How to restrict attacks when there are <5 player units haven't been set yet? 
    //    //Limit attacks to at least 5 seconds after the 5th unit is spawned

    //    // Loop through each enemy
    //    foreach (var enemy in enemyUnits)
    //    {
    //        if (enemy == null || enemy.IsDead)
    //            continue;

    //        UnitInstance closestPlayer = null;
    //        float minDistance = Mathf.Infinity;

    //        // For this enemy, find the closest player unit
    //        foreach (var player in playerUnits)
    //        {
    //            if (player == null || player.IsDead)
    //                continue;

    //            float dist = Vector3.Distance(enemy.transform.position, player.transform.position);
    //            if (dist < minDistance)
    //            {
    //                minDistance = dist;
    //                closestPlayer = player;
    //            }
    //        }

    //        // Compare to overall best so far
    //        if (closestPlayer != null && minDistance < minOverallDistance)
    //        {
    //            minOverallDistance = minDistance;
    //            chosenEnemy = enemy;
    //            chosenTarget = closestPlayer;
    //        }
    //    }

    //    if (chosenEnemy != null && chosenTarget != null)
    //    {
    //        Debug.Log($"Enemy {chosenEnemy.name} will chase {chosenTarget.name} at distance {minOverallDistance}");

    //        // Tell the enemy to move towards the target's position
    //        chosenEnemy.SetDestination(chosenTarget.transform.position);
    //        chosenEnemy.Attack(chosenTarget);
    //    }
    //    else
    //    {
    //        if (chosenEnemy == null)
    //            Debug.LogWarning("No valid enemy found to attack.");
    //        if (chosenTarget == null)
    //            Debug.LogWarning("No valid target found for enemy to attack.");
    //    }
    //}

    //public void FindAndAttackClosestPlayerBuilding() //how to merge with AttackBestTarget()?
    //{
    //    // Make sure you have enemies and players
    //    if (enemyUnits == null || playerBuildings == null)
    //    {
    //        Debug.LogError("Enemy units or Player buildings list not set.");
    //        return;
    //    }

    //    UnitInstance chosenEnemy = null;
    //    BuildingInstance chosenTargetBuilding = null;
    //    float minOverallDistance = Mathf.Infinity;

    //    //How to restrict attacks when there are <5 player units haven't been set yet? 
    //    //Limit attacks to at least 5 seconds after the 5th unit is spawned

    //    // Loop through each enemy
    //    foreach (var enemy in enemyUnits)
    //    {
    //        if (enemy == null || enemy.IsDead)
    //            continue;

    //        BuildingInstance closestBuilding = null;
    //        float minDistance = Mathf.Infinity;

    //        // For this enemy, find the closest player unit
    //        foreach (var building in playerBuildings)
    //        {
    //            if (building == null || building.IsDead)
    //                continue;

    //            float dist = Vector3.Distance(enemy.transform.position, building.transform.position);
    //            if (dist < minDistance)
    //            {
    //                minDistance = dist;
    //                closestBuilding = (BuildingInstance)building;
    //            }
    //        }

    //        // Compare to overall best so far
    //        if (closestBuilding != null && minDistance < minOverallDistance)
    //        {
    //            minOverallDistance = minDistance;
    //            chosenEnemy = enemy;
    //            chosenTargetBuilding = closestBuilding;
    //        }
    //    }

    //    if (chosenEnemy != null && chosenTargetBuilding != null)
    //    {
    //        Debug.Log($"Enemy {chosenEnemy.name} will chase {chosenTargetBuilding.name} at distance {minOverallDistance}");

    //        // Tell the enemy to move towards the target's position
    //        chosenEnemy.SetDestination(chosenTargetBuilding.transform.position);
    //        chosenEnemy.AttackBuilding(chosenTargetBuilding);
    //    }
    //    else
    //    {
    //        if (chosenEnemy == null)
    //            Debug.LogWarning("No valid enemy found to attack.");
    //        if (chosenTargetBuilding == null)
    //            Debug.LogWarning("No valid target building found for enemy to attack.");
    //    }
    //}

    void AttackBestTarget()
    {
        if (enemyUnits == null)
        {
            Debug.LogError("Missing lists for enemy units.");
            return;
        }

        if (playerUnits == null)
        {
            Debug.LogError("Missing lists for player units.");
            return;
        }
        
        if (playerBuildings == null)
        {
            Debug.LogError("Missing lists for player buildings.");
            return;
        }

        UnitInstance selectedEnemy = null;
        object selectedTarget = null;  // can be UnitInstance or BuildingBase
        float bestScore = Mathf.Infinity;

        // Combine units & buildings
        var players = new List<object>();
        players.AddRange(playerUnits.Where(u => u != null && !u.IsDead));
        players.AddRange(playerBuildings.Where(b => b != null && !b.IsDead));

        foreach (var enemy in enemyUnits)
        {
            if (enemy == null || enemy.IsDead) continue;

            foreach (var target in players)
            {
                Vector3 targetPos = (target is UnitInstance u) ? u.transform.position
                                      : (target as BuildingBase).transform.position;
                float distance = Vector3.Distance(enemy.transform.position, targetPos);
                float hp = (target is UnitInstance ui) ? ui.CurrentHealth : (target as BuildingBase).Hp;
                float maxHp = (target is UnitInstance u1) ? u1.MaxHealth : (target as BuildingInstance).MaxHealth;

                float healthRatio = hp / maxHp;
                float weight = 30f; // tweak until behavior feels right
                float score = distance + healthRatio * weight;

                if (score < bestScore)
                {
                    bestScore = score;
                    selectedEnemy = enemy;
                    selectedTarget = target;
                }
            }
        }

        if (selectedEnemy != null && selectedTarget != null)
        {
            Vector3 dest = (selectedTarget is UnitInstance uT)
                ? uT.transform.position
                : (selectedTarget as BuildingBase).transform.position;

            selectedEnemy.SetDestination(dest);
            if (selectedTarget is UnitInstance uiT)
            {
                selectedEnemy.Attack(uiT);
            }
            else
            {
                selectedEnemy.AttackBuilding(selectedTarget as BuildingInstance);
            }

            Debug.Log($"AI: {selectedEnemy.name} ? attacking target {selectedTarget} (score: {bestScore})");
        }
        else
        {
            if (selectedEnemy == null)
                Debug.LogWarning("No valid enemy found for attack.");
            if (selectedTarget == null)
                Debug.LogWarning("No valid target found for attack.");
        }
    }


}






