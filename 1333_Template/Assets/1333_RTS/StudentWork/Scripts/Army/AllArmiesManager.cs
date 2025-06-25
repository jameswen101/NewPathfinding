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

    
