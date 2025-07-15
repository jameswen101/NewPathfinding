using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TeamMaterialInfo", menuName = "TeamMaterialInfo")]

public class TeamMaterialInfo: ScriptableObject
{
    public string teamName;
    public Material material;
    public Sprite materialIcon;
}

