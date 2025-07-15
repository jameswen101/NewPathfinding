using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectUnitButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button button;
    // Start is called before the first frame update
    [SerializeField] private UnitType unitType;
    [SerializeField] private SoldierPlacer soldierPlacer;
    [SerializeField] private AudioManager audioManager;
    public void OnClick()
    {
        audioManager.PlaySFX("Beep Short");
        soldierPlacer.StartPlacingSoldier(unitType);
    }

    public void Setup(UnitType unitType, SoldierPlacer soldierPlacer, AudioManager audioManager) //add soldierPlacer as parameter
    {
        this.unitType = unitType;
        this.soldierPlacer = soldierPlacer;
        this.audioManager = audioManager;

        buttonText.text = unitType.unitTypeName;
        buttonImage.sprite = unitType.unitIcon; //make sure every UnitType has unitIcon set up
        Debug.Log($"Assigning icon for {unitType.unitTypeName}: {unitType.unitIcon}");

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioManager.PlaySFX("Answer Button");
    }
}
