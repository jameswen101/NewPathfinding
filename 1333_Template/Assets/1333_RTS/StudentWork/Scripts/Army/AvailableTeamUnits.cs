using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AvailableTeamUnits", menuName = "AvailableTeamUnits")]

public class AvailableTeamUnits : ScriptableObject
{
    public List<UnitType> AvailableUnits = new();
    public List<UnitType> DefaultUnits = new();
    //a bool variable to check if there are any new units that need to be added to UI
    public bool HasNewUnits = false;
    // Start is called before the first frame update

    public void AddUnit(UnitType unit)
    {
        if (!AvailableUnits.Contains(unit))
        {
            AvailableUnits.Add(unit);
            HasNewUnits = true;
        }
    }

    void Start()
    {
        AvailableUnits = DefaultUnits;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
