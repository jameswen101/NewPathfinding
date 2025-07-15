using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinePlacementUI : MonoBehaviour
{
    [SerializeField] private RectTransform layoutGroupParent;
    [SerializeField] private SelectMachineButton ButtonPrefab;
    [SerializeField] private MachineCollection MachineCollection;
    [SerializeField] private MachinePlacer machinePlacer;
    [SerializeField] private ArmyData armyData;

    // Start is called before the first frame update
    void Start()
    {
        foreach (MachineType mt in MachineCollection.MachineList)
        {
            SelectMachineButton button = Instantiate(ButtonPrefab, layoutGroupParent);
            button.Setup(mt, machinePlacer, armyData);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
