using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeasantUnit : UnitInstance
{
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private float attackRange = 1.5f;
    private float lastAttackTime;

    private BuildingInstance castleTarget;
    public BuildingInstance linkedBuilding; //how to assign house in scene to prefab object?

    private void Start()
    {
        castleTarget = GameObject.Find("CastlePrefab").GetComponent<BuildingInstance>();
    }

    void Update()
    {
        if (IsDead || castleTarget == null || castleTarget.IsDead) return;

        float distance = Vector3.Distance(transform.position, castleTarget.transform.position);

        if (distance > attackRange)
        {
            SetDestination(castleTarget.transform.position);
        }
        else if (Time.time >= lastAttackTime + attackInterval)
        {
            AttackBuilding(castleTarget);
            lastAttackTime = Time.time;
        }
        if (linkedBuilding == null || linkedBuilding.IsDead)
        {
            Die(); // Or Destroy(gameObject)
            return;
        }
    }

    public override void AttackBuilding(BuildingInstance target)
    {
        if (target == null) return;
        target.TakeDamage(1); // peasants do 1 damage
        Debug.Log($"{name} peasant hit {target.name}");
    }

}

