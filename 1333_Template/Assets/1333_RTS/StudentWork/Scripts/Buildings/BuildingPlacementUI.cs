using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementUI : MonoBehaviour
{
    [SerializeField] private RectTransform layoutGroupParent;
    [SerializeField] private SelectBuildingButton ButtonPrefab;
    [SerializeField] private BuildingTypes BuildingData;
    [SerializeField] private BuildingPlacer buildingPlacer;

    // Start is called before the first frame update
    void Start()
    {
        foreach(BuildingData t in BuildingData.Buildings)
        {
            SelectBuildingButton button = Instantiate(ButtonPrefab, layoutGroupParent);
            button.Setup(t, buildingPlacer);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
