using UnityEngine;


public class BuildingInstance : BuildingBase, IHasHealth
{
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    public override void Initialize(BuildingData buildingData, Vector2Int origin)
    {
        BuildingData = buildingData; // this stores it in the field
        Origin = origin;
        CurrentHp = buildingData.currentHealth;
    }
}


