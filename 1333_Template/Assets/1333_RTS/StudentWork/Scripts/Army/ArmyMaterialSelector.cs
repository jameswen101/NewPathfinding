using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyMaterialSelector : MonoBehaviour
{
    [SerializeField] private List<TeamMaterialInfo> availableMaterials;
    [SerializeField] private GameObject selectionPanel; // parent object for the buttons
    [SerializeField] private TeamMaterialsCollection teamMaterials;
    [SerializeField] private ArmyMaterialButton selectMaterialButton;

    private bool playerSelected = false;
    GameObject[] armyObjects;
    ArmyData playerArmy = null;
    ArmyData enemyArmy = null;
    private void Start()
    {
        foreach (GameObject obj in armyObjects)
        {
            ArmyData data = obj.GetComponent<ArmyData>();
            if (data.ArmyID == 0)
                playerArmy = data;
            else if (data.ArmyID == 1)
                enemyArmy = data;
        }
    }

    private void Awake()
    {
        // Create a fresh copy of the list every time
        availableMaterials = new List<TeamMaterialInfo>(teamMaterials.materials);

        armyObjects = GameObject.FindGameObjectsWithTag("Army");
    }


    public void PlayerSelectMaterial(TeamMaterialInfo chosenMaterial)
    {
        // Assign material to player
        playerArmy.SetTeamMaterial(chosenMaterial.material);
        playerSelected = true;

        // Remove chosen from pool
        availableMaterials.Remove(chosenMaterial);

        // Randomly pick for enemy
        TeamMaterialInfo enemyMaterial = availableMaterials[Random.Range(0, availableMaterials.Count)];
        enemyArmy.SetTeamMaterial(enemyMaterial.material);

        // Hide UI
        selectionPanel.SetActive(false);

        Debug.Log($"Player picked {chosenMaterial.name}, enemy picked {enemyMaterial.name}");
    }
}

