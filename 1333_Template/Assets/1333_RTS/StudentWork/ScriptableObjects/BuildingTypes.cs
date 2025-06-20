using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BuildingTypes", menuName = "ScriptableObjects/BuildingTypes")]

public class BuildingTypes : ScriptableObject
{
    public List<BuildingData> Buildings = new();
}

[System.Serializable]
public class BuildingData
{
    public string buildingName;
    public int width;
    public int height;
    public int maxHealth = 100;
    public int currentHealth;
    public TeamArmies army;
    public GameObject buildingPrefab;
    public Sprite buildingIcon;
    public int moveCost = -1; //terrain color = rock/danger's color?
}
