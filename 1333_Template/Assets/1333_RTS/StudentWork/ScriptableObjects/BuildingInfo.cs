using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BuildingInfo", menuName = "BuildingInfo")]

public class BuildingInfo : ScriptableObject
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private TeamArmies army;
    public GameObject buildingPrefab;

    public int Width => width;
    public int Height => height;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public TeamArmies Army => army;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
