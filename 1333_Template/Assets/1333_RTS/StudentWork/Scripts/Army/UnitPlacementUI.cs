using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacementUI : MonoBehaviour
{
    [SerializeField] private RectTransform layoutGroupParent;
    [SerializeField] private SelectUnitButton ButtonPrefab;
    [SerializeField] private AvailableTeamUnits availableTeamUnits;
    [SerializeField] private SoldierPlacer soldierPlacer;
    [SerializeField] private ArmyData armyData;

    // Start is called before the first frame update
    void Start()
    {
        soldierPlacer.SetArmyData(armyData);
        foreach (UnitType ut in availableTeamUnits.AvailableUnits)
        {
            var buttonGO = Instantiate(ButtonPrefab, layoutGroupParent);
            var button = buttonGO.GetComponent<SelectUnitButton>();
            button.Setup(ut, soldierPlacer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
