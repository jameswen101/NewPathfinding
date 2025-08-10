using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitInstance : UnitBase, IHasHealth
{
    [Header("Visuals & FX")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject skinRoot;
    [SerializeField] private LineRenderer pathLine;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;

    [SerializeField] private GridManager gridM;
    private PathFinder pathfinder;
    public List<Vector3> path = new();
    private int pathIndex = 0;
    private bool moving = false;
    private Vector3 movementTarget;

    // Public access if needed by Army Manager
    //public List<GridNode> CurrentPath => path;
    public bool IsMoving => moving; //how to use IsMoving?
    public UnitType UnitType => unitType;
    public Vector2Int OriginPoint { get; private set; }

    //[SerializeField] private ArmyData army;

    public ArmyData Army;

    [SerializeField] public int ArmyID { get; set; }

    [SerializeField] private AudioManager audioManager; // Reference to AudioManager for playing SFX

    private float walkingSFXCooldown = 0f;

    //public TextMeshPro healthText;

    [SerializeField] private GameObject healthBarPrefab;

    [SerializeField] private GameObject bloodFXPrefab;

    [SerializeField] private GameObject buildingDestroyedFXPrefab;


    protected override void Awake()
    {
        base.Awake();
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
            if (audioManager == null)
            {
                Debug.LogError("AudioManager not found in the scene!");
            }
        }
        if (gridM == null)
        {
            gridM = FindObjectOfType<GridManager>();
            if (gridM == null)
            {
                Debug.LogError("GridManager not found in the scene!");
            }
        }
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        if (healthBar == null)
        {
            Debug.LogWarning($"Unit {name} has no HealthBar assigned at Awake.");
        }
    }


    public void Initialize(PathFinder assignedPathfinder, Material teamMaterial, GridManager gridManager, UnitType unitType, Vector2Int OriginPoint, ArmyData armyData, int ArmyID)
    {
        // Debug: Check which arguments are null
        Debug.Log($@"[UnitInstance.Initialize] 
    assignedPathfinder = {(assignedPathfinder != null ? "OK" : "NULL")}
    teamMaterial = {(teamMaterial != null ? "OK" : "NULL")}
    gridManager = {(gridManager != null ? "OK" : "NULL")}
    unitType = {(unitType != null ? "OK" : "NULL")}
    armyData = {(armyData != null ? "OK" : "NULL")}
    ArmyID = {ArmyID}");

        pathfinder = assignedPathfinder;
        gridM = gridManager;
        this.unitType = unitType;
        this.OriginPoint = OriginPoint;
        Army = armyData;
        this.ArmyID = ArmyID;

        CurrentHealth = unitType.MaxHp;
        MaxHealth = unitType.MaxHp;

        // Debug logs

        if (unitType == null)
        {
            Debug.LogError("[UnitInstance.Initialize] unitType is NULL!");
            return;
        }

        if (teamMaterial == null)
        {
            Debug.LogWarning("[UnitInstance.Initialize] teamMaterial is NULL!");
            return;
        }

        Debug.Log($"{unitType.name} initialized with MaxHealth: {MaxHealth}, CurrentHealth: {CurrentHealth}");

        Debug.Log($"{unitType.name}'s ArmyData is {(armyData == null ? "NULL" : "OK")}, ArmyID = {ArmyID}");

        // Early validation
        if (armyData == null)
        {
            Debug.LogError($"[UnitInstance.Initialize] {name} missing ArmyData!");
            return;
        }

        if (ArmyID < 0)
        {
            Debug.LogError($"[UnitInstance.Initialize] {name} has invalid ArmyID: {ArmyID}");
            return;
        }

        // Fallback: if army wasn't passed in, look it up by ID
        if (Army == null)
        {
            if (AllArmiesManager.Instance != null)
            {
                if (AllArmiesManager.Instance.TryGetArmy(ArmyID, out ArmyData foundArmy))
                {
                    Army = foundArmy;
                    Debug.Log($"[{name}] Fallback assigned Army: {foundArmy.name}");
                    Debug.Log($"[{name}] Fallback assigned ArmyID: {foundArmy.ArmyID}");
                }
                else
                {
                    Debug.LogWarning($"[{name}] Army still null after TryGetArmy with ID={ArmyID}");
                }
            }
        }


        // Apply team color
        foreach (var renderer in skinRoot.GetComponentsInChildren<Renderer>())
        {
            var mats = renderer.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = teamMaterial;
            }
            renderer.materials = mats;
        }

        // Initialize it
        healthBar.GetComponent<ClickProxy>().linkedObject = this.gameObject;
        healthBar.Initialize(this.transform, this, Camera.main);
        healthBar.SetHealthText(CurrentHealth, MaxHealth); // Set initial health text
        healthBar.UpdateHealthBar(CurrentHealth, MaxHealth); // Update health bar UI
        Debug.Log($"{name}'s starting health: {CurrentHealth}/{MaxHealth}");
        Debug.Log($"{healthBar.healthText.text}");
    }



    public void SetDestination(Vector3 targetPosition)
    {
        movementTarget = targetPosition;

        // Request path from Pathfinder
        path = pathfinder.CalculatePath(gridM.GetNodeFromWorldPosition(transform.position), gridM.GetNodeFromWorldPosition(targetPosition));
        pathIndex = 0;
        moving = path != null && path.Count > 1;
        Debug.Log($"Setting destination to {targetPosition} for {name}. Moving: {moving}, Path count: {path?.Count ?? 0}");
        DrawPathLine();
        if (moving)
        {
            animator.SetBool("IsMoving", true);
            Debug.Log($"Unit {name} is now moving towards {targetPosition}.");
        }
        else
        {
            animator.SetBool("IsMoving", false);
            Debug.Log($"Unit {name} cannot move to {targetPosition}. No valid path found.");
        }
    }

    public void SetDestination(GridNode node)
    {
        SetDestination(node.WorldPosition);
        Debug.Log($"Setting destination to node at {node.WorldPosition} for {name}.");
    }

    public override void MoveTo(GridNode node)
    {
        SetDestination(node);
        Debug.Log($"Moving {name} to node at {node.WorldPosition}.");
    }

    private void Update()
    {
        // If not moving or no path, exit
        if (!moving || path == null || pathIndex >= path.Count)
        {
            return;
        }

        // Update cooldown timer
        walkingSFXCooldown -= Time.deltaTime;

        if (walkingSFXCooldown <= 0f)
        {
            PlayWalkingSFX();
            walkingSFXCooldown = 0.5f; // play every half second
        }

        // Move towards the next point
        Vector3 nextPoint = path[pathIndex];
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextPoint, step);

        // Reached current point?
        if (Vector3.Distance(transform.position, nextPoint) < 0.05f)
        {
            pathIndex++;
            if (pathIndex >= path.Count)
            {
                moving = false;
                // Optionally trigger idle animation
            }
        }

        if (CurrentHealth <= 0)
        {
            Die();
            return;
        }
    }

    private void PlayWalkingSFX()
    {
        if (audioManager == null)
        {
            Debug.LogError("AudioManager is NULL! No audio will play.");
        }
        else
        {
            Debug.Log("AudioManager exists.");
        }

        audioManager.PlaySFX("Footsteps");
    }

    private void DrawPathLine()
    {
        if (pathLine == null || path == null || path.Count == 0)
        {
            if (pathLine != null)
                pathLine.positionCount = 0;
            return;
        }

        pathLine.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            pathLine.SetPosition(i, path[i] + Vector3.up * 0.1f);
        }

        pathLine.startColor = Color.yellow;
        pathLine.endColor = Color.red;
    }

    public void Attack(UnitInstance target)
    {
        if (target == null || target.IsDead)
            return;

        // Prevent attacking same team
        if (Army != null && target.Army != null)
        {
            if (Army.TeamMaterial == target.Army.TeamMaterial)
            {
                Debug.Log("Cannot attack unit on same team.");
                return;
            }
        }

        //check if UnitType is null
        if (UnitType == null)
        {
            Debug.LogError("UnitType undefined"); //currently null
        }

        if (audioManager == null)
        {
            Debug.LogError("AudioManager is NULL! No audio will play.");
        }
        else
        {
            Debug.Log("AudioManager exists.");
        }

        if (audioManager == null)
        {
            Debug.LogError("audioManager is NULL! No audio will play.");
        }
        else
        {
            Debug.Log("AudioManager exists.");
        }

        //Play attack SFX
        audioManager.PlaySFX("Knife Stabbing");

        // Spawn blood FX
        if (bloodFXPrefab != null)
        {
            // Instantiate at target's position + offset (e.g., above ground)
            Vector3 spawnPosition = target.transform.position + Vector3.up * 1.0f;
            GameObject bloodFX = Instantiate(bloodFXPrefab, spawnPosition, Quaternion.identity);

            // Optional: Destroy after X seconds to clean up
            Destroy(bloodFX, 2.0f);
        }

        target.TakeDamage(UnitType.Damage, this);

        Debug.Log($"{UnitType.unitTypeName} attacked {target.UnitType.unitTypeName} for {UnitType.Damage} damage.");

        if (target.IsDead)
        {
            target.Die();
            Debug.Log($"{target.UnitType.unitTypeName} has died.");
        }

        if (target.UnitType.CanEscort)
        {
            TakeDamage(target.UnitType.RetaliatoryDamage, target);
            Debug.Log($"{name} took {target.UnitType.RetaliatoryDamage} for attacking a police unit.");
        }
    }

    public virtual void AttackBuilding(BuildingInstance target)
    {
        if (target == null || target.IsDead)
            return;

        // Prevent attacking same team
        if (Army != null && target.Army != null)
        {
            if (Army.TeamMaterial == target.Army.TeamMaterial)
            {
                Debug.Log("Cannot attack unit on same team.");
                return;
            }
        }

        //check if BuildingData is null
        if (target.Data == null)
        {
            Debug.LogError("BuildingData undefined"); 
        }

        if (audioManager == null)
        {
            Debug.LogError("AudioManager is NULL! No audio will play.");
        }
        else
        {
            Debug.Log("AudioManager exists.");
        }

        if (audioManager == null)
        {
            Debug.LogError("audioManager is NULL! No audio will play.");
        }
        else
        {
            Debug.Log("AudioManager exists.");
        }

        //Play attack SFX
        audioManager.PlaySFX("Knife Stabbing");

        // Spawn building destroyed FX
        if (buildingDestroyedFXPrefab != null)
        {
            // Instantiate at target's position + offset (e.g., above ground)
            Vector3 spawnPosition = target.transform.position + Vector3.up * 1.0f;
            GameObject buildingDestroyedFX = Instantiate(buildingDestroyedFXPrefab, spawnPosition, Quaternion.identity);

            // Optional: Destroy after X seconds to clean up
            Destroy(buildingDestroyedFX, 2.0f);
        }

        target.TakeDamage(UnitType.Damage);

        Debug.Log($"{UnitType.unitTypeName} attacked {target.Data.buildingName} for {UnitType.Damage} damage.");

        if (target.IsDead)
        {
            // have the building burn for 2 secs before collapsing?
            target.Die();
            Debug.Log($"{target.Data.buildingName} has collapsed.");
        }

        if (target.Data.IsDefensiveStructure)
        {
            TakeDamage(target.Data.retaliationDamage, this);
            Debug.Log($"{name} took {target.Data.retaliationDamage} for attacking tower.");
        }
    }

    public void Heal(UnitInstance target)
    {
        if (target == null || target.IsDead)
            return;
        // Check if UnitType is null
        if (UnitType == null)
        {
            Debug.LogError("UnitType undefined");
            return;
        }
        // Check if target is same team
        if (Army != null && target.Army != null)
        {
            if (Army.TeamMaterial != target.Army.TeamMaterial)
            {
                Debug.Log("Cannot heal unit on another team.");
                return;
            }
        }
        // Check if healing amount is valid
        if (UnitType.HealingAmount <= 0)
        {
            Debug.LogWarning($"{UnitType.unitTypeName} has no healing ability.");
            return;
        }
        // Play healing SFX
        audioManager.PlaySFX("Level Up Short");
        // Heal the target
        target.CurrentHealth += UnitType.HealingAmount;
        target.CurrentHealth = Mathf.Min(target.CurrentHealth, target.MaxHealth); // Ensure it doesn't exceed max health
        // Update health bar and text
        target.healthBar.UpdateHealthBar(target.CurrentHealth, target.MaxHealth);
        target.healthBar.SetHealthText(target.CurrentHealth, target.MaxHealth);
        Debug.Log($"{UnitType.unitTypeName} healed {target.UnitType.unitTypeName} for {UnitType.HealingAmount} health.");
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;

        audioManager.PlaySFX("Explosion");
        Destroy(gameObject, 2f);

        if (Army != null)
        {
            Army.Units.Remove(this);
            Debug.Log($"{Army.name} has {Army.Units.Count} units remaining.");

            if (Army.ArmyID == 1) // Enemy died -> increment Player's kill count
            {
                var playerArmyGO = GameObject.Find("AM");
                var playerArmy = playerArmyGO?.GetComponent<ArmyData>();

                if (playerArmy != null)
                {
                    playerArmy.unitKillCount++;
                    Debug.Log($"Player kill count: {playerArmy.unitKillCount}");
                    playerArmy.SetKillCountText(playerArmy.unitKillCount);
                }
                else
                {
                    Debug.LogWarning("Player army not found.");
                }
            }
            else if (Army.ArmyID == 0) // Player died -> increment Enemy's kill count
            {
                var enemyArmyGO = GameObject.Find("AM(1)");
                var enemyArmy = enemyArmyGO?.GetComponent<ArmyData>();

                if (enemyArmy != null)
                {
                    enemyArmy.unitKillCount++;
                    Debug.Log($"Enemy kill count: {enemyArmy.unitKillCount}");
                    enemyArmy.SetKillCountText(enemyArmy.unitKillCount);
                }
                else
                {
                    Debug.LogWarning("Enemy army not found.");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Army is null, cannot remove {name}.");
        }
    }


    public void SetPath(List<Vector3> path)
    {
        // Assign to internal path-following logic
        this.path = path; //make currentPath and pathIndex variables
        pathIndex = 0;
    }


}