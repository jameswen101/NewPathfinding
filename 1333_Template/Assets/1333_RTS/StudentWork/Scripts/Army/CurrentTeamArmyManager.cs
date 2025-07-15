using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentTeamArmyManager : MonoBehaviour
{
    public int ArmyID;
    public bool IsPlayer => ArmyID == 0;
    public List<UnitBase> CurrentlyActiveUnits = new List<UnitBase>();
    private AvailableTeamUnits _spawnableUnits;

    public void Initalize(AvailableTeamUnits spawnableUnits)
    {
        _spawnableUnits = spawnableUnits;
    }

    public void UpdateAllUnits()
    {
        foreach (UnitBase unit in CurrentlyActiveUnits)
        {
            unit.Tick();
        }
    }

    public void SpawnUnit()
    {
        //Instantiate(_spawnableUnits[Random.Range(0, _spawnableUnits.AvailableUnits.Count - 1)], transform.position); //currently error
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
