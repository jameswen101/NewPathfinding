using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AvailableTeamUnits", menuName = "AvailableTeamUnits")]

public class AvailableTeamUnits : ScriptableObject
{
    public List<UnitType> AvailableUnits = new List<UnitType>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
