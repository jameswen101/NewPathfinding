using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UnitData", menuName = "UnitData")]

public class UnitData: ScriptableObject
{
    public UnitType UnitType;
    public Vector3 Position;
    public int Health;
    public int ArmyId;
    public Material TeamMaterial;
    // add UnitInstance object here?
}
