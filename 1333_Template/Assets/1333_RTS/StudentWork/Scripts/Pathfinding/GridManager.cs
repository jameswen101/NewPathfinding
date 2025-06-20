using RTS_1333;
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
    [SerializeField] private List<TerrainType> terrainTypes = new List<TerrainType>();
    private readonly Dictionary<Vector2Int, BuildingInstance> _buildingOccupancy = new(); /// Tracks buildings that occupy grid nodes.

    public bool IsInitialized { get; private set; } = false;

    [SerializeField] private Color finalPathColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();

        StartNode = GetRandomWalkableNode();

        // Ensure EndNode is different from StartNode
        do
        {
            EndNode = GetRandomWalkableNode();
        } while (EndNode.WorldPosition == StartNode.WorldPosition);

        Debug.Log($"StartNode: {StartNode.Name}, EndNode: {EndNode.Name}");
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

                // Assign a random terrain
                if (terrainTypes == null || terrainTypes.Count == 0)
                {
                    Debug.LogWarning("No terrain types assigned!");
                    return;
                }

                TerrainType terrain = terrainTypes[Random.Range(0, terrainTypes.Count)];

                gridNodes[x, y] = new GridNode
                {
                    Name = $"Cell_{x}_{y}",
                    WorldPosition = worldPos,
                    Walkable = terrain.walkable,
                    Weight = terrain.movementCost,
                    TerrainType = terrain,
                    GizmoColor = terrain.gizmoColor
                };
            }
        }

        IsInitialized = true;
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
                Gizmos.color = node.GizmoColor; // use terrain-specific color
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

    private GridNode GetRandomWalkableNode()
    {
        for (int i = 0; i < 1000; i++) // max attempts
        {
            int x = Random.Range(0, gridSettings.GridSizeX);
            int y = Random.Range(0, gridSettings.GridSizeY);
            GridNode node = gridNodes[x, y];

            if (node.Walkable)
                return node;
        }

        // If no walkable node found after 1000 attempts, fallback
        Debug.LogWarning("No walkable node found!");
        return gridNodes[0, 0]; // fallback or throw
    }

    public void AssignRandomStartAndEnd()
    {
        StartNode = GetRandomWalkableNode();

        do
        {
            EndNode = GetRandomWalkableNode();
        } while (EndNode.WorldPosition == StartNode.WorldPosition);
    }

    public void SetStartNode(GridNode node)
    {
        StartNode = node;
        Debug.Log($"StartNode set to: {node.Name}");
    }

    public void SetEndNode(GridNode node)
    {
        EndNode = node;
        Debug.Log($"EndNode set to: {node.Name}");
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

    public bool CanPlaceBuilding(BuildingTypes type, Vector2Int origin)
    {
        for (int dx = 0; dx < type.Buildings[dx].width; dx++)
        {
            for (int dy = 0; dy < type.Buildings[dy].height; dy++)
            {
                Vector2Int pos = origin + new Vector2Int(dx, dy);
                if (!IsValidCoordinate(pos.x, pos.y) || _buildingOccupancy.ContainsKey(pos))
                    return false;
            }
        }
        return true;
    }

    public void PlaceBuilding(BuildingInstance instance)
    {
        Vector2Int origin = instance.OriginPoint;
        BuildingTypes type = instance.Type;
        for (int dx = 0; dx < type.Buildings[dx].width; dx++)
        {
            for (int dy = 0; dy < type.Buildings[dy].height; dy++)
            {
                Vector2Int pos = origin + new Vector2Int(dx, dy);
                _buildingOccupancy[pos] = instance;
                if (type.IsSolid)
                    SetWalkable(pos.x, pos.y, false);
            }
        }
    }

    public void RemoveBuilding(BuildingBase instance)
    {
        Vector2Int origin = instance.OriginPoint;
        BuildingTypes type = instance.Type;
        for (int dx = 0; dx < type.Buildings[dx].width; dx++)
        {
            for (int dy = 0; dy < type.Buildings[dy].height; dy++)
            {
                Vector2Int pos = origin + new Vector2Int(dx, dy);
                _buildingOccupancy.Remove(pos);
                if (type.IsSolid)
                    SetWalkable(pos.x, pos.y, true);
            }
        }
    }

    public bool IsOccupied(Vector2Int pos)
    {
        return _buildingOccupancy.ContainsKey(pos);
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
