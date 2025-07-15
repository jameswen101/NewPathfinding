using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArmyMaterialButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Button button;
    [SerializeField] private ArmyMaterialSelector selector;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private TeamMaterialInfo teamMaterialInfo;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
    }

    private void OnButtonClicked()
    {
        audioManager.PlaySFX("Beep Short");
        selector.PlayerSelectMaterial(teamMaterialInfo);
    }

    public void Setup (TeamMaterialInfo teamMaterialInfo, ArmyMaterialSelector selector, AudioManager audioManager)
    {
        this.teamMaterialInfo = teamMaterialInfo;
        this.selector = selector;
        this.audioManager = audioManager;
        buttonText.text = teamMaterialInfo.name;
        buttonImage.sprite = teamMaterialInfo.materialIcon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClicked);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioManager == null)
        {
            Debug.LogError("AudioManager is NULL! No audio will play.");
        }
        else
        {
            Debug.Log("AudioManager exists.");
        }

        audioManager.PlaySFX("Answer Button");
    }
}
