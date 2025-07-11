using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] public ArmyData Army { get; set; }

    [SerializeField] public int ArmyID { get; set; }

    [SerializeField] private AudioManager audioManager; // Reference to AudioManager for playing SFX

    private float walkingSFXCooldown = 0f;

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
        target.TakeDamage(UnitType.Damage);

        Debug.Log($"{UnitType.unitTypeName} attacked {target.UnitType.unitTypeName} for {UnitType.Damage} damage.");

        if (target.IsDead)
        {
            target.Die();
            Debug.Log($"{target.UnitType.unitTypeName} has died.");
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
    }

    public void SetPath(List<Vector3> path)
    {
        // Assign to internal path-following logic
        this.path = path; //make currentPath and pathIndex variables
        pathIndex = 0;
    }


}