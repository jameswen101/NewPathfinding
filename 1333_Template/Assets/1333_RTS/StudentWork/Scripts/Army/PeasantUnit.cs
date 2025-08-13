using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeasantUnit : UnitInstance
{
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private float attackRange = 1.5f;
    private float lastAttackTime;

    [SerializeField] private BuildingInstance castleTarget;
    public BuildingInstance linkedBuilding; //how to assign house in scene to prefab object?
    private bool hasSetHouse = false;

    private void Start()
    {
        castleTarget = GameObject.Find("CastlePrefab").GetComponent<BuildingInstance>();
        if (castleTarget == null)
        {
            Debug.LogError("Castle target not found in the scene!");
            return;
        }
        if (linkedBuilding == null && Army.Houses.Count > 0) //why repeating null references at this line when armyData is not null?
        {
            linkedBuilding = Army.Houses[^1]; // Get the last house in the list
            hasSetHouse = true; // Set the flag to true since we are now using a house
            Debug.Log($"{name} is now linked to the last house in the list: {Army.Houses[^1].name}");
        }

    }

    void Update()
    {
        if (IsDead || castleTarget == null || castleTarget.IsDead) return;

        if (Army.Units.Count >= 7 && Army.Buildings.Count >= 4)
        {
            float distance = Vector3.Distance(transform.position, castleTarget.transform.position);

            if (distance > attackRange) //if not right next to the castle
            {
                // If not in range, move towards the castle
                SetDestination(castleTarget.transform.position); //currently null
            }
            else if (Time.time >= lastAttackTime + attackInterval)
            {
                AttackBuilding(castleTarget);
                lastAttackTime = Time.time;
            }
            // Good: only read IsDead if linkedBuilding is not null
            if (hasSetHouse && (linkedBuilding == null || linkedBuilding.IsDead))
            {
                Die();
                return;
            }
        }   
    }

    public override void AttackBuilding(BuildingInstance target)
    {
        if (target == null) return;
        target.TakeDamage(1); // peasants do 1 damage
        Debug.Log($"{name} peasant hit {target.name}");
    }

    public void SetHouse(BuildingInstance house)
    {
        linkedBuilding = house;
        Debug.Log($"{name} linked to house {linkedBuilding?.name}");
        hasSetHouse = true;
    }

}

