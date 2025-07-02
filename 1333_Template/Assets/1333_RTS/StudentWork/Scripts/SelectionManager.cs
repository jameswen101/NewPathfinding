using TMPro;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private Transform sourceUnit;
    private Transform targetObject;
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
                if (sourceUnit == null)
                {
                    sourceUnit = clicked.transform;
                    Debug.Log($"Source unit selected: {sourceUnit.name}");
                    statusText.text = $"Source unit selected: {sourceUnit.name}";
                }
                else
                {
                    targetObject = clicked.transform;
                    Debug.Log($"Target unit selected: {targetObject.name}");
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
            else
            {
                Debug.Log("Clicked on an unrecognized object.");
            }
        }
    }

    void ConfirmSelection()
    {
        // This is where you trigger your pathfinding or attack logic
        Debug.Log($"Ready to issue order: {sourceUnit.name} -> {targetObject.name}");

        // Reset selection so you can pick a new source later
        sourceUnit = null;
        targetObject = null;
    }
}