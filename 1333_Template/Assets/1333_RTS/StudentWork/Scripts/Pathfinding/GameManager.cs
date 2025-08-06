using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private PathFinder pathFinder;
    [SerializeField] private ArmyComposition armyComposition;
    private bool selectingStart = true;
    [SerializeField] private ArmyPathfindingTester armyPathfindingTester;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private EnemyAIManager enemyAIManager;
    private ArmyData playerArmy;
    private ArmyData enemyArmy;

    // Start is called before the first frame update

    private void Awake()
    {
        gridManager.InitializeGrid();
    }

    void Start()
    {
        StartNewGame(2); //starting a new game with 2 players
        //call BGM
        if (audioManager == null)
        {
            Debug.LogError("AudioManager is NULL! No audio will play.");
        }
        else
        {
            Debug.Log("AudioManager exists.");
        }

        audioManager.PlayMusic("Raga Legacy", true);
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            audioManager.StopMusic(); // stop music
            SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
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

            armyData.Initialize(gridManager, pathFinder, i, armyPathfindingTester.armyMaterials[i]);

            if (i == 0)
            {
                playerArmy = armyData;
            }
            else if (i == 1)
            {
                enemyArmy = armyData;
            }

            foreach (var unitComp in armyComposition.entries)
            {
                var startX = Random.Range(0, gridSizeX);
                var startY = Random.Range(0, gridSizeY);
                var position = new Vector3(startX * nodeSize, 0, startY * nodeSize);
                // Presumably you instantiate and place units here later
            }
        }

    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            SceneManager.LoadScene(data.sceneName);
            // pass data to relevant systems after scene load
        }
    }

    public void SaveAll()
    {
        SaveData data = new SaveData();
        data.sceneName = SceneManager.GetActiveScene().name;
        data.waveNumber = enemyAIManager.waveNumber;

        data.playerUnits = UnitSaveUtility.SaveUnits(playerArmy._units);
        data.enemyUnits = UnitSaveUtility.SaveUnits(enemyArmy._units);

        SaveSystem.SaveGame(data);
    }


}
