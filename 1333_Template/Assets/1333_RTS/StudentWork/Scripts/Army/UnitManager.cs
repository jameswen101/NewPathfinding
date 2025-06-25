using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GridTest gridTest;
    public GridNode startNode;
    public GridNode endNode;
    public GameObject dummyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    /*
     * public GridNode spawnNode;
    public GameObject dummyPrefab;
    public GridManager gridManager;
    private Dictionary<int, ArmyManager> armyManager;
    public ArmyManager PlayerArmy => armyManager[0];
    public Vector3 spawnLocation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnDummyUnit(Transform parent)
    {
        if (!gridManager.IsInitialized)
        {
            Debug.LogError("Grid not initialized!");
            return;
        }
        //spawn a cube, just need a way to know where the start is + where the end is
        int randomX = Random.Range(0, gridManager.GridSettings.GridSizeX);
        int randomY = Random.Range(0, gridManager.GridSettings.GridSizeY);
        spawnNode = gridManager.GetNode(randomX, randomY);
        Debug.Log($"Dummy unit spawned at ({randomX}, {randomY}) - World Position: {spawnNode.WorldPosition}");
        //instantiate dummy unit prefab here in future
        GameObject dummy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        dummy.transform.position = spawnNode.WorldPosition;
        //GameObject unit = Instantiate(dummyPrefab, startNode.WorldPosition, Quaternion.identity); //add one of those NPC assets here?
        //UnitController controller = unit.GetComponent<UnitController>();
        //controller.MoveTo(endNode); // start pathfinding
    }
     */
    void Update()
    {
        
    }

    public void SpawnDummyUnit()
    {
        //spawn a cube, just need a way to know where the start is + where the end is
        GameObject unit = Instantiate(dummyPrefab, startNode.WorldPosition, Quaternion.identity); //add one of those NPC assets here?
        //UnitController controller = unit.GetComponent<UnitController>();
        //controller.MoveTo(endNode); // start pathfinding
    }
}
