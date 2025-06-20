using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField] protected UnitType unitType;
    public int Width => unitType != null ? unitType.Width : 1;
    public int Height => unitType != null ? unitType.Height : 1;
    public abstract void MoveTo(GridNode targetNode);
    protected UnitState State;
    public virtual void Tick()
    {
        switch (State)
        {
            case UnitState.Moving:
                //DoMove();
                break;
            case UnitState.Attacking:
                break;
                //case 
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
