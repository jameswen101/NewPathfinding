using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{
    [SerializeField] private ArmyData playerArmyData;
    [SerializeField] private ArmyData enemyArmyData;
    [SerializeField] private SelectionManager selectionManager;
    private List<UnitInstance> playerUnits;
    private List<UnitInstance> enemyUnits;
    private List <BuildingBase> playerBuildings;
    private List<BuildingBase> enemyBuildings;
    private int alivePlayerUnits;
    private int numPlayerBuildings;

    private bool delayTimerStarted = false;
    private float delayStartTime;
    [SerializeField] private float attackDelayDuration = 5f; // 5 seconds delay before AI attacks
    private bool readyToAttack = false;
    private float aiTimer = 0f;
    private bool startedAttacking = false;
    private bool attackCoolingDown = false;
    private float cooldownTimer = 2f; // Timer for attack cooldown
    private bool hasAttacked = false; // Flag to check if AI has attacked at least once
    private bool isAttacking = false;
    private float attackInterval = 2f; // attack every 2 seconds
    private int enemiesPerWave = 2;

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private AvailableTeamUnits availableTeamUnits;
    [SerializeField] private UnitType mageType;
    public int waveNumber = 0;

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
            else
            {
                if (playerArmyData.Buildings.Any(b => b != null && b.name == "CastlePrefab (1)"))
                {
                    Debug.Log("CastlePrefab (1) is in the player's building list.");
                }
                else
                {
                    Debug.LogWarning("CastlePrefab (1) is NOT in the player's building list.");
                    var castleInst = GameObject.Find("CastlePrefab (1)")?.GetComponent<BuildingInstance>();
                    playerArmyData.Buildings.Add(castleInst);
                    Debug.Log("Player castle added to player army's building list.");
                }
            }
            enemySpawner.SpawnWave(1); // or 2, or 3 depending on your logic
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

    void Update()
    {
        UpdateEnemyAI();

        if (!startedAttacking)
        {
            HandleDelayCountdown(alivePlayerUnits, numPlayerBuildings);
        }

        // Check for wave 2
        if (playerArmyData.unitKillCount == 12 && hasAttacked && waveNumber == 1) //how to make sure they have already attacked before this round?
        {
            //check if any dead enemy units are still in the game- if there are, remove them
            enemyUnits = enemyArmyData.Units.Where(u => u != null && !u.IsDead).ToList();
            foreach (var unit in enemyUnits)
            {
                if (unit.IsDead)
                {
                    enemyArmyData.Units.Remove(unit);
                    Destroy(unit); // Destroy the dead unit
                    enemyUnits.Remove(unit); // Remove from the local list
                    Debug.Log($"Removed dead enemy unit: {unit.name}");
                }
            }
            StartNextWave();
            availableTeamUnits.AvailableUnits.Add(mageType); // Add Mage to available units after wave 1 is cleared
            Debug.Log("All wave 1 enemies dead. Starting wave 2.");
            selectionManager.statusText.text = "Wave 2 incoming!";
        }

        // Optionally wave 3 too
        if (playerArmyData.unitKillCount == 30 && hasAttacked && playerArmyData.buildingKillCount == 19 && waveNumber == 2) //how to make sure they have already attacked before this round?
        {
            //check if any dead enemy units are still in the game- if there are, remove them
            enemyUnits = enemyArmyData.Units.Where(u => u != null && !u.IsDead).ToList();
            foreach (var unit in enemyUnits)
            {
                if (unit.IsDead)
                {
                    enemyArmyData.Units.Remove(unit);
                    Destroy(unit); // Destroy the dead unit
                    enemyUnits.Remove(unit); // Remove from the local list
                    Debug.Log($"Removed dead enemy unit: {unit.name}");
                }
            }
            //check if any dead enemy walls are still in the game- if there are, remove them
            enemyBuildings = enemyArmyData.Buildings.Where(b => b != null && !b.IsDead).ToList();
            foreach (var building in enemyBuildings)
            {
                if (building.IsDead)
                {
                    enemyArmyData.Buildings.Remove(building);
                    Destroy(building); // Destroy the dead wall
                    enemyBuildings.Remove(building); // Remove from the local list
                    Debug.Log($"Removed dead enemy wall: {building.name}");
                }
            }
            StartNextWave();
            Debug.Log("All wave 2 enemies dead and all enemy walls collapsed. Starting wave 3.");
            selectionManager.statusText.text = "Wave 3 incoming!";
        }
    }


    private void StartNextWave()
    {
        waveNumber++;
        enemySpawner.SpawnWave(waveNumber);
    }

    private void UpdateEnemyAI()
    {
    playerUnits = playerArmyData.Units.Where(u => u != null && !u.IsDead).ToList();
    enemyUnits = enemyArmyData.Units.Where(u => u != null && !u.IsDead).ToList();
    playerBuildings = playerArmyData.Buildings.Where(b => b != null && !b.IsDead).ToList();
    enemyBuildings = enemyArmyData.Buildings.Where(b => b != null && !b.IsDead).ToList();

        int numPlayerBuildings = playerBuildings.Count;
        if (numPlayerBuildings < 4)
        {
            Debug.Log($"Player has only {numPlayerBuildings} buildings. Needs 4+ to trigger AI.");
        }

        int alivePlayerUnits = 0;
        foreach (var unit in playerArmyData.Units)
        {
            if (unit != null && !unit.IsDead)
                alivePlayerUnits++;
        }

        HandleDelayCountdown(alivePlayerUnits, numPlayerBuildings);
    }

    void HandleDelayCountdown(int aliveUnits, int numBuildings)
    {
        if (aliveUnits >= 10 && numBuildings >= 4)
        {
            //start wave 1 here?
            if (!delayTimerStarted)
            {
                delayTimerStarted = true;
                delayStartTime = attackDelayDuration; // e.g. 5 seconds
            }

            delayStartTime -= Time.deltaTime;
            if (delayStartTime >= 0)
            {
                selectionManager.statusText.text = $"Enemy attacks in: {Mathf.Ceil(delayStartTime)}s";
            }

            if (delayStartTime <= 0f && !startedAttacking)
            {
                readyToAttack = true;
                startedAttacking = true;
                hasAttacked = true; // AI has attacked at least once
                selectionManager.statusText.text = "Enemy is attacking!";
                StartCoroutine(AttackRoutine());
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (playerUnits.Count > 0 || playerBuildings.Count > 0)
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                if (enemyUnits.Count == 0)
                    break;

                Debug.Log($"Enemy attacker {i + 1} is attacking...");
                AttackBestTarget(); // Implement logic to choose target and attacker
            }

            yield return new WaitForSeconds(attackInterval);
        }

        isAttacking = false;
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
            selectionManager.statusText.text = $"Enemy AI is attacking {selectedTarget}";
            cooldownTimer = 2f; // reset cooldown timer
            attackCoolingDown = true; // start cooldown after attack
            Debug.Log("Enemy AI has attacked the target and is now cooling down.");
        }
        else
        {
            if (selectedEnemy == null)
                Debug.LogWarning("No valid enemy found for attack.");
            if (selectedTarget == null)
                Debug.LogWarning("No valid target found for attack.");

            var castle = playerBuildings.FirstOrDefault(b => b.name.Contains("Castle"));
            if (castle != null && playerUnits.Count == 0)
            {
                AttackClosestToCastle();
            }
        }
    }

    void AttackClosestToCastle()
    {
        if (playerBuildings.Count == 0 || enemyUnits.Count == 0)
            return;

        // Find the player's castle (assuming it's the only building remaining)
        BuildingBase castle = playerBuildings[0];
        Vector3 castlePos = castle.transform.position;

        UnitInstance closestEnemy = null;
        float minSqrDistance = float.MaxValue;

        // Loop through all enemy units
        foreach (UnitInstance enemy in enemyUnits)
        {
            if (enemy == null || enemy.IsDead)
                continue;

            float sqrDistance = (enemy.transform.position - castlePos).sqrMagnitude;
            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            // Move that enemy toward the castle and attack it
            closestEnemy.SetDestination(castlePos);
            closestEnemy.AttackBuilding(castle as BuildingInstance);
            selectionManager.statusText.text = $"Enemy AI: {closestEnemy.name} attacking castle";
        }
    }
}






