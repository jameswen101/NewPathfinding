using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierUnit : MonoBehaviour //controls solider movement
{
    public void MoveAlongPath(List<Vector3> path)
    {
        StartCoroutine(FollowPath(path));
    }

    public IEnumerator FollowPath(List<Vector3> path)
    {
        int count = path.Count;
        for (int i = 0; i < count; i++)
        {
            // Only go to the second‑last node, if path length >= 3
            Vector3 point = (i == count - 1 && count >= 3)
                ? path[count - 2]
                : path[i];

            //if path = 2 -> go to the very end

            while (Vector3.Distance(transform.position, point) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    point, 5f * Time.deltaTime);
                yield return null;
            }

            // Stop early
            if (i == count - 2)
                yield break;
        }
    }

}
