using UnityEngine;


public class BuildingInstance : BuildingBase
{
    public override void Initialize(BuildingData buildingData, Vector2Int origin)
    {
        BuildingData = buildingData; // this stores it in the field
        Origin = origin;
        CurrentHp = buildingData.currentHealth;
    }
}


