using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
[Serializable]
public class ArmyData : MonoBehaviour, IArmyData
{
    [SerializeField] private string _factionName;

    public GridManager GridManager;
    [field: SerializeField]
    public PathFinder Pathfinder { get; private set; }

    [field: SerializeField]
    public int ArmyID { get; set; }
    public bool IsPlayer => ArmyID == 0;
    public string FactionName => _factionName;

    [SerializeField]
    private List<UnitInstance> _units = new List<UnitInstance>();

    [SerializeField]
    private List<BuildingBase> _buildings = new List<BuildingBase>();

    public IList<UnitInstance> Units => _units;
    public IList<BuildingBase> Buildings => _buildings;


    public void Initialize(GridManager gridManager, PathFinder pathfinder, int armyID, string factionName)
    {
        GridManager = gridManager;
        Pathfinder = pathfinder;
        ArmyID = armyID;
        _factionName = factionName;

        AllArmiesManager.Instance.RegisterArmy(armyID, this);
    }

    public void InitializeFromData(List<UnitData> unitDataList)
    {
        ClearUnits();
        foreach (var unitData in unitDataList) SpawnUnit(unitData);
    }

    public void SpawnUnit(UnitData data) //UnitData parameter is currently blank?
    {
        if (data == null) 
        {
            Debug.LogError("UnitData parameter doesn't exist");
            return;
        }

        if (data.UnitType == null || data.UnitType.unitPrefab == null)
        {
            Debug.LogError("Invalid UnitType or missing prefab");
            return;
        }

        GameObject go = GameObject.Instantiate(data.UnitType.unitPrefab, data.Position, Quaternion.identity);
        UnitInstance instance = go.GetComponent<UnitInstance>(); //unit expects itself to have a unitinstance on it
        //pass the GridM from this class to UI here

        if (go == null)
        {
            Debug.LogError("Prefab instantiation failed.");
            return;
        }

        if (instance == null)
        {
            Debug.LogError($"Spawned prefab '{go.name}' does not have a UnitInstance component!");
            return;
        }

        instance.Initialize(Pathfinder, data.TeamMaterial, GridManager);

        if (Units == null)
        {
            Debug.LogError("Units list is null! Cannot add instance.");
            return;
        }

        Units.Add(instance); 
    }

    public void RemoveDeadUnits()
    {
        for (int i = Units.Count - 1; i >= 0; i--)
        {
            var unit = Units[i];
            if (unit == null || unit.IsDead)
            {
                Units.RemoveAt(i);
            }
        }
    }

    private void ClearUnits()
    {
        foreach (var unit in Units)
            if (unit != null)
                Object.Destroy(unit.gameObject);
        Units.Clear();
    }

    public void SpawnBuilding(BuildingTypes type, Vector3 worldPosition)
    {
        var buildingData = type.Buildings[0]; // Example: first building in the list
        if (buildingData == null || buildingData.buildingPrefab == null)
        {
            Debug.LogError("Invalid building data or prefab");
            return;
        }

        Vector2Int origin = GridManager.GetNodeFromWorldPosition(worldPosition).Coordinates;

        if (!GridManager.CanPlaceBuilding(type, origin))
        {
            Debug.Log("Invalid building placement");
            return;
        }

        // Snap to grid
        Vector3 worldCenter = GridManager.GetNode(origin.x, origin.y).WorldPosition;

        // Instantiate building GameObject (prefab) and get BuildingInstance component
        GameObject go = GameObject.Instantiate(buildingData.buildingPrefab, worldCenter, Quaternion.identity);
        BuildingInstance building = go.GetComponent<BuildingInstance>();

        if (building == null)
        {
            Debug.LogError("Prefab is missing BuildingInstance component");
            return;
        }

        // Initialize + register building
        building.Initialize(buildingData, origin);
        building.AssignToArmy(this);

        // Place on grid + store in AAM
        GridManager.PlaceBuilding(building);
        AddBuilding(building);
    }


    public void AddBuilding(BuildingBase building)
    {
        if (!Buildings.Contains(building))
        {
            Buildings.Add(building);
            // todo
            building.AssignToArmy(this);
        }
    }

    public void RemoveBuilding(BuildingBase building)
    {
        if (Buildings.Contains(building))
        {
            Buildings.Remove(building);
            GridManager.RemoveBuilding(building);
            Object.Destroy(building.gameObject);
        }
    }

    public void Dispose()
    {
        AllArmiesManager.Instance?.UnregisterArmy(ArmyID);
    }
}
