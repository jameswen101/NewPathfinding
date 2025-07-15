using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MachineManager : MonoBehaviour
{
    //[SerializeField] private MachineType machineTypes;
    [SerializeField] private PathFinder pathFinder;
    [SerializeField] private GridManager gridManager;
    private List<MachineInstance> machines = new List<MachineInstance>();

    public void SpawnMachine(MachineType machineData, Vector3 position, ArmyData ownerArmy)
    {
        GameObject prefab = machineData.machinePrefab;
        if (prefab == null)
        {
            Debug.LogError("Machine prefab is missing.");
            return;
        }

        GameObject go = Instantiate(prefab, position, Quaternion.identity);
        MachineInstance instance = go.GetComponent<MachineInstance>();

        if (instance == null)
        {
            Debug.LogError("Prefab is missing MachineInstance component.");
            return;
        }

        Vector2Int positionTwo = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        instance.AssignToArmy(ownerArmy);
        instance.Initialize(pathFinder, ownerArmy.TeamMaterial, gridManager, machineData, positionTwo);

        machines.Add(instance);
    }

    public void RemoveMachine(MachineInstance machine)
    {
        if (machines.Contains(machine))
        {
            machines.Remove(machine);
            Destroy(machine.gameObject);
        }
    }
}
