using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierMovementManager : MonoBehaviour
{
    private SoldierUnit selectedSoldier;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathFinder pathfinder; // your A* class
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) HandleClick();
    }

    void HandleClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject target = hit.collider.gameObject;

            if (target.TryGetComponent<SoldierUnit>(out var soldier))
            {
                selectedSoldier = soldier;
            }
            else if (selectedSoldier != null)
            {
                Vector3 start = selectedSoldier.transform.position;
                Vector3 end = hit.point;

                List<Vector3> path = pathfinder.CalculatePath(gridManager.GetNodeFromWorldPosition(start), gridManager.GetNodeFromWorldPosition(end));
                selectedSoldier.MoveAlongPath(path);

                // Visualize path
                lineRenderer.positionCount = path.Count;
                lineRenderer.SetPositions(path.ToArray());

                selectedSoldier = null;
            }
        }
    }
}

