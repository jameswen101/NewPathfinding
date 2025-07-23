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

    [SerializeField] private ArmyData army;

    public ArmyData Army
    {
        get => army;
        set => army = value;
    }


    [SerializeField] public int ArmyID { get; set; }

    [SerializeField] private AudioManager audioManager; // Reference to AudioManager for playing SFX

    private float walkingSFXCooldown = 0f;

    //public TextMeshPro healthText;

    [SerializeField] private GameObject healthBarPrefab;

    [SerializeField] private GameObject bloodFXPrefab;


    //public void UpdateHealthText()
    //{
    //    healthText.text = $"{CurrentHealth} / {MaxHealth}";
    //}


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
        //if (healthText == null)
        //{
        //    healthText = GetComponentInChildren<TextMeshPro>();
        //    if (healthText == null)
        //    {
        //        Debug.LogWarning($"Unit {name} has no HealthText assigned at Awake.");
        //    }
        //}
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
        Debug.Log($"ArmyData is {(armyData == null ? "NULL" : "OK")}, ArmyID = {ArmyID}");

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
        // Instantiate as child of the unit
        GameObject healthBarObj = Instantiate(
            healthBarPrefab,
            transform // parent transform
        );

        // Optional: set local offset
        healthBarObj.transform.localPosition = new Vector3(0, 2f, 2f);

        // Get HealthBar component
        HealthBar healthBar = healthBarObj.GetComponent<HealthBar>();

        // Initialize it
        healthBar.Initialize(this.transform, this, Camera.main, new Vector3(0, 2f, 0));
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

        DrawPathLine();
    }

    public void SetDestination(GridNode node)
    {
        SetDestination(node.WorldPosition);
    }

    public override void MoveTo(GridNode node)
    {
        SetDestination(node);
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

        target.TakeDamage(UnitType.Damage);

        Debug.Log($"{UnitType.unitTypeName} attacked {target.UnitType.unitTypeName} for {UnitType.Damage} damage.");

        if (target.IsDead)
        {
            target.Die();
            Debug.Log($"{target.UnitType.unitTypeName} has died.");
        }
    }

    public void AttackBuilding(BuildingInstance target)
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
            Debug.LogError("BuildingData undefined"); //currently null
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

        target.TakeDamage(UnitType.Damage);

        Debug.Log($"{UnitType.unitTypeName} attacked {target.Data.buildingName} for {UnitType.Damage} damage.");

        if (target.IsDead)
        {
            target.Die();
            Debug.Log($"{target.Data.buildingName} has collapsed.");
        }
    }

    public void Die()
    {
        IsDead = true;
        //animator.SetTrigger("Die");
        // Play explosion SFX
        audioManager.PlaySFX("Explosion");
        // play particle FX
        // Disable the unit after a short delay
        Destroy(gameObject, 2f);
        if (Army != null)
        {
            Army.Units.Remove(this);
            Debug.Log($"{Army.name} has {Army.Units.Count} units remaining.");
            if (!Army.IsPlayerControlled && Army.Units.Count == 0)
            {
                Debug.Log("Player Wins!");
                // You can trigger a victory UI here:
                SceneManager.LoadScene("WinScreen");
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