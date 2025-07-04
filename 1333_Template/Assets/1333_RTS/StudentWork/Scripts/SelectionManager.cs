using TMPro;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private Transform sourceObject;
    private Transform targetObject;
    private UnitInstance sourceUnit;
    private UnitInstance targetUnit;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI statusText;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clicked = hit.collider.gameObject;

            if (clicked.CompareTag("Unit"))
            {
                Debug.Log("Clicked on object with tag unit");
                UnitInstance clickedUnit = clicked.GetComponent<UnitInstance>();
                if (clickedUnit == null)
                {
                    Debug.LogWarning("Clicked object has no UnitInstance component!");
                    return;
                }

                if (sourceUnit == null)
                {
                    sourceUnit = clickedUnit;
                    Debug.Log($"Source unit selected: {sourceUnit.name} (ArmyID {sourceUnit.ArmyID})");
                    statusText.text = $"Source unit selected: {sourceUnit.name}";
                }
                else
                {
                    targetUnit = clickedUnit;
                    Debug.Log($"Target unit selected: {targetUnit.name} (ArmyID {targetUnit.ArmyID})");
                    ConfirmSelection();
                }
            }
            else if (clicked.CompareTag("Building"))
            {
                Debug.Log("Clicked on object with tag building");
                if (sourceUnit != null)
                {
                    targetObject = clicked.transform;
                    Debug.Log($"Target building selected: {targetObject.name}");
                    ConfirmSelection();
                }
                else
                {
                    Debug.Log("You must select a unit first before choosing a target building.");
                }
            }
            else if (clicked.CompareTag("Machine"))
            {
                Debug.Log("Clicked on object with tag machine");
                if (sourceUnit != null)
                {
                    targetObject = clicked.transform;
                    Debug.Log($"Target machine selected: {targetObject.name}");
                    ConfirmSelection();
                }
            }
            else
            {
                Debug.Log("Clicked on an unrecognized object.");
            }
        }
    }

    void ConfirmSelection()
    {
        Debug.Log($"Ready to issue order: {sourceUnit.name} -> {targetUnit?.name ?? "no target"}");

        if (sourceUnit != null && targetUnit != null)
        {
            // Check if they are enemies
            if (sourceUnit.Army != null && targetUnit.Army != null)
            {
                if (sourceUnit.Army.TeamMaterial == targetUnit.Army.TeamMaterial)
                {
                    Debug.Log("Cannot attack unit on the same team.");
                }
                else
                {
                    // They are on different teams, ATTACK!
                    sourceUnit.Attack(targetUnit);
                    Debug.Log($"Issued attack command: {sourceUnit.name} attacks {targetUnit.name}");
                    Debug.Log($"Target health remaining: {targetUnit.CurrentHealth}");
                }
            }
            else
            {
                // If you don’t care about teams, always attack:
                sourceUnit.Attack(targetUnit);
                Debug.Log($"Issued attack command: {sourceUnit.name} attacks {targetUnit.name}");
                Debug.Log($"Target health remaining: {targetUnit.CurrentHealth}");
            }
        }
        else
        {
            Debug.LogWarning("Cannot issue attack: missing source or target unit.");
        }

        // Reset selection
        sourceUnit = null;
        targetUnit = null;
    }

}