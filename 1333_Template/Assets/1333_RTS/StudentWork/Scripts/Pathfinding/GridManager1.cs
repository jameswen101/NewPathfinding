using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridManager1 : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    public GridSettings GridSettings => gridSettings;

    //private
    public GridNode1[,] gridNodes; //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    //[SerializeField] private
    public List<GridNode1> AllNodes = new(); //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    public PathFinder1 pathFinder;
    public GridNode1 StartNode; //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    public GridNode1 EndNode; //currently MonoBehavior class gridnode1; change back to struct gridnode if not working

    public bool IsInitialized { get; private set; } = false;

    [SerializeField] private Color finalPathColor = Color.cyan;

    // Start is called before the first frame update
    void Start()
    {
        StartNode = gridNodes[0, 0]; 
        EndNode = gridNodes[9, 4];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeGrid()
    {
        gridNodes = new GridNode1[gridSettings.GridSizeX, gridSettings.GridSizeY]; //currently class gridnode1; change back to struct gridnode if not working
        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                Vector3 worldPos = gridSettings.UseXZPlane
                    ? new Vector3(x, 0, y) * gridSettings.NodeSize
                    : new Vector3(x, y, 0) * gridSettings.NodeSize;

                GridNode1 node = new GridNode1
                {
                    Name = $"Cell_{(x + gridSettings.GridSizeX) * x + y}",
                    WorldPosition = worldPos,
                    Walkable = true,
                    Weight = 1
                };
                gridNodes[x, y] = node;
            }
        }
        IsInitialized = true;
        DoSomethingOnEachNode(OnValidate);
        PopulateDebugList();
    }

    private void PopulateDebugList()
    {
        AllNodes.Clear();
        for (int x = 0;x < gridSettings.GridSizeX;x++)
        {
            for (int y = 0;y < gridSettings.GridSizeY;y++)
            {
                GridNode1 node = gridNodes[x, y]; //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
                AllNodes.Add(new GridNode1
                {
                    Name = $"Cell_{x}_{y}",
                    WorldPosition = node.WorldPosition,
                    Walkable = node.Walkable,
                    Weight = node.Weight
                }); //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
            }
        }
    }

    public GridNode1 GetNode(int x, int y) //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    {
        if (x < 0 || x>= gridSettings.GridSizeX || y < 0 || y>= gridSettings.GridSizeY)
            throw new System.IndexOutOfRangeException("Grid node indices out of range.");
        return gridNodes[x, y];
    }
    public void SetWalkable(int x, int y, bool walkable)
    {
        gridNodes[x,y].Walkable = walkable;
    }
    private void OnDrawGizmos()
    {
        if (gridNodes == null || gridSettings == null) return;
        Gizmos.color = Color.green;
        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                GridNode1 node = gridNodes[x,y];
                Gizmos.color = node.Walkable ? Color.green : Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * gridSettings.NodeSize * 0.9f);
            }
        }
        // Draw final path
        List<GridNode1> FinalPath = pathFinder.CalculatePath(StartNode, EndNode); //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
        if (FinalPath != null && FinalPath.Count > 1) //FinalPath nodes > 1?
        {
            Gizmos.color = finalPathColor;
            for (int i = 0; i < FinalPath.Count - 1; i++)
            {
                Gizmos.DrawLine(FinalPath[i].WorldPosition, FinalPath[i + 1].WorldPosition);
                Debug.Log($"Node visited: {FinalPath[i].WorldPosition.x}, {FinalPath[i].WorldPosition.z}");
            }

        }
    }

    public GridNode1 GetNodeFromWorldPosition(Vector3 position) //currently MonoBehavior class gridnode1; change back to struct gridnode if not working
    {
        int x = GridSettings.UseXZPlane ? Mathf.RoundToInt(f: position.x / GridSettings.NodeSize) : Mathf.RoundToInt(f: position.x / GridSettings.NodeSize);
        int y = GridSettings.UseXZPlane ? Mathf.RoundToInt(f: position.z / GridSettings.NodeSize) : Mathf.RoundToInt(f: position.y / GridSettings.NodeSize);
        x = Mathf.Clamp(x, 0, GridSettings.GridSizeX - 1);
        y = Mathf.Clamp(y, 0, GridSettings.GridSizeY - 1);
        return GetNode(x, y);
    }

    private int count;
    private void OnValidate()
    {
        count++;
        Debug.Log("Validated " + count);
    }


    private void DoSomethingOnEachNode(System.Action thingToDo)
    {
        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for(int y = 0;y < gridSettings.GridSizeY;y++)
            {
                thingToDo?.Invoke();
            }
        }
    }

    [CustomEditor(typeof(GridManager1))]
    public class GridManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GridManager1 grid = (GridManager1)target;
            if (grid.IsInitialized)
            {
                if (GUILayout.Button("Refresh Grid Debug View"))
                {
                    grid.PopulateDebugList();
                }
            }
        }
    }
}
