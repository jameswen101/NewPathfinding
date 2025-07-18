using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{
    [SerializeField] private ArmyData playerArmyData;
    [SerializeField] private ArmyData enemyArmyData;
    private List<UnitInstance> playerUnits = new();
    private List<UnitInstance> enemyUnits = new();
    private bool delayTimerStarted = false;
    private float delayStartTime;

    private void Start()
    {
        // List of all player units
        List<UnitInstance> playerUnits = (List<UnitInstance>)playerArmyData.Units;

        // List of all enemy units
        List<UnitInstance> enemyUnits = (List<UnitInstance>)enemyArmyData.Units;
        InvokeRepeating(nameof(UpdateEnemyAI), 1f, 1f);
    }

    private void UpdateEnemyAI()
    {
        // Check how many player units are alive
        int alivePlayerUnits = 0;
        foreach (var unit in playerArmyData.Units)
        {
            if (unit != null && !unit.IsDead)
                alivePlayerUnits++;
        }

        // Start timer when the 2nd unit spawns
        if (alivePlayerUnits >= 5 && !delayTimerStarted)
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
            return;
        }

        // All conditions met: attack
        FindAndAttackClosestPlayerUnit();
    }

    public void FindAndAttackClosestPlayerUnit()
    {
        // Make sure you have enemies and players
        if (enemyUnits == null || playerUnits == null)
        {
            Debug.LogError("Enemy or Player units list not set.");
            return;
        }

        UnitInstance chosenEnemy = null;
        UnitInstance chosenTarget = null;
        float minOverallDistance = Mathf.Infinity;

        //How to restrict attacks when there are <5 player units haven't been set yet? 
        //Limit attacks to at least 5 seconds after the 5th unit is spawned

        // Loop through each enemy
        foreach (var enemy in enemyUnits)
        {
            if (enemy == null || enemy.IsDead)
                continue;

            UnitInstance closestPlayer = null;
            float minDistance = Mathf.Infinity;

            // For this enemy, find the closest player unit
            foreach (var player in playerUnits)
            {
                if (player == null || player.IsDead)
                    continue;

                float dist = Vector3.Distance(enemy.transform.position, player.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestPlayer = player;
                }
            }

            // Compare to overall best so far
            if (closestPlayer != null && minDistance < minOverallDistance)
            {
                minOverallDistance = minDistance;
                chosenEnemy = enemy;
                chosenTarget = closestPlayer;
            }
        }

        if (chosenEnemy != null && chosenTarget != null)
        {
            Debug.Log($"Enemy {chosenEnemy.name} will chase {chosenTarget.name} at distance {minOverallDistance}");

            // Tell the enemy to move towards the target's position
            chosenEnemy.SetDestination(chosenTarget.transform.position);
            chosenEnemy.Attack(chosenTarget);
        }
        else
        {
            Debug.LogWarning("No valid enemy and target found for attack.");
        }
    }



}






