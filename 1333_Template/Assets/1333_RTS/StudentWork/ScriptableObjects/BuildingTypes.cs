using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BuildingTypes", menuName = "ScriptableObjects/BuildingTypes")]

public class BuildingTypes : ScriptableObject
{
    public List<BuildingData> Buildings = new();
    //a bool variable to check if there are any new units that need to be added to UI
    public bool HasNewBuildings = false;

    public void AddBuilding(BuildingData building)
    {
        if (!Buildings.Contains(building))
        {
            Buildings.Add(building);
            HasNewBuildings = true;
        }
    }
}


