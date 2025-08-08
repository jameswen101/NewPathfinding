using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class BuildingSaveData 
{
    public string buildingName;
    public BuildingData buildingData
    {
        get { return Resources.Load<BuildingData>(buildingName); }
        set { buildingName = value.name; }
    }
    public Vector3 position;
    public int currentHP;
}
