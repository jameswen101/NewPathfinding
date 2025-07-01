using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierUnit : MonoBehaviour
{
    public void MoveAlongPath(List<Vector3> path)
    {
        StartCoroutine(FollowPath(path));
    }

    private IEnumerator FollowPath(List<Vector3> path)
    {
        foreach (Vector3 point in path)
        {
            while (Vector3.Distance(transform.position, point) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, point, 5f * Time.deltaTime);
                yield return null;
            }
        }
    }
}
