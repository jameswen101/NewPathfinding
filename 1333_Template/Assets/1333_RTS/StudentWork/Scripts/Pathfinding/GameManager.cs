using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private PathFinder pathFinder;
    [SerializeField] private ArmyComposition armyComposition;
    private bool selectingStart = true;
    [SerializeField] private ArmyPathfindingTester armyPathfindingTester;
    // Start is called before the first frame update

    private void Awake()
    {
        gridManager.InitializeGrid();
        unitManager.SpawnDummyUnit();
    }

    void Start()
    {
        StartNewGame(2); //starting a new game with 2 players
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ShuffleGridAndPath();
        }
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 worldPos = hit.point;
                GridNode clickedNode = gridManager.GetNodeFromWorldPosition(worldPos);

                if (clickedNode.Walkable)
                {
                    if (selectingStart)
                    {
                        gridManager.SetStartNode(clickedNode);
                    }
                    else
                    {
                        if (clickedNode.WorldPosition != gridManager.StartNode.WorldPosition)
                            gridManager.SetEndNode(clickedNode);
                    }

                    selectingStart = !selectingStart; // Toggle between start/end
                }
            }
        }
    }

    void ShuffleGridAndPath()
    {
        gridManager.InitializeGrid(); // This already handles random terrain + assigns nodes
                                      // If you need to assign Start/End node explicitly:
        gridManager.AssignRandomStartAndEnd();
    }

    public void StartNewGame(int armyCount)
    {
        Debug.Log($"[GameManager] Starting new game with {armyCount} armies.");

        var gridSizeX = gridManager.GridSettings.GridSizeX;
        var gridSizeY = gridManager.GridSettings.GridSizeY;
        var nodeSize = gridManager.GridSettings.NodeSize;

        for (var i = 0; i < armyCount; i++)
        {
            var armyData = gameObject.AddComponent<ArmyData>();

            armyData.Initialize(gridManager, pathFinder, i, armyPathfindingTester.armyMaterials[i]); //setting up the army

            // Example spawn for now: one unit in each army
            foreach (var unitComp in armyComposition.entries)
            {
                var startX = Random.Range(0, gridSizeX);
                var startY = Random.Range(0, gridSizeY);

                var position = new Vector3(startX * nodeSize, 0, startY * nodeSize);
                //var unitData = new UnitData
                //{ UnitType = unitComp.UnitConfig, Position = position, Health = unitComp.UnitConfig.MaxHp, ArmyId = i };

                //armyData.SpawnUnit(unitData);
            }
        }
    }
}
