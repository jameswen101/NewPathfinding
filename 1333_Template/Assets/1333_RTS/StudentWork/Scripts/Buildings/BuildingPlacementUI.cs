using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementUI : MonoBehaviour
{
    [SerializeField] private RectTransform layoutGroupParent;
    [SerializeField] private SelectBuildingButton ButtonPrefab;
    [SerializeField] private BuildingTypes buildingTypes;
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private ArmyData armyData;
    [SerializeField] private AudioManager audioManager;
    private int buttonCount;

    // Start is called before the first frame update
    void Start()
    {
        buildingPlacer.SetArmyData(armyData);
        foreach (BuildingData t in buildingTypes.Buildings)
        {
            SelectBuildingButton button = Instantiate(ButtonPrefab, layoutGroupParent);
            button.Setup(t, buildingPlacer, audioManager);
            buttonCount++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (buildingTypes.HasNewBuildings)
        {
            // refresh available team units
            if (buttonCount != buildingTypes.Buildings.Count)
            {
                //find the difference between the current button count and the available buildings count
                int difference = buildingTypes.Buildings.Count - buttonCount;
                // if there are more available buttons, add buttons
                for (int i = 0; i < difference; i++)
                {
                    var buttonGO = Instantiate(ButtonPrefab, layoutGroupParent);
                    var button = buttonGO.GetComponent<SelectBuildingButton>();
                    BuildingData newBuildingData = buildingTypes.Buildings[buttonCount + i];
                    button.Setup(newBuildingData, buildingPlacer, audioManager);
                    buttonCount++;
                }
            }
            buildingTypes.HasNewBuildings = false;
        }
    }
}
