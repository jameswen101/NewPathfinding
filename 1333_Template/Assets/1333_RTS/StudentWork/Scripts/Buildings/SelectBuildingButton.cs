using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

public class SelectBuildingButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button button;
    // Start is called before the first frame update
    [SerializeField] private BuildingData buildingData;
    [SerializeField] private BuildingPlacer buildingPlacer;

    public void OnClick()
    {
        buildingPlacer.StartPlacing(buildingData);
    }

    public void Setup(BuildingData buildingData)
    {
        this.buildingData = buildingData;

        buttonText.text = buildingData.buildingName;
        buttonImage.sprite = buildingData.buildingIcon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
