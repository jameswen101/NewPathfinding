using System;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        ArmyData[] armiesInScene = FindObjectsOfType<ArmyData>();
        foreach (ArmyData army in armiesInScene)
        {
            RegisterArmy(army.ArmyID, army);
            Debug.Log($"Registered army with ID {army.ArmyID}");
        }
    }


    public void RegisterArmy(int armyId, ArmyData army)
    {
        if (_armies.ContainsKey(armyId))
        {
            Debug.LogWarning($"Army with ID {armyId} is already registered.");
            return;
        }
        _armies.Add(armyId, army);
        Debug.Log($"Registered army {armyId}. Total armies: {_armies.Count}");
    }

    public void UnregisterArmy(int armyId)
        {
            _armies.Remove(armyId);
        }

    public bool TryGetArmy(int armyId, out ArmyData army)
    {
        bool found = _armies.TryGetValue(armyId, out army);
        Debug.Log($"TryGetArmy({armyId}) found={found}, total armies: {_armies.Count}");
        return found;
    }


}


