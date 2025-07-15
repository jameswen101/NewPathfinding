using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamMaterialsCollection", menuName = "RTS/Team Materials Collection")]
public class TeamMaterialsCollection : ScriptableObject
{
    [Tooltip("List of team materials to choose from.")]
    public List<TeamMaterialInfo> materials;
}

