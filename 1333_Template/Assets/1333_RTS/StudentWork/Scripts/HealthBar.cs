using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0); // Adjust height above object

    private Transform targetTransform;
    private IHasHealth targetHealth; // interface or script holding health

    public void Initialize(Transform target, IHasHealth healthSource)
    {
        targetTransform = target;
        targetHealth = healthSource;
    }

    void Update()
    {
        if (targetTransform == null || targetHealth == null)
        {
            Destroy(gameObject);
            return;
        }

        // Follow target
        transform.position = targetTransform.position + worldOffset;

        // Update fill
        float fill = Mathf.Clamp01(targetHealth.CurrentHealth / targetHealth.MaxHealth);
        fillImage.fillAmount = fill;

        transform.forward = Camera.main.transform.forward;

    }
}
