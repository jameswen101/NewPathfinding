using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GridNode 
{
    public string Name; //grid index
    public Vector3 WorldPosition;
    public bool Walkable;
    public TerrainType TerrainType;
    public int Weight;
    public bool isWall;
    public int fCost;
    public int gCost;
    public int hCost;
    public Color GizmoColor;
    public Vector2Int Coordinates => new Vector2Int((int)WorldPosition.x, (int)WorldPosition.y);

}
