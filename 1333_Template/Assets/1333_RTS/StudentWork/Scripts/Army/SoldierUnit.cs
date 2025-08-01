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
        Debug.Log($"FollowPath called, path length: {count}");

        if (path == null || count <= 2)
        {
            Debug.Log("Skipping movement — path too short or null");
            yield break;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 point = path[i];
            Debug.Log($"Moving to path[{i}] = {point}");

            while (Vector3.Distance(transform.position, point) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, point, 5f * Time.deltaTime);
                yield return null;
            }
        }
    }

}
