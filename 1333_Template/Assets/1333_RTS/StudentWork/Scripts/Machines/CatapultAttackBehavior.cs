using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultAttackBehavior : MachineAttackBehavior
{
    public float Damage = 100f;
    public int MinRange = 20;
    public int MaxRange = 50;

    public LineRenderer lineRenderer;
    public float lineDuration = 0.5f;

    public override void TryAttack(Targetable target)
    {
        float distance = Vector3.Distance(transform.position, target.GetPosition());
        if (distance >= MinRange && distance <= MaxRange)
        {
            Debug.Log($"Ballista attacks target at {distance:F1} units.");
            target.TakeDamage(Damage);
            ShowAttackLine(target.GetPosition());
        }
        else
        {
            Debug.Log("Ballista: Target out of range.");
        }
    }

    private void ShowAttackLine(Vector3 targetPosition)
    {
        if (lineRenderer == null)
        {
            Debug.LogWarning("LineRenderer not assigned.");
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPosition);

        CancelInvoke(nameof(HideLine));
        Invoke(nameof(HideLine), lineDuration);
    }

    private void HideLine()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }
}
