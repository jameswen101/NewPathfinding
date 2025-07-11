using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

public class SelectBuildingButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button button;
    // Start is called before the first frame update
    [SerializeField] private BuildingData buildingData;
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private AudioManager audioManager;

    public void OnClick()
    {
        buildingPlacer.StartPlacing(buildingData);
    }

    public void Setup(BuildingData buildingData, BuildingPlacer buildingPlacer, AudioManager audioManager) //add buildingPlacer as parameter
    {
        this.buildingData = buildingData;
        this.buildingPlacer = buildingPlacer;
        this.audioManager = audioManager;

        buttonText.text = buildingData.buildingName;
        buttonImage.sprite = buildingData.buildingIcon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioManager.PlaySFX("Answer Button");
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
