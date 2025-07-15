using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyManager : MonoBehaviour
{
    public int ArmyID;
    public bool IsPlayer => ArmyID == 0;
    public List<UnitBase> Units = new List<UnitBase>();
    public List<BuildingBase> Buildings = new List<BuildingBase> (); 
    public GridManager GridManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
