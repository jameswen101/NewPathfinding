using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitSaveUtility
{
    public static List<UnitSaveData> SaveUnits(List<UnitInstance> units)
    {
        var saved = new List<UnitSaveData>();
        foreach (var u in units)
        {
            if (u == null || u.IsDead) continue;
            saved.Add(new UnitSaveData
            {
                unitType = u.UnitType.name,
                position = u.transform.position,
                currentHP = (int)u.CurrentHealth
            });
        }
        return saved;
    }
}

