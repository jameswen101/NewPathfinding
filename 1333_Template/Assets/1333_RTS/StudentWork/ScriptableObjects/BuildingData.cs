using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData")]

public class BuildingData: ScriptableObject
{
    public string buildingName;
    public int width;
    public int height;
    public int maxHealth = 100;
    public int currentHealth;
    public TeamArmies army;
    public GameObject buildingPrefab;
    public Sprite buildingIcon;
    public bool IsSolid;
    public int moveCost = -1; //terrain color = rock/danger's color?
    public bool IsDefensiveStructure; //if true, this building can attack enemies
    public int retaliationDamage = 10; //only defensive structures use this- damage dealt to enemies that attack this building

    //public int BuildTime = 5; //time to build this building
    //public int BuildCost = 100; //cost to build this building
}
