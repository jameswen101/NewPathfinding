using UnityEngine;

public class GridNode1: MonoBehaviour
{
    public string Name;
    public Vector3 WorldPosition;
    public bool Walkable;
    public int Weight;
    public bool isWall;
    public int fCost;
    public int gCost;
    public int hCost;

    public override bool Equals(object obj)
    {
        return obj is GridNode node &&
               WorldPosition == node.WorldPosition;
    }

    public override int GetHashCode()
    {
        return WorldPosition.GetHashCode();
    }
}
