using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectMachineButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button button;
    [SerializeField] private MachineType machineType;
    [SerializeField] private MachinePlacer machinePlacer;
    [SerializeField] private ArmyData armyData;

    public void Setup(MachineType type, MachinePlacer placer, ArmyData data)
    {
        machineType = type;
        machinePlacer = placer;
        armyData = data;

        buttonText.text = type.machineName;
        buttonImage.sprite = type.machineIcon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        machinePlacer.StartPlacingMachine(machineType, armyData);
    }
}
