using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TerrainType", menuName = "TerrainType")]

public class TypeOfTerrain : ScriptableObject
{
    [SerializeField] private string terrainName = "Default";
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private bool walkable = true;
    [SerializeField] private int movementCost = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
