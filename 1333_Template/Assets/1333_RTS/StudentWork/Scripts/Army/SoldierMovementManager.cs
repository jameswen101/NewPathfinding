using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoldierMovementManager : MonoBehaviour
{
    private SoldierUnit selectedSoldier;      // source
    //private Highlightable selectedHighlight;  // highlight for source

    //private Highlightable targetHighlight;    // highlight for target

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
        Debug.Log($"HandleClick: mainCamera={(mainCamera == null ? "NULL" : "OK")}, selectedSoldier={(selectedSoldier == null ? "null" : "has value")}");
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (ray.origin == null || ray.direction == null)
        {
            Debug.LogError("Ray origin or direction is null. Cannot proceed with raycasting.");
            return;
        }
        Debug.Log($"Ray origin: {ray.origin}, direction: {ray.direction}");
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
                    Debug.Log($"Selected soldier: {selectedSoldier.name}");

                    //    if (selectedHighlight != null)
                    //        selectedHighlight.SetHighlight(false);

                    //    selectedHighlight = soldier.GetComponent<Highlightable>();
                    //    if (selectedHighlight != null)
                    //        selectedHighlight.SetHighlight(true);
                    //
                }
                else
                {
                    // This becomes the target soldier
                    //if (targetHighlight != null)
                    //    targetHighlight.SetHighlight(false);

                    //targetHighlight = soldier.GetComponent<Highlightable>();
                    //if (targetHighlight != null)
                    //    targetHighlight.SetHighlight(true);

                    // Move to target
                    MoveTo(clicked.transform.position);
                    if (selectedSoldier == null)
                    {
                        Debug.LogError("Selected soldier is null when trying to move.");
                    }
                    if (clicked.name == null)
                    {
                        Debug.LogError("Clicked object name is null when trying to move.");
                    }
                    if (clicked.transform == null)
                    {
                        Debug.LogError("Clicked object transform is null when trying to move.");
                    }
                    Debug.Log($"Moving {selectedSoldier.name} to {clicked.name} at position {clicked.transform.position}");
                }
            }
            // If clicking a building
            else if (clicked.CompareTag("Building"))
            {
                //if (targetHighlight != null)
                //    targetHighlight.SetHighlight(false);

                //targetHighlight = clicked.GetComponent<Highlightable>();
                //if (targetHighlight != null)
                //    targetHighlight.SetHighlight(true);

                if (selectedSoldier != null)
                {
                    MoveTo(clicked.transform.position);
                    Debug.Log($"Moving {selectedSoldier.name} to building at position {clicked.transform.position}");
                }
            }
            // If clicking a machine
            else if (clicked.CompareTag("Machine"))
            {
                //if (targetHighlight != null)
                //    targetHighlight.SetHighlight(false);

                //targetHighlight = clicked.GetComponent<Highlightable>();
                //if (targetHighlight != null)
                //    targetHighlight.SetHighlight(true);

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

    void MoveTo(Vector3 destination, int stopBeforeLast = 3)
    {
        Debug.Log($"Calculating path with stopBeforeLast = {stopBeforeLast}");

        Vector3 start = selectedSoldier.transform.position;
        List<Vector3> fullPath = pathfinder.CalculatePath(
            gridManager.GetNodeFromWorldPosition(start),
            gridManager.GetNodeFromWorldPosition(destination)
        );

        int count = fullPath.Count;
        Debug.Log($"Full path length: {count}");

        if (fullPath == null || count < 3)
        {
            Debug.Log("Path too short (<3 nodes). Movement skipped.");
            return;
        }

        // Decide where to stop: e.g. stopBeforeLast = 2 or 3
        int targetIndex = Mathf.Max(0, count - stopBeforeLast);
        Debug.Log($"Truncated path to stop at index: {targetIndex}");

        List<Vector3> truncated = fullPath.Take(targetIndex + 1).ToList();

        selectedSoldier.MoveAlongPath(truncated);

        lineRenderer.positionCount = truncated.Count;
        lineRenderer.SetPositions(truncated.ToArray());

        selectedSoldier = null;
    }


}
