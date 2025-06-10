using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    [SerializeField] private GridManager1 gridManager;
    [SerializeField] private UnitManager1 unitManager;
    // Start is called before the first frame update

    private void Awake()
    {
        gridManager.InitializeGrid();
        unitManager.SpawnDummyUnit();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
