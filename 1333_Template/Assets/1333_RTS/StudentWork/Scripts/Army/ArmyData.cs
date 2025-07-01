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

    public Material TeamMaterial { get; private set; }



    private void Awake()
    {
        Debug.Log($"ArmyData Awake with ArmyID={ArmyID}");
        Debug.Log($"AllArmiesManager.Instance == null? {AllArmiesManager.Instance == null}");
        //AllArmiesManager.Instance.RegisterArmy(ArmyID, this);
    }

    private bool isRegistered;

    public void Initialize(GridManager gridManager, PathFinder pathfinder, int armyID, Material teamMaterial)
    {
        GridManager = gridManager;
        Pathfinder = pathfinder;
        ArmyID = armyID;
        TeamMaterial = teamMaterial;

        AllArmiesManager.Instance.RegisterArmy(armyID, this);
        isRegistered = true;
    }


    public void InitializeFromData(List<UnitData> unitDataList)
    {
        ClearUnits();
        foreach (var unitData in unitDataList) SpawnUnit(unitData);
    }

    public void SpawnUnit(UnitType unitType, Vector3 position, Material teamMaterial)
{
    if (unitType == null || unitType.unitPrefab == null)
    {
        Debug.LogError("Invalid UnitType or missing prefab.");
        return;
    }

    GameObject go = GameObject.Instantiate(unitType.unitPrefab, position, Quaternion.identity);
    if (go == null)
    {
        Debug.LogError("Prefab instantiation failed.");
        return;
    }

    UnitInstance instance = go.GetComponent<UnitInstance>();
    if (instance == null)
    {
        Debug.LogError($"Spawned prefab '{go.name}' does not have a UnitInstance component!");
        return;
    }

    // Convert position to grid coords
    Vector2Int gridPos = new Vector2Int(
        Mathf.RoundToInt(position.x),
        Mathf.RoundToInt(position.z)
    );

    // Initialize UnitInstance (make sure you have a matching Initialize overload)
    instance.Initialize(Pathfinder, teamMaterial, GridManager, unitType, gridPos);

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

    public void SpawnUnit(UnitData data)
    {
        throw new NotImplementedException();
    }
}
