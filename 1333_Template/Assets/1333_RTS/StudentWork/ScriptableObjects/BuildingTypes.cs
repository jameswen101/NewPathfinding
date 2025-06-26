using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BuildingTypes", menuName = "ScriptableObjects/BuildingTypes")]

public class BuildingTypes : ScriptableObject
{
    public List<BuildingData> Buildings = new();
}


