using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
[Serializable]
public class ArmyData : MonoBehaviour, IArmyData
{
    [SerializeField] private string _factionName;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AllArmiesManager allArmiesManager;

    public GridManager GridManager;
    [field: SerializeField]
    public PathFinder Pathfinder { get; private set; }

    [field: SerializeField]
    public int ArmyID { get; set; }
    public bool IsPlayer => ArmyID == 0;
    public string FactionName => _factionName;

    public List<UnitInstance> _units = new();

    public List<BuildingInstance> _buildings = new();

    public IList<UnitInstance> Units => _units;
    public IList<BuildingInstance> Buildings => _buildings;

    public List<BuildingInstance> NonStartingBuildings = new(); //holds all buildings that are not starting buildings, e.g. factories, barracks, etc.

    public Material TeamMaterial { get; set; }

    public List<MachineInstance> _machines = new();

    public IList<MachineInstance> Machines => _machines;

    public HealthBar healthBarPrefab;

    public bool IsPlayerControlled;

    public bool hasAddedBuildings;

    public bool hasAddedUnits;

    public bool AllBuildingsDestroyed => Buildings.Count == 0;
    public bool FinalWaveActivated { get; private set; }

    public event Action<ArmyData> OnFinalWaveStart;

    public int unitKillCount, buildingKillCount;

    public TextMeshProUGUI unitKillCountText;
    private void Awake()
    {
        Debug.Log($"ArmyData Awake with ArmyID={ArmyID}");
        Debug.Log($"AllArmiesManager.Instance == null? {AllArmiesManager.Instance == null}");
        //AllArmiesManager.Instance.RegisterArmy(ArmyID, this);
    }

    private bool isRegistered;

    public void SetKillCountText(int killCount)
    {
        if (unitKillCountText != null)
        {
            unitKillCountText.text = $"Kill count: {killCount}";
            Debug.Log($"Set unitKillCountText to: {killCount}");
        }
        else
        {
            Debug.LogError("unitKillCountText is not assigned in ArmyData.");
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (IsPlayerControlled && hasAddedBuildings && hasAddedUnits && Units.Count == 0 && NonStartingBuildings.Count == 0)
        {
            SceneManager.LoadScene("LoseScreen");
        }
    }

    public void SetTeamMaterial(Material TeamMaterial)
    {
        this.TeamMaterial = TeamMaterial;
    }

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


        HealthBar hb = Instantiate(healthBarPrefab);
        hb.Initialize(instance.transform, instance, mainCamera);
        hb.SetHealthText(instance.CurrentHealth, instance.MaxHealth);
        Debug.Log("Calling HealthTextDebugLog from ArmyData.");
        hb.HealthTextDebugLog();

        // Convert position to grid coords
        Vector2Int gridPos = new Vector2Int(
        Mathf.RoundToInt(position.x),
        Mathf.RoundToInt(position.z)
    );

    // Initialize UnitInstance (make sure you have a matching Initialize overload)
    instance.Initialize(Pathfinder, teamMaterial, GridManager, unitType, gridPos, this, ArmyID);

    if (Units == null)
    {
        Debug.LogError("Units list is null! Cannot add instance.");
        return;
    }

    _units.Add(instance);
        Debug.Log($"Added {instance.name} to Army {ArmyID} Units list.");
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
        building.Initialize(buildingData, origin, GridManager, Pathfinder, this, TeamMaterial); // Fix: Use the instance property 'Pathfinder' instead of the type 'PathFinder'
        building.AssignToArmy(this);

        // Place on grid + store in AAM
        GridManager.PlaceBuilding(building);
        AddBuilding(building);
        Debug.Log($"Added {building.name} to Army {ArmyID} Buildings list.");
    }


    public void AddBuilding(BuildingInstance building)
    {
        if (!Buildings.Contains(building))
        {
            _buildings.Add(building);
            // todo
            building.AssignToArmy(this);
        }
    }

    public void RemoveBuilding(BuildingInstance building)
    {
        if (Buildings.Contains(building))
        {
            _buildings.Remove(building);
            GridManager.RemoveBuilding(building);
            Object.Destroy(building.gameObject);
        }
    }

    public void SpawnMachine(MachineType machineType, Vector3 position, Material teamMaterial)
    {
        if (machineType == null || machineType.machinePrefab == null)
        {
            Debug.LogError("Invalid MachineType or missing prefab.");
            return;
        }

        Vector2Int origin = GridManager.GetNodeFromWorldPosition(position).Coordinates;

        if (!GridManager.CanPlaceMachine(machineType, origin))
        {
            Debug.Log("Invalid building placement");
            return;
        }

        // Snap to grid
        Vector3 worldCenter = GridManager.GetNode(origin.x, origin.y).WorldPosition;

        GameObject go = GameObject.Instantiate(machineType.machinePrefab, position, Quaternion.identity);
        if (go == null)
        {
            Debug.LogError("Prefab instantiation failed.");
            return;
        }

        MachineInstance instance = go.GetComponent<MachineInstance>();
        if (instance == null)
        {
            Debug.LogError($"Spawned prefab '{go.name}' does not have a MachineInstance component!");
            return;
        }

        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.z)
        );

        instance.Initialize(Pathfinder, teamMaterial, GridManager, machineType, gridPos);

        _machines.Add(instance);
        Debug.Log($"Added {instance.name} to Army {ArmyID} Machines list.");
    }

    public void RemoveDestroyedMachines()
    {
        for (int i = _machines.Count - 1; i >= 0; i--)
        {
            var machine = _machines[i];
            if (machine == null || machine.IsDestroyed)
            {
                _machines.RemoveAt(i);
            }
        }
    }

    public void Dispose()
    {
        AllArmiesManager.Instance?.UnregisterArmy(ArmyID);
    }

    public void CheckFinalWave()
    {
        if (AllBuildingsDestroyed && !FinalWaveActivated)
        {
            FinalWaveActivated = true;
            Debug.Log($"Final wave activated for Army {ArmyID}");
            OnFinalWaveStart?.Invoke(this);
        }
    }


    public void SpawnUnit(UnitData data)
    {
        throw new NotImplementedException();
    }
}
