using UnityEngine;


public class BuildingInstance : BuildingBase, IHasHealth
{
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    public HealthBar healthBar; 

    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
    }

    public override void Initialize(BuildingData buildingData, Vector2Int origin)
    {
        BuildingData = buildingData; // this stores it in the field
        Origin = origin;
        CurrentHp = buildingData.currentHealth;
    }
    public void TakeDamage(int incomingDamage)
    {
        CurrentHealth -= incomingDamage;
        healthBar.UpdateHealthBar(CurrentHealth, MaxHealth);
        Debug.Log($"{BuildingData.buildingName} took {incomingDamage} damage.");

        if (CurrentHealth <= 0)
        {
            OnDestroy();
        }
    }
}


