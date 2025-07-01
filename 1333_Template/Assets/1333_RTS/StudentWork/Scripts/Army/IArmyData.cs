using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IArmyData
{
    int ArmyID { get; }
    bool IsPlayer { get; }
    public IList <UnitInstance> Units { get; }
    public IList <BuildingBase> Buildings { get; }
    string FactionName { get; }

    void Initialize(GridManager gridManager, PathFinder pathfinder, int armyID, Material teamMaterial);
    void InitializeFromData(List<UnitData> data);

    void SpawnUnit(UnitType unitType, Vector3 position, Material teamMaterial);
    void RemoveDeadUnits();
    void AddBuilding(BuildingBase building);
    void RemoveBuilding(BuildingBase building);
}
