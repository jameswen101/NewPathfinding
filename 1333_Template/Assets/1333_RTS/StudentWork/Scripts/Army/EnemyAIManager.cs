using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{
    [SerializeField] private ArmyData playerArmyData;
    [SerializeField] private ArmyData enemyArmyData;
    private List<UnitInstance> playerUnits;
    private List<UnitInstance> enemyUnits;
    private List <BuildingBase> playerBuildings;
    private List<BuildingBase> enemyBuildings;
    private bool delayTimerStarted = false;
    private float delayStartTime;
    private bool readyToAttack = false;
    private float aiTimer = 0f;
    private bool startedAttacking = false;
    private bool attackCoolingDown = false;
    private float cooldownTimer = 2f; // Timer for attack cooldown

    private void Start()
    {
        // List of all player units
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
    }

    private void Update()
    {
        aiTimer += Time.deltaTime;
        if (aiTimer >= 1f)
        {
            if (!startedAttacking)
            {
                Debug.Log($"readyToAttack = {readyToAttack}");
            }
            aiTimer = 0f;
            if (!readyToAttack)
            {
                UpdateEnemyAI();
            }    
        }
    }


    private void UpdateEnemyAI()
    {
        // Use the class-level fields—not new local vars!
        playerUnits = playerArmyData.Units.ToList();
        playerBuildings = playerArmyData.Buildings.ToList();

        // Similarly for enemyUnits/playerBuildings
        enemyUnits = enemyArmyData.Units.ToList();
        enemyBuildings = enemyArmyData.Buildings.ToList();

        // How many player buildings exist?
        int numPlayerBuildings = playerBuildings.Count;

        // Require castle + 3 other buildings
        if (numPlayerBuildings < 4)
        {
            Debug.Log($"Player has only {numPlayerBuildings} buildings. Needs 4+ to trigger AI.");
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
        }

        // If fewer than 2 player units, do nothing
        if (alivePlayerUnits < 5)
        {
            Debug.Log("Player doesn't have enough units to trigger AI attack.");
        }

        // All conditions met: attack
        //how to decide which one first?
        if (alivePlayerUnits >= 5 && delayTimerStarted && numPlayerBuildings >= 4 && Time.time > delayStartTime + 5f)
        {
            readyToAttack = true;
            Debug.Log("Enemy AI is attacking player units now.");
            startedAttacking = true;
            int limiter = 0, maxTicks = 50;
            while ((playerUnits.Count > 0 || playerBuildings.Count > 0) && limiter++ < maxTicks)
            {
                while (cooldownTimer > 0f)
                {
                    cooldownTimer -= Time.deltaTime;
                }
                if (cooldownTimer <= 0f)
                {
                    attackCoolingDown = false;
                    if (!attackCoolingDown)
                    {
                        Debug.Log("Enemy AI is ready to attack.");
                        AttackBestTarget();
                    }
                }
            }
            if (limiter >= maxTicks)
                Debug.LogError("AI loop capped out at maxTicks — possible infinite loop avoided.");
        }
    }

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

            Debug.Log($"AI: {selectedEnemy.name} → attacking target {selectedTarget} (score: {bestScore})");
            cooldownTimer = 2f; // reset cooldown timer
            attackCoolingDown = true; // start cooldown after attack
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






