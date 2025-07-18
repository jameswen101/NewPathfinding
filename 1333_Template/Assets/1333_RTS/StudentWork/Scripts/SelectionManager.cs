using System.Collections;
using TMPro;
using UnityEngine;
using static UnitType;

public class SelectionManager : MonoBehaviour
{
    private Transform sourceObject;
    private Transform targetObject;
    private UnitInstance sourceUnit;
    private UnitInstance targetUnit;
    private BuildingInstance targetBuilding;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI statusText;
    private bool sourceWasMoving = false;

    void Update()
    {
        if (sourceUnit != null)
        {
            if (!sourceUnit.IsDead)
            {
                // Check if movement just stopped
                if (sourceWasMoving && !sourceUnit.IsMoving)
                {
                    Debug.Log($"Deselecting {sourceUnit.name} because it finished moving.");
                    sourceUnit = null;
                    sourceWasMoving = false;
                    statusText.text = "Unit deselected.";
                }
                else
                {
                    // Update moving state for next frame
                    sourceWasMoving = sourceUnit.IsMoving;
                }
            }
            else
            {
                Debug.Log($"Deselecting {sourceUnit.name} because it has died.");
                sourceUnit = null;
                sourceWasMoving = false;
                statusText.text = "Unit deselected.";
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (mainCamera == null)
            {
                Debug.LogError("SelectionManager: mainCamera is NULL!");
                return;
            }
            HandleClick();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete) && sourceUnit != null) //change to either backspace/delete/tab
        {
            DeselectAll();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete) && sourceUnit == null) //change to either backspace/delete/tab
        {
            Debug.Log("No unit to deselect");
            return;
        }
    }

    public void DeselectAll()
    {
        sourceUnit = null;
        targetUnit = null;
        statusText.text = "All units deselected";
        StartCoroutine(ClearStatusTextAfterDelay(2f)); // 2 seconds delay

        if (sourceUnit != null)
        {
            Debug.Log("Unable to deselect unit");
        }
    }


    private IEnumerator ClearStatusTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        statusText.text = "";
    }


    void HandleClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clicked = hit.collider.gameObject;

            if (clicked.CompareTag("Unit"))
            {
                UnitInstance clickedUnit = clicked.GetComponent<UnitInstance>();
                if (clickedUnit == null)
                {
                    Debug.LogWarning("Clicked object has no UnitInstance component!");
                    return;
                }

                // Only allow player-controlled units as source
                if (sourceUnit == null) //if player hasn't selected a source unit yet
                {
                    // Fallback: attempt to resolve missing Army
                    if (clickedUnit.Army == null && AllArmiesManager.Instance != null)
                    {
                        if (AllArmiesManager.Instance.TryGetArmy(clickedUnit.ArmyID, out ArmyData fallbackArmy))
                        {
                            clickedUnit.Army = fallbackArmy;
                            Debug.Log($"[{clickedUnit.name}] Fallback assigned Army: {fallbackArmy.name}");
                            Debug.Log($"[{clickedUnit.name}] Army ID: {clickedUnit.ArmyID}");
                        }
                        else
                        {
                            Debug.LogWarning($"[{clickedUnit.name}] Army still null after TryGetArmy with ID={clickedUnit.ArmyID}");
                        }
                    }

                    if (clickedUnit.Army != null && clickedUnit.Army.IsPlayerControlled) //if player selects a player unit
                    {
                        sourceUnit = clickedUnit;
                        statusText.text = $"Source unit selected: {sourceUnit.name}";
                    }
                    else
                    {
                        Debug.Log($"Clicked unit: {clickedUnit.name}");
                        Debug.Log($"Army null? {(clickedUnit.Army == null ? "YES" : "NO")}");
                        if (clickedUnit.Army != null)
                            Debug.Log($"IsPlayerControlled: {clickedUnit.Army.IsPlayerControlled}");
                            Debug.Log($"ArmyID: {clickedUnit.Army.ArmyID}");
                        if (clickedUnit.Army.ArmyID == 0)
                        {
                            clickedUnit.Army.IsPlayerControlled = true; // This is a player unit
                        }
                        statusText.text = clickedUnit.Army == null
                            ? "Unit has no army assigned."
                            : "Cannot select enemy units as source.";
                        return;
                    }
                }
                //choose target unit
                else if (sourceUnit != null && sourceUnit.Army != null && sourceUnit.Army.IsPlayerControlled) //if player has selected a source unit
                {
                    targetObject = clicked.transform;
                    targetUnit = clickedUnit;
                    if (targetUnit == null)
                    {
                        Debug.LogError("Target unit is NULL after clicking on unit!");
                        return;
                    }
                    // Check if the target unit is from the same army
                    if (targetUnit.Army != null && targetUnit.Army.IsPlayerControlled)
                    {
                        Debug.Log("Cannot select player units as target.");
                        statusText.text = "Cannot select player units as target.";
                        return;
                    }
                    else
                    {
                        Debug.Log($"Target unit selected: {targetUnit.name}");
                        statusText.text = $"Target unit selected: {targetUnit.name}";
                        ConfirmSelection();
                    }
                }

            }
            else if (clicked.CompareTag("Building"))
            {
                Debug.Log("Clicked on object with tag building");
                if (sourceUnit != null)
                {
                    targetObject = clicked.transform;
                    BuildingInstance clickedBuilding = clicked.GetComponent<BuildingInstance>();
                    if (clickedBuilding == null)
                    {
                        Debug.LogWarning("Clicked object has no BuildingInstance component!");
                        return;
                    }
                    //should player unit only be able to select enemy buildings to attack, or be able to hide in player buildings as well?
                    if (clickedBuilding.GetComponent<BuildingInstance>().Army != null && clickedBuilding.GetComponent<BuildingInstance>().Army.IsPlayerControlled) //if building belongs to player's army
                    {
                        Debug.Log("Cannot select player buildings as target.");
                        statusText.text = "Cannot select player buildings as target.";
                        return;
                    }
                    else
                    {
                        targetBuilding = clickedBuilding;
                        targetUnit = null; // Clear target unit since we're selecting a building
                        if (targetObject == null)
                        {
                            Debug.LogError("Target object is NULL after clicking on building!");
                            return;
                        }
                    }

                        Debug.Log($"Target building selected: {targetBuilding.name}"); //should it be target building or target object?
                    statusText.text = $"Target building selected: {targetBuilding.name}";
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

    void ConfirmSelection() //try to expand this to allow buildings
    {
        Debug.Log($"Ready to issue order: {sourceUnit.name} -> {targetUnit?.name ?? "no target"}"); //how to expand this to allow buildings?

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
                    if (sourceUnit == null || targetUnit == null)
                    {
                        Debug.LogError("Cannot attack, sourceUnit or targetUnit is null!");
                        return;
                    }

                    // Determine what type of target this is
                    bool isTargetBuilding = targetObject != null && targetObject.CompareTag("Building");

                    // Get the source attack type
                    AttackType attackType = sourceUnit.UnitType.attackType;

                    if (sourceUnit.UnitType.CanAttackUnits)
                    {
                        //compare unit types; if source unit is NOT an enemy unit type + if enemy unit IS an enemy unit type, then attack

                        if (sourceUnit.UnitType.unitTypeName == "Enemy" && targetUnit.UnitType.unitTypeName != "Enemy" || sourceUnit.UnitType.unitTypeName != "Enemy" && targetUnit.UnitType.unitTypeName == "Enemy")
                        {
                            sourceUnit.Attack(targetUnit);
                            Debug.Log($"Issued attack command: {sourceUnit.name} attacks {targetUnit.name}");
                            Debug.Log($"Target health remaining: {targetUnit.CurrentHealth}");
                        }

                        else
                        {
                            Debug.Log("Cannot attack unit on the same team.");
                            statusText.text = "Cannot attack unit on the same team.";
                        }
                    }
                    else
                    {
                        Debug.Log($"{sourceUnit.UnitType.unitTypeName} with attack type {attackType} cannot attack units.");
                        statusText.text = $"{sourceUnit.UnitType.unitTypeName} cannot attack units.";
                    }
                }
            }
            else
            {
                // If you don’t care about teams, always attack:
                if (sourceUnit == null || targetUnit == null)
                {
                    Debug.LogError("Cannot attack, sourceUnit or targetUnit is null!");
                    return;
                }
                //sourceUnit.Attack(targetUnit);
                //Debug.Log($"Issued attack command: {sourceUnit.name} attacks {targetUnit.name}");
                //Debug.Log($"Target health remaining: {targetUnit.CurrentHealth}");
            }
        }
        else if (sourceUnit != null && targetBuilding != null)
        {
            // If the target is a building, check if the source can attack buildings
            AttackType attackType = sourceUnit.UnitType.attackType;
            if (sourceUnit.UnitType.CanAttackBuildings)
            {
                sourceUnit.AttackBuilding(targetBuilding);
                Debug.Log($"Issued attack command: {sourceUnit.name} attacks {targetBuilding.name}");
                Debug.Log($"Target health remaining: {targetBuilding.CurrentHealth}");
                if (targetBuilding.name == "Castle" && targetBuilding.CurrentHealth <= 0)
                {
                    Debug.Log("Castle destroyed! Game over.");
                    statusText.text = "Castle destroyed! Game over.";
                    // Here you can add logic to end the game or trigger a game over state
                    targetBuilding.Win();
                }
                if (targetBuilding.name == "Castle" && targetBuilding.CurrentHealth > 0)
                {
                    Debug.Log("Castle is still standing.");
                    statusText.text = "Castle is still standing.";
                }
            }
            else
            {
                Debug.Log($"{sourceUnit.UnitType.unitTypeName} with attack type {attackType} cannot attack buildings.");
                statusText.text = $"{sourceUnit.UnitType.unitTypeName} cannot attack buildings.";
            }
        }
        else
        {
            Debug.LogWarning("Cannot issue attack: missing source or target unit.");
        }

        // Reset selection
        sourceUnit = null;
        targetUnit = null;
        targetBuilding = null;
    }

}