using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


    public interface IArmyData
    {
        int ArmyID { get; }
        bool IsPlayer { get; }
        IReadOnlyList<UnitInstance> Units { get; }
        IReadOnlyList<BuildingBase> Buildings { get; }
        string FactionName { get; }

        void Initialize(GridManager gridManager, PathFinder pathfinder, int armyID, string factionName);
        void InitializeFromData(List<UnitData> data);

        void SpawnUnit(UnitData data);
        void RemoveDeadUnits();
        void AddBuilding(BuildingBase building);
        void RemoveBuilding(BuildingBase building);
    }

    public class AllArmiesManager : MonoBehaviour
    {
        [SerializeField] private PathFinder pathfinder;
        private readonly Dictionary<int, ArmyData> _armies = new();
        public IReadOnlyCollection<ArmyData> AllArmies => _armies.Values;
        public static AllArmiesManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple ArmyManager instances found. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void RegisterArmy(int armyId, ArmyData army)
        {
            if (_armies.ContainsKey(armyId))
            {
                Debug.LogWarning($"Army with ID {armyId} is already registered.");
                return;
            }

            _armies.Add(armyId, army);
        }

        public void UnregisterArmy(int armyId)
        {
            _armies.Remove(armyId);
        }

        public bool TryGetArmy(int armyId, out ArmyData army)
        {
            return _armies.TryGetValue(armyId, out army);
        }
    }

    [Serializable]
    public class ArmyData : IArmyData
    {
        [SerializeField] private string _factionName;
        [SerializeField] private List<UnitInstance> _units = new();
        [SerializeField] private List<BuildingBase> _buildings = new();

        public GridManager GridManager { get; private set; }
        public PathFinder Pathfinder { get; private set; }

        public int ArmyID { get; private set; }
        public bool IsPlayer => ArmyID == 0;
        public string FactionName => _factionName;

        public IReadOnlyList<UnitInstance> Units => _units;
        public IReadOnlyList<BuildingBase> Buildings => _buildings;


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

        public void SpawnUnit(UnitData data)
        {
            if (data.UnitType == null || data.UnitType.unitPrefab == null)
            {
                Debug.LogError("Invalid UnitType or missing prefab");
                return;
            }

            UnitInstance instance = Instantiate(data.UnitType.unitPrefab, data.Position, Quaternion.identity);
            instance.Initialize(Pathfinder, data.TeamMaterial); //get the instance of the unit + the pathfinder it will use to find path + material
            _units.Add(instance);
        }



        public void RemoveDeadUnits()
        {
            _units.RemoveAll(unit => unit == null || unit.IsDead);
        }





        private void ClearUnits()
        {
            foreach (var unit in _units)
                if (unit != null)
                    Object.Destroy(unit.gameObject);
            _units.Clear();
        }

        public void SpawnBuilding(BuildingTypes type, Vector3 worldPosition) 
        {
            var buildingData = type.Buildings[0]; // for example, first building in the list
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
            //instantiate building gameobject (prefab)
            BuildingInstance building = Instantiate(buildingData.buildingPrefab, worldPosition, Quaternion.identity)
                    .GetComponent<BuildingBase>();
            //initialize + register building
            building.Initialize(buildingData, origin);
            building.AssignToArmy(this);
            //place on grid + store in AAM
            GridManager.PlaceBuilding(building);
            AddBuilding(building);
        }

        public void AddBuilding(BuildingBase building)
        {
            if (!_buildings.Contains(building))
            {
                _buildings.Add(building);
                // todo
                building.AssignToArmy(this);
            }
        }

        public void RemoveBuilding(BuildingBase building)
        {
            if (_buildings.Contains(building))
            {
                _buildings.Remove(building);
                GridManager.RemoveBuilding(building);
                Object.Destroy(building.gameObject);
            }
        }

        public void Dispose()
        {
            AllArmiesManager.Instance?.UnregisterArmy(ArmyID);
        }
    }

    /// <summary>
    ///     For later save / load
    /// </summary>
    [Serializable]
    public class UnitData
    {
        public UnitType UnitType;
        public Vector3 Position;
        public int Health;
        public int ArmyId;
        public Material TeamMaterial;
    }
