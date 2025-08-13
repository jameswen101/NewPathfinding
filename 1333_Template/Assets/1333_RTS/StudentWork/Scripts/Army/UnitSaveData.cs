using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitSaveData
{
    public string unitType;
    public UnitType UnitType
    {
        get { return Resources.Load<UnitType>(unitType); }
        set { unitType = value.name; }
    }
    public Vector3 position;
    public int currentHP;
}

