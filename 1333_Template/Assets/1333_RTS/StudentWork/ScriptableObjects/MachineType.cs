using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMachineType", menuName = "RTS/Machine Type")]
public class MachineType : ScriptableObject
{
    public string machineName;
    public GameObject machinePrefab;
    public Sprite machineIcon;
    public float moveSpeed;
    public float attackRange;
    public float damage;
    public int width;   // grid size
    public int height;
    // Any machine-specific fields (ammo count, reload time)
}
