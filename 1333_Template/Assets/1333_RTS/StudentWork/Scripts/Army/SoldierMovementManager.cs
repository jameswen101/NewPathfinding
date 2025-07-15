using System.Collections.Generic;
using UnityEngine;

public class SoldierMovementManager : MonoBehaviour
{
    private SoldierUnit selectedSoldier;      // source
    private Highlightable selectedHighlight;  // highlight for source

    private Highlightable targetHighlight;    // highlight for target

    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathFinder pathfinder;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick();
    }

    void HandleClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clicked = hit.collider.gameObject;

            // If clicking a soldier
            if (clicked.TryGetComponent<SoldierUnit>(out var soldier))
            {
                // If no soldier selected yet, this becomes the source
                if (selectedSoldier == null)
                {
                    selectedSoldier = soldier;

                    if (selectedHighlight != null)
                        selectedHighlight.SetHighlight(false);

                    selectedHighlight = soldier.GetComponent<Highlightable>();
                    if (selectedHighlight != null)
                        selectedHighlight.SetHighlight(true);
                }
                else
                {
                    // This becomes the target soldier
                    if (targetHighlight != null)
                        targetHighlight.SetHighlight(false);

                    targetHighlight = soldier.GetComponent<Highlightable>();
                    if (targetHighlight != null)
                        targetHighlight.SetHighlight(true);

                    // Move to target
                    MoveTo(clicked.transform.position);
                }
            }
            // If clicking a building
            else if (clicked.CompareTag("Building")) 
            {
                if (targetHighlight != null)
                    targetHighlight.SetHighlight(false);

                targetHighlight = clicked.GetComponent<Highlightable>();
                if (targetHighlight != null)
                    targetHighlight.SetHighlight(true);

                if (selectedSoldier != null)
                {
                    MoveTo(clicked.transform.position);
                }
            }
            // If clicking a machine
            else if (clicked.CompareTag("Machine")) 
            {
                if (targetHighlight != null)
                    targetHighlight.SetHighlight(false);

                targetHighlight = clicked.GetComponent<Highlightable>();
                if (targetHighlight != null)
                    targetHighlight.SetHighlight(true);

                if (selectedSoldier != null)
                {
                    MoveTo(clicked.transform.position);
                }
            }
            else
            {
                // Clicking ground, just move
                if (selectedSoldier != null) //make sure system knows what soldier is called
                {
                    MoveTo(hit.point);
                }
            }
        }
    }

    void MoveTo(Vector3 destination)
    {
        Vector3 start = selectedSoldier.transform.position;
        List<Vector3> path = pathfinder.CalculatePath(
            gridManager.GetNodeFromWorldPosition(start),
            gridManager.GetNodeFromWorldPosition(destination));

        selectedSoldier.MoveAlongPath(path);

        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.ToArray());

        // Clear highlights
        if (selectedHighlight != null)
            selectedHighlight.SetHighlight(false);
        if (targetHighlight != null)
            targetHighlight.SetHighlight(false);

        selectedSoldier = null;
        selectedHighlight = null;
        targetHighlight = null;
    }
}
