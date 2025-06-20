using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TerrainType", menuName = "TerrainType")]

public class TerrainType : ScriptableObject
{
    public string terrainName = "Default";
    //[SerializeField] private 
    public Color gizmoColor;
    //[SerializeField] private
    public bool walkable = true;
    //[SerializeField] private 
    public int movementCost = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
