using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingSaveUtility
{
    public static List<BuildingSaveData> SaveBuildings(List<BuildingInstance> buildings)
    {
        var saved = new List<BuildingSaveData>();
        foreach (var b in buildings)
        {
            if (b == null || b.IsDead) continue;
            saved.Add(new BuildingSaveData
            {
                buildingName = b.Data.name,
                position = b.transform.position,
                currentHP = (int)b.CurrentHealth
            });
        }
        return saved;
    }
}
