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
    public bool IsMoving => moving;
    public bool IsDead;
    public UnitType UnitType { get; private set; }
    public Vector2Int OriginPoint { get; private set; }

    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void Initialize(PathFinder assignedPathfinder, Material teamMaterial, GridManager gridManager, UnitType unitType, Vector2Int OriginPoint)
    {
        pathfinder = assignedPathfinder;
        gridM = gridManager;
        UnitType = unitType;
        this.OriginPoint = OriginPoint;

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
        if (!moving || path == null || pathIndex >= path.Count)
            return;

        Vector3 nextPoint = path[pathIndex];
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextPoint, step);

        if (Vector3.Distance(transform.position, nextPoint) < 0.05f)
        {
            pathIndex++;
            if (pathIndex >= path.Count)
            {
                moving = false;
                // You can trigger idle animations here if needed
            }
        }
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

        target.TakeDamage(UnitType.Damage);

        Debug.Log($"{UnitType.unitTypeName} attacked {target.UnitType.unitTypeName} for {UnitType.Damage} damage.");
    }


    public void TakeDamage(int incomingDamage)
    {
        int mitigated = Mathf.Max(incomingDamage - UnitType.Defence, 1);
        CurrentHealth -= mitigated;
        Debug.Log($"{UnitType.unitTypeName} took {mitigated} damage (after {UnitType.Defence} defence).");

        if (CurrentHealth <= 0)
        {
            IsDead = true;
        }
    }

}