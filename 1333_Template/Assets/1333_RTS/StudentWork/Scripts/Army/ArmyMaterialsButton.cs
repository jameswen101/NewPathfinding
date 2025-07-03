using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmyMaterialButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private ArmyMaterialSelector selector;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private TeamMaterialInfo teamMaterialInfo;

    private void Awake()
    {
    }

    private void OnButtonClicked()
    {
        selector.PlayerSelectMaterial(teamMaterialInfo);
    }

    public void Setup (TeamMaterialInfo teamMaterialInfo, ArmyMaterialSelector selector)
    {
        this.teamMaterialInfo = teamMaterialInfo;
        this.selector = selector;
        buttonText.text = teamMaterialInfo.name;
        buttonImage.sprite = teamMaterialInfo.materialIcon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClicked);
    }
}
