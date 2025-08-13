using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacementUI : MonoBehaviour
{
    [SerializeField] private RectTransform layoutGroupParent;
    [SerializeField] private SelectUnitButton ButtonPrefab;
    [SerializeField] private AvailableTeamUnits availableTeamUnits;
    [SerializeField] private SoldierPlacer soldierPlacer;
    [SerializeField] private UnitPlacementManager unitPlacementManager;
    [SerializeField] private ArmyData armyData;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private EnemyAIManager enemyAIManager;
    private int buttonCount;

    // Start is called before the first frame update
    void Start()
    {
        //soldierPlacer.SetArmyData(armyData);
        if (enemyAIManager == null)
        {
            enemyAIManager = FindAnyObjectByType<EnemyAIManager>();
            if (enemyAIManager == null)
            {
                Debug.LogError("EnemyAIManager not found in the scene!");
            }
        }
        foreach (UnitType ut in availableTeamUnits.AvailableUnits)
        {
            var buttonGO = Instantiate(ButtonPrefab, layoutGroupParent);
            var button = buttonGO.GetComponent<SelectUnitButton>();
            button.Setup(ut, soldierPlacer, audioManager, unitPlacementManager);
            buttonCount++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (availableTeamUnits.HasNewUnits)
        {
            // refresh available team units
            if (buttonCount != availableTeamUnits.AvailableUnits.Count)
            {
                //find the difference between the current button count and the available units count
                int difference = availableTeamUnits.AvailableUnits.Count - buttonCount;
                // if there are more available units, add buttons
                for (int i = 0; i < difference; i++)
                {
                    var buttonGO = Instantiate(ButtonPrefab, layoutGroupParent);
                    var button = buttonGO.GetComponent<SelectUnitButton>();
                    UnitType newUnitType = availableTeamUnits.AvailableUnits[buttonCount + i];
                    button.Setup(newUnitType, soldierPlacer, audioManager, unitPlacementManager);
                    buttonCount++;
                }
            }
            availableTeamUnits.HasNewUnits = false;
        }
    }
}
