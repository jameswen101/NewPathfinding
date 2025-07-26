using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ArmyMaterialSelector : MonoBehaviour
{
    [SerializeField] private List<TeamMaterialInfo> availableMaterials;
    [SerializeField] private GameObject selectionPanel; // parent object for the buttons
    [SerializeField] private TeamMaterialsCollection teamMaterials;
    [SerializeField] private ArmyMaterialButton selectMaterialButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float selectionTime = 10f; // Time limit for material selection
    private bool hasAutoSelected = false;
    private bool playerSelected = false;
    public bool materialsSelected => playerSelected || hasAutoSelected;
    GameObject[] armyObjects;
    ArmyData playerArmy = null;
    ArmyData enemyArmy = null;
    public event System.Action OnArmiesReady;
    private bool armiesReady = false;


    private void Start()
    {
        timerText.text = $"Time left: {selectionTime:F1} seconds";
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

    private void Update()
    {
        selectionTime -= Time.deltaTime;
        if (selectionTime >= 0f)
        {
            // Update the timer text
            timerText.text = $"Time left: {selectionTime:F1} seconds";
        }
        else
        {
            // Time is up, handle auto-selection if not already done
            if (!hasAutoSelected && !playerSelected)
            {
                AutoSelectMaterials();
                hasAutoSelected = true;  // Prevent any future calls
                timerText.text = $"Time left: {selectionTime:F1} seconds"; //how can this be changed when time is up?

            }
        }
    }


    public void PlayerSelectMaterial(TeamMaterialInfo chosenMaterial)
    {
        // Assign material to player
        playerArmy.SetTeamMaterial(chosenMaterial.material); //if we separate unit + building materials, we will need to make 2 functions for setting materials
        playerSelected = true;
        foreach (BuildingInstance building in playerArmy.Buildings)
        {
            Renderer[] renderers = building.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                Material[] mats = renderer.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = chosenMaterial.material;
                }
                renderer.materials = mats;

            }
            Debug.Log($"Player building {building.name} set to material {chosenMaterial.name}.");
        }

        // Remove chosen from pool
        availableMaterials.Remove(chosenMaterial);

        // Randomly pick for enemy
        TeamMaterialInfo enemyMaterial = availableMaterials[Random.Range(0, availableMaterials.Count)];
        enemyArmy.SetTeamMaterial(enemyMaterial.material);
        foreach (BuildingInstance building in enemyArmy.Buildings)
        {
            Renderer[] renderers = building.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                Material[] mats = renderer.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = enemyMaterial.material;
                }
                renderer.materials = mats;
            }
            Debug.Log($"Enemy building {building.name} set to material {enemyMaterial.name}.");
        }

        // Hide UI
        selectionPanel.SetActive(false);
        StartCoroutine(ClearTimerText());
        if (!armiesReady)
        {
            armiesReady = true;
            OnArmiesReady?.Invoke();
        }
        Debug.Log($"Player picked {chosenMaterial.name}, enemy picked {enemyMaterial.name}");
    }

    private void AutoSelectMaterials()
    {
        if (availableMaterials.Count < 2)
        {
            Debug.LogError("Not enough materials available for auto-selection.");
            return;
        }
        // Randomly select two different materials
        TeamMaterialInfo playerMaterial = availableMaterials[Random.Range(0, availableMaterials.Count)];
        availableMaterials.Remove(playerMaterial);
        TeamMaterialInfo enemyMaterial = availableMaterials[Random.Range(0, availableMaterials.Count)];
        // Assign materials to armies
        playerArmy.SetTeamMaterial(playerMaterial.material);
        enemyArmy.SetTeamMaterial(enemyMaterial.material);
        // Hide UI
        selectionPanel.SetActive(false);
        Debug.Log($"Auto-selected: Player picked {playerMaterial.name}, Enemy picked {enemyMaterial.name}");
        timerText.text = "Time's up! Materials auto-selected.";
        if (!armiesReady)
        {
            TryInvokeArmiesReady();
        }
        // change text to blank after 2 seconds
        StartCoroutine(ClearTimerText());
    }

    private IEnumerator ClearTimerText()
    {
        yield return new WaitForSeconds(2f);
        timerText.text = "";
        timerText.gameObject.SetActive(false);
        if (!timerText.gameObject.activeSelf)
            Debug.Log("Timer text is inactive on itself.");
        else
        {
            Debug.Log("Timer text is still active on itself.");
        }
    }

    private void TryInvokeArmiesReady()
    {
        if (!armiesReady)
        {
            armiesReady = true;
            OnArmiesReady?.Invoke();
        }
    }
}

