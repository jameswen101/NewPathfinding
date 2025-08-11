using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAIManager : MonoBehaviour
{
    [SerializeField] private ArmyData playerArmyData;
    [SerializeField] private ArmyData enemyArmyData;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private AudioManager audioManager; // Prefab for enemy units
    private List<UnitInstance> playerUnits;
    private List<UnitInstance> enemyUnits;
    private List <BuildingInstance> playerBuildings;
    private List<BuildingInstance> enemyBuildings;
    private int alivePlayerUnits;
    private int numPlayerBuildings;

    private bool delayTimerStarted = false;
    public float delayStartTime;
    [SerializeField] private float attackDelayDuration = 5f; // 5 seconds delay before AI attacks
    private bool readyToAttack = false;
    private float aiTimer = 0f;
    public bool startedAttacking = false;
    private bool attackCoolingDown = false;
    private float cooldownTimer = 2f; // Timer for attack cooldown
    private bool hasAttacked = false; // Flag to check if AI has attacked at least once
    private bool isAttacking = false;
    private float attackInterval = 2f; // attack every 2 seconds
    private int enemiesPerWave = 4;
    public int minPlayerUnitsThisWave = 7; // Minimum player units required to trigger AI attack
    public int minPlayerBuildingsThisWave = 4; // Minimum player buildings required to trigger AI attack

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private AvailableTeamUnits availableTeamUnits;
    [SerializeField] private BuildingTypes buildingTypes;
    [SerializeField] private UnitType mageType;
    [SerializeField] private UnitType policeType;
    [SerializeField] private BuildingData houseData;
    public int waveNumber = 0;
    public bool isUnlockedScreenOpen = false;
    [SerializeField] private bool wave2Started = false;
    [SerializeField] private bool wave3Started = false;
    private bool wave1Ended = false;
    private bool wave2Ended = false;
    private bool wave1To2Started = false;
    private bool wave2To3Started = false;
    public GameObject upcomingTarget;
    public UnitInstance selectedSource; // The unit that is currently attacking

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
            waveNumber = 1;
            enemySpawner.SpawnWave(waveNumber); // or 2, or 3 depending on your logic
            UpdateEnemyAI();
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

    private IEnumerator TransitionWave1To2()
    {
        availableTeamUnits.AddUnit(mageType); // Add Mage to available units after wave 1 is cleared
        audioManager.PlaySFX("Level Up Short"); // Play level up sound
        SceneManager.LoadScene("MageUnlockScreen", LoadSceneMode.Additive); // Load mage unlock screen on top of existing scene
        isUnlockedScreenOpen = true;
        yield return new WaitForSecondsRealtime(7f); // works even if timescale = 0
        if (SceneManager.GetSceneByName("MageUnlockScreen").isLoaded)
        {
            SceneManager.UnloadSceneAsync("MageUnlockScreen");
            isUnlockedScreenOpen = false;
            Debug.Log($"Closed {"MageUnlockScreen"} after {7f} seconds.");
        }
        if (!wave2Started) //not gonna work since wave 1 already ended
        {
            Debug.Log("Mage unlocked screen closed. Wave 1 is coming to an end.");
            minPlayerUnitsThisWave = 9; // Increase minimum player units for wave 2
            minPlayerBuildingsThisWave = 5; // Increase minimum player buildings for wave 2
            Debug.Log($"Minimum player units for wave 2: {minPlayerUnitsThisWave}, minimum player buildings for wave 2: {minPlayerBuildingsThisWave}");

        }
        else
        {
            Debug.Log("Mage unlock screen is still open. Wave 2 will start after the screen is closed.");
        }
        StartNextWave();
        Debug.Log("Wave 2 starting now!");
        enemySpawner.SpawnWave(2); // Start wave 2, breaks the loop
        wave1Ended = true; // Mark wave 1 as ended, breaks the loop
        wave2Started = true; //breaks the loop
    }

    private IEnumerator TransitionWave2To3()
    {
        availableTeamUnits.AddUnit(policeType); // Add Police to available units after wave 2 is cleared
        audioManager.PlaySFX("Level Up Short"); // Play level up sound
        SceneManager.LoadScene("PoliceUnlockScreen", LoadSceneMode.Additive); // Load police unlock screen on top of existing scene
        isUnlockedScreenOpen = true;
        yield return new WaitForSecondsRealtime(7f); // works even if timescale = 0
        if (SceneManager.GetSceneByName("PoliceUnlockScreen").isLoaded)
        {
            SceneManager.UnloadSceneAsync("PoliceUnlockScreen");
            isUnlockedScreenOpen = false;
            Debug.Log($"Closed {"PoliceUnlockScreen"} after {7f} seconds.");
        }
        buildingTypes.AddBuilding(houseData); // Add House to available buildings after wave 2 is cleared
        audioManager.PlaySFX("Level Up Short"); // Play level up sound
        SceneManager.LoadScene("HouseUnlockScreen", LoadSceneMode.Additive); // Load house unlock screen on top of existing scene
        isUnlockedScreenOpen = true;
        yield return new WaitForSecondsRealtime(7f); // works even if timescale = 0
        if (SceneManager.GetSceneByName("HouseUnlockScreen").isLoaded)
        {
            SceneManager.UnloadSceneAsync("HouseUnlockScreen");
            isUnlockedScreenOpen = false;
            Debug.Log($"Closed {"HouseUnlockScreen"} after {7f} seconds.");
        }
        if (!wave3Started) //not gonna work since wave 2 already ended
        {
            Debug.Log("Police unlocked screen closed. Wave 2 is coming to an end.");
            minPlayerUnitsThisWave = 11; // Increase minimum player units for wave 3
            minPlayerBuildingsThisWave = 6; // Increase minimum player buildings for wave 3
            Debug.Log($"Minimum player units for wave 3: {minPlayerUnitsThisWave}, minimum player buildings for wave 3: {minPlayerBuildingsThisWave}");
        }
        else
        {
            Debug.Log("Police unlock screen is still open. Wave 3 will start after the screen is closed.");
        }
        StartNextWave();
        Debug.Log("Wave 3 starting now!");
        enemySpawner.SpawnWave(3); // Start wave 3, breaks the loop
        wave2Ended = true; // Mark wave 2 as ended, breaks the loop
        wave3Started = true; //breaks the loop
    }

    private bool CanTransitionWave2()
    {
        if (waveNumber != 1) return false;
        if (wave1To2Started || wave2Started || wave1Ended) return false;
        if (!hasAttacked) return false;
        if (!playerArmyData || !enemyArmyData) return false;

        // defensive: treat null/missing entries as dead, and prune if you want
        var enemyAlive = (enemyArmyData.Units ?? new List<UnitInstance>())
            .Any(u => u != null && !u.IsDead);

        return playerArmyData.unitKillCount >= 12 && !enemyAlive;
    }

    private bool CanTransitionWave3()
    {
        if (waveNumber != 2) return false;
        if (wave2To3Started || wave3Started || wave2Ended) return false;
        if (!hasAttacked) return false;
        if (!playerArmyData || !enemyArmyData) return false;
        // defensive: treat null/missing entries as dead, and prune if you want
        var enemyAlive = (enemyArmyData.Units ?? new List<UnitInstance>())
            .Any(u => u != null && !u.IsDead);
        var buildingsAlive = (enemyArmyData.Buildings ?? new List<BuildingInstance>())
            .Any(b => b != null && !b.IsDead);
        return playerArmyData.unitKillCount >= 30 && !enemyAlive && !buildingsAlive;
    }

    void Update()
    {
        UpdateEnemyAI();
         
        if (!startedAttacking) //if startedAttacking is still true when it shouldn't, then this will be skipped
        {
            HandleDelayCountdown(alivePlayerUnits, numPlayerBuildings);
        }

        if (!wave1To2Started && CanTransitionWave2())
        {
            wave1To2Started = true;          // set immediately to avoid double-fire
            StartCoroutine(TransitionWave1To2());
        }

       if (!wave2To3Started && CanTransitionWave3())
        {
            wave2To3Started = true;          // set immediately to avoid double-fire
            if (!isUnlockedScreenOpen)
            {
                StartCoroutine(TransitionWave2To3());
            }
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
        Debug.Log($"[AI] Player buildings: {playerBuildings.Count}, Player units: {playerUnits.Count}");
        Debug.Log($"[AI] Enemy units: {enemyUnits.Count}, Enemy buildings: {enemyBuildings.Count}");


        int numPlayerBuildings = playerBuildings.Count;
        if (numPlayerBuildings < minPlayerBuildingsThisWave)
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
        if (aliveUnits >= minPlayerUnitsThisWave && numBuildings >= minPlayerBuildingsThisWave)
        {
            if (!ValidateForAttack()) return;
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
                Debug.Log("Enemy AI is ready to attack!");
                StartCoroutine(AttackRoutine());
            }
        }
    }

    private bool ValidateForAttack()
    {
        if (!playerArmyData) { Debug.LogError("playerArmyData is NULL"); return false; }
        if (!enemyArmyData) { Debug.LogError("enemyArmyData is NULL"); return false; }
        if (playerArmyData.Units == null) { Debug.LogError("playerArmyData.Units is NULL"); return false; }
        if (enemyArmyData.Units == null) { Debug.LogError("enemyArmyData.Units is NULL"); return false; }
        if (playerArmyData.Buildings == null) { Debug.LogError("playerArmyData.Buildings is NULL"); return false; }
        if (enemyArmyData.Buildings == null) { Debug.LogError("enemyArmyData.Buildings is NULL"); return false; }
        if (!selectionManager) { Debug.LogWarning("selectionManager is NULL (will skip player auto)"); }
        return true;
    }

    private IEnumerator AttackRoutine() //currently has a null reference
    {
        Debug.Log("[AttackRoutine] start");
        if (!ValidateForAttack()) yield break;

        if (playerArmyData == null || enemyArmyData == null)
        {
            Debug.LogError("Player or enemy army data is not assigned!");
            yield break; // Exit if army data is not set
        }
        // Safe list materialization (never null)
        playerUnits = (playerArmyData.Units ?? new List<UnitInstance>()).Where(u => u && !u.IsDead).ToList();
        enemyUnits = (enemyArmyData.Units ?? new List<UnitInstance>()).Where(u => u && !u.IsDead).ToList();
        playerBuildings = (playerArmyData.Buildings ?? new List<BuildingInstance>()).Where(b => b && !b.IsDead).ToList();

        if (playerUnits == null)
        {
            Debug.LogError("Player units list is null!");
            yield break; // Exit if player units are not set
        }
        if (enemyUnits == null)
        {
            Debug.LogError("Enemy units list is null!");
            yield break; // Exit if enemy units are not set
        }
        if (playerBuildings == null)
        {
            Debug.LogError("Player buildings list is null!");
            yield break; // Exit if player buildings are not set
        }
        while (enemyUnits.Count > 0 && playerUnits.Count > 0 || playerBuildings.Count > 0)
        {
            // Confirm how many healthy enemy buildings are left
            var healthyEnemyBuildings = (enemyArmyData.Buildings ?? new List<BuildingInstance>())
                                    .Where(b => b && !b.IsDead).ToList();
            Debug.Log($"[AttackRoutine] enemy={enemyUnits.Count} players={playerUnits.Count} pBlds={playerBuildings.Count} eBlds={healthyEnemyBuildings.Count}");

            if (enemyUnits.Count > 0)
            {
                for (int i = 0; i < enemiesPerWave; i++)
                {
                    if (enemyUnits.Count == 0) break;

                    Debug.Log($"Enemy attacker {i + 1} is attacking...");
                    Debug.Log($"[AttackRoutine] enemyUnits={enemyUnits.Count} playerUnits={playerUnits.Count} playerBuildings={playerBuildings.Count}");

                    if (playerUnits.Count == 0 && playerBuildings.Count == 0)
                    {
                        Debug.Log("All player units and buildings destroyed. Ending attack routine.");
                        yield break;
                    }

                    AttackBestTarget();
                }
            }


            yield return new WaitForSeconds(attackInterval);
        }

        isAttacking = false;
        Debug.Log("[AttackRoutine] end");
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
        object selectedTarget = null;  // can be UnitInstance or BuildingInstance
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
                                      : (target as BuildingInstance).transform.position;
                float distance = Vector3.Distance(enemy.transform.position, targetPos);
                float hp = (target is UnitInstance ui) ? ui.CurrentHealth : (target as BuildingInstance).Hp;
                float maxHp = (target is UnitInstance u1) ? u1.MaxHealth : (target as BuildingInstance).MaxHealth;

                float healthRatio = maxHp > 0 ? hp / maxHp : 1f;
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
            Debug.Log($"[AttackBestTarget] attacker={selectedEnemy.name} target={(selectedTarget as Object)?.name}");
            Vector3 dest = (selectedTarget is UnitInstance uT)
                ? uT.transform.position
                : (selectedTarget as BuildingInstance).transform.position;
            selectedSource = selectedEnemy;

            selectedEnemy.SetDestination(dest);
            if (selectedTarget is UnitInstance uiT)
            {
                selectedEnemy.Attack(uiT);
            }
            else
            {
                selectedEnemy.AttackBuilding(selectedTarget as BuildingInstance);
            }

            if (selectedTarget is MonoBehaviour mb) // all components inherit from MonoBehaviour
            {
                upcomingTarget = mb.gameObject;
            }
            else
            {
                Debug.LogWarning("Unable to cast target to GameObject.");
            }

            Debug.Log($"AI: {selectedEnemy.name} → attacking target {selectedTarget} (score: {bestScore})");
            selectionManager.statusText.text = $"Enemy AI is attacking {selectedTarget}";
            cooldownTimer = 2f; // reset cooldown timer
            attackCoolingDown = true; // start cooldown after attack
            Debug.Log("Enemy AI has attacked the target and is now cooling down.");
        }
        else
        {
            Debug.LogWarning($"[AttackBestTarget] attacker={(selectedEnemy ? selectedEnemy.name : "NULL")} target={(selectedTarget == null ? "NULL" : selectedTarget.ToString())}");
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
        BuildingInstance castle = playerBuildings[0];
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