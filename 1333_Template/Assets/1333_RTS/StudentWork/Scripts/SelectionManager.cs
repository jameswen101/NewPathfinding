using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private ArmyData playerArmy;
    [SerializeField] private ArmyData enemyArmy;
    [SerializeField] private SoldierMovementManager soldierMovementManager;
    public TextMeshProUGUI statusText;
    private bool sourceWasMoving = false;
    private bool canAutoAttackBuildings = false;

    void Update()
    {
        if (playerArmy.Units.Count >= 7 && playerArmy.Buildings.Count >= 4)
        {
            canAutoAttackBuildings = true;
        }
        if (canAutoAttackBuildings) 
        {
            AutoAttackEnemyBuildings();
        }
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

            ClickProxy clickProxy = hit.collider.GetComponent<ClickProxy>();
            if (clickProxy != null && clickProxy.linkedObject != null)
            {
                Debug.Log("Clicked on a ClickProxy attached to a health bar or child object.");
                clicked = clickProxy.linkedObject; // Now can be a Unit or Building GameObject
            }

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
                //if source can heal, allow the player to choose a player unit as target
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
                        if (sourceUnit.UnitType.CanHeal)
                        {
                            Debug.Log($"Target unit selected: {targetUnit.name}");
                            statusText.text = $"Target unit selected: {targetUnit.name}";
                            ConfirmSelection();
                        }
                        else
                        {
                            Debug.Log("Cannot select player units as target for attack.");
                            statusText.text = "Cannot select player units as target for attack.";
                            return;
                        }
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

    void ConfirmSelection() //try to expand this to allow player to go to player buildings
    {
        Debug.Log($"Ready to issue order: {sourceUnit.name} -> {targetUnit?.name ?? "no target"}"); //how to expand this to allow buildings?


        if (sourceUnit != null && targetUnit != null) //player chooses to attack a unit
        {
            // Check if they are enemies
            if (sourceUnit.Army != null && targetUnit.Army != null) //else = sourceUnit or targetUnit has no army assigned
            {
                if (sourceUnit.Army.TeamMaterial == targetUnit.Army.TeamMaterial) //if they are from the same team
                {

                    if (sourceUnit.UnitType.CanHeal)
                    {
                        // If the source unit is a Mage, allow healing
                        sourceUnit.Heal(targetUnit);
                        Debug.Log($"Issued heal command: {sourceUnit.name} heals {targetUnit.name}");
                        Debug.Log($"Target health after healing: {targetUnit.CurrentHealth}");
                        statusText.text = $"Target health after healing: {targetUnit.CurrentHealth}";
                        //don't reset sourceUnit, as player may want to heal multiple units in a row
                        //if player wishes to change units, they can press backspace/delete
                        targetUnit = null; // Reset target unit after healing
                        targetObject = null; // Reset target object after healing
                    }
                    else
                    {
                        Debug.Log($"{sourceUnit.UnitType} cannot heal other units.");
                        statusText.text = $"{sourceUnit.UnitType} cannot heal other units.";
                        //don't reset sourceUnit, as they may want to attack multiple units in a row or clicked on a player unit by mistake
                        targetUnit = null; // Deselect target unit as well
                        targetObject = null; // Deselect target object as well
                    }
                }
                else //if they are from different teams
                {
                    // Determine what type of target this is
                    bool isTargetBuilding = targetObject != null && targetObject.CompareTag("Building");

                    // Get the source attack type
                    AttackType attackType = sourceUnit.UnitType.attackType;

                    if (sourceUnit.UnitType.CanAttackUnits)
                    {
                        //compare unit types; if source unit is NOT an enemy unit type + if enemy unit IS an enemy unit type, then attack

                        if (sourceUnit.UnitType.unitTypeName == "Enemy" && targetUnit.UnitType.unitTypeName != "Enemy" || sourceUnit.UnitType.unitTypeName != "Enemy" && targetUnit.UnitType.unitTypeName == "Enemy")
                        //if unit types are from different sides
                        {
                            sourceUnit.Attack(targetUnit); //make the attack automated
                            Debug.Log($"Issued attack command: {sourceUnit.name} attacks {targetUnit.name}");
                            Debug.Log($"Target health remaining: {targetUnit.CurrentHealth}");
                            statusText.text = $"Target health remaining: {targetUnit.CurrentHealth}";
                            sourceUnit = null; // Reset source unit after attack
                            targetUnit = null; // Reset target unit after attack
                            targetObject = null; // Reset target object after attack
                        }
                    }
                    else
                    {
                        Debug.Log($"{sourceUnit.UnitType.unitTypeName} with attack type {attackType} cannot attack units.");
                        statusText.text = $"{sourceUnit.UnitType.unitTypeName} cannot attack units.";
                        targetUnit = null; // Reset target unit if it cannot be attacked
                    }
                }
            }
            else
            {
                if (sourceUnit.Army == null || targetUnit.Army == null)
                {
                    Debug.LogError("Cannot attack, sourceUnit or targetUnit's army is unknown!");
                    return;
                }
            }
        }
        else if (sourceUnit != null && targetBuilding != null) //player chooses to attack a building
        {
            // If the target is a building, check if the source can attack buildings
            AttackType attackType = sourceUnit.UnitType.attackType;
            if (sourceUnit.UnitType.CanAttackBuildings)
            {
                sourceUnit.AttackBuilding(targetBuilding); //make the attack automated
                Debug.Log($"Issued attack command: {sourceUnit.name} attacks {targetBuilding.name}");
                Debug.Log($"Target health remaining: {targetBuilding.CurrentHealth}");
                statusText.text = $"{targetBuilding.name} health remaining: {targetBuilding.CurrentHealth}";
                // Reset selection
                sourceUnit = null;
                targetUnit = null;

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
                // Handle moving to friendly buildings
                if (targetBuilding.Army != null && targetBuilding.Army.IsPlayerControlled)
                {
                    sourceUnit.SetDestination(targetBuilding.transform.position);
                    Debug.Log($"{sourceUnit.name} is moving to defend building {targetBuilding.name}");
                    statusText.text = $"{sourceUnit.name} is now guarding {targetBuilding.name}";
                    sourceUnit = null;
                    targetBuilding = null;
                    return;
                }
                Debug.Log($"{sourceUnit.UnitType.unitTypeName} with attack type {attackType} cannot attack buildings.");
                statusText.text = $"{sourceUnit.UnitType.unitTypeName} cannot attack buildings.";
            }
        }
        else //if either sourceUnit or targetUnit and targetBuilding are both null
        {
            Debug.LogWarning("Cannot issue attack: missing source or target unit.");
        }
    }

    public void IssueAutomatedCommand(UnitInstance sourceUnit = null, BuildingInstance sourceBuilding = null, UnitInstance target = null)
    {
        this.sourceUnit = sourceUnit;
        targetUnit = target;
        sourceObject = (sourceUnit != null) ? sourceUnit.transform : (sourceBuilding != null) ? sourceBuilding.transform : null;
        if (sourceBuilding != null && sourceUnit == null)
        {
            sourceUnit = playerArmy.Units[0]; // Default to the first unit in the player's army if no source unit is specified
            //look for closest unit -> go to building where enemy will attack
            for (int i = 1; i < enemyArmy.Units.Count; i++)
            {
                UnitInstance unit = enemyArmy.Units[i];
                if (unit == null || unit.IsDead || !unit.UnitType.CanAttackUnits) //skip over all units that can't attack enemy units
                    continue;
                // Check if this unit is closer than the current targetUnit
                if (Vector3.Distance(sourceBuilding.transform.position, unit.transform.position) < Vector3.Distance(sourceBuilding.transform.position, sourceUnit.transform.position))
                {
                    sourceUnit = unit; // Assign the closest unit to sourceUnit
                }
            }
            sourceUnit.SetDestination(sourceBuilding.transform.position);
            Debug.Log($"Assigned closest unit {sourceUnit.name} to attack enemy unit {target.name} trying to attack building {sourceBuilding.name}");
        }
        if (sourceUnit == null && sourceBuilding == null)
        {
            Debug.LogError("No target unit or building specified for automated command.");
            return;
        }

        if (sourceUnit != null && targetUnit != null)
        {
            ConfirmSelection();
        }
        else
        {
            if (sourceUnit == null)
            {
                Debug.LogError("Source unit is null in IssueAutomatedCommand.");
            }
            else if (targetUnit == null)
            {
                Debug.LogError("Target unit is null in IssueAutomatedCommand.");
            }
        }
    }

    public void AutoAttackEnemyBuildings()
    {
        Debug.Log("[AutoAttackEnemyBuildings] Called");

        // 1. Filter player units that can attack buildings and are alive
        List<UnitInstance> attackingUnits = playerArmy._units.FindAll(
            u => u != null && !u.IsDead && u.UnitType.CanAttackBuildings
        );

        // 2. Filter enemy buildings that are alive and not the castle yet
        List<BuildingInstance> enemyWalls = enemyArmy._buildings.FindAll(
            b => b != null && !b.IsDead && b.name.Contains("Wall")
        );

        // 3. Try to find the enemy castle
        BuildingInstance enemyCastle = enemyArmy._buildings.Find(
            b => b != null && b.name == "CastlePrefab"
        );

        Debug.Log($"[AutoAttackEnemyBuildings] Player units: {attackingUnits.Count}");
        Debug.Log($"[AutoAttackEnemyBuildings] Enemy buildings: {enemyWalls.Count + 1}");

        if (attackingUnits.Count == 0)
        {
            Debug.LogWarning("No player units available that can attack buildings.");
            return;
        }

        if ((enemyWalls == null || enemyWalls.Count == 0) && enemyCastle == null)
        {
            Debug.LogWarning("No enemy walls or castle found.");
            return;
        }

        // 4. Loop through all eligible attacking units
        foreach (UnitInstance unit in attackingUnits)
        {
            BuildingInstance target = null;

            // Prioritize walls first
            if (enemyWalls != null && enemyWalls.Count > 0)
            {
                // Find the closest wall to this unit
                target = enemyWalls
                    .OrderBy(w => Vector3.Distance(unit.transform.position, w.transform.position))
                    .FirstOrDefault();
            }
            else if (enemyCastle != null && !enemyCastle.IsDead)
            {
                target = (BuildingInstance)enemyCastle;
            }

            if (target != null)
            {
                Debug.Log($"Auto-assigning {unit.name} to attack {target.name}");
                targetBuilding = (BuildingInstance)target;
                sourceUnit = unit; // Set the source unit for the command
                // If the target is a building, set the destination
                if (targetBuilding != null)
                {
                    sourceUnit.SetDestination(targetBuilding.transform.position);
                    Debug.Log($"{sourceUnit.name} is moving to attack {targetBuilding.name}");
                    statusText.text = $"{sourceUnit.name} is now attacking {targetBuilding.name}";
                }
                else
                {
                    Debug.LogWarning("Target building is null after auto-attack assignment.");
                }
                if (sourceUnit != null && targetBuilding != null)
                {
                    Debug.Log($"Issued automated attack command: {sourceUnit.name} attacks {targetBuilding.name}");
                    statusText.text = $"{sourceUnit.name} is attacking {targetBuilding.name}";
                    // Confirm the selection and issue the attack command
                    ConfirmSelection();
                }
                else
                {
                    if (sourceUnit == null)
                    {
                        Debug.LogError("Source unit is null after auto-attack assignment.");
                    }
                    if (targetBuilding == null)
                    {
                        Debug.LogError("Target building is null after auto-attack assignment.");
                    }
                }
            }
        }
    }


}