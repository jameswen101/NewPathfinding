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
