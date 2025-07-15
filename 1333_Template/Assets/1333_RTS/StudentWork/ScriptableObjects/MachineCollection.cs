using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMachineCollection", menuName = "RTS/Machine Collection")]

public class MachineCollection : ScriptableObject
{
    public List<MachineType> MachineList = new();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
