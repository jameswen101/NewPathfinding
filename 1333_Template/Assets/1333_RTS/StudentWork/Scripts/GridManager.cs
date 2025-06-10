using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    public GridSettings GridSettings => gridSettings;

    //private
    public GridNode[,] gridNodes;
    //[SerializeField] private
    public List<GridNode> AllNodes = new();
    public PathFinder pathFinder;
    public GridNode StartNode;
    public GridNode EndNode;

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
        gridNodes = new GridNode[gridSettings.GridSizeX, gridSettings.GridSizeY];
        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                Vector3 worldPos = gridSettings.UseXZPlane
                    ? new Vector3(x, 0, y) * gridSettings.NodeSize
                    : new Vector3(x, y, 0) * gridSettings.NodeSize;

                GridNode node = new GridNode
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
                GridNode node = gridNodes[x, y];
                AllNodes.Add(new GridNode
                {
                    Name = $"Cell_{x}_{y}",
                    WorldPosition = node.WorldPosition,
                    Walkable = node.Walkable,
                    Weight = node.Weight
                });
            }
        }
    }

    public GridNode GetNode(int x, int y)
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
                GridNode node = gridNodes[x,y];
                Gizmos.color = node.Walkable ? Color.green : Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * gridSettings.NodeSize * 0.9f);
            }
        }
        // Draw final path
        List<Vector3> FinalPath = pathFinder.CalculatePath(StartNode, EndNode); 
        if (FinalPath != null && FinalPath.Count > 1) //FinalPath nodes > 1?
        {
            Gizmos.color = finalPathColor;    
            for (int i = 0; i < FinalPath.Count - 1; i++)    
            {        
                Gizmos.DrawLine(FinalPath[i], FinalPath[i + 1]);
                Debug.Log($"Node visited: ({FinalPath[i].x}, {FinalPath[i].z})"); //make sure Debug.Log ends once destination reached
            }
        }
    }

    public GridNode GetNodeFromWorldPosition(Vector3 position)
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

    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GridManager grid = (GridManager)target;
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
