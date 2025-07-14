using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform pivotTransform; // << reference to the pivot
    [SerializeField] private Slider slider;

    private IHasHealth targetHealth;

    public void Initialize(Transform target, IHasHealth healthSource, Camera camera)
    {
        targetTransform = target;
        targetHealth = healthSource;
        mainCamera = camera;
    }

    private void Update()
    {
        if (targetTransform == null) return;

        // Keep the bar at a fixed world position above the target
        transform.position = targetTransform.position + Vector3.up * 2f;

        // Rotate only the pivot to face camera
        pivotTransform.rotation = Quaternion.LookRotation(
            pivotTransform.position - mainCamera.transform.position
        );

        if (slider != null && targetHealth != null)
        {
            slider.value = targetHealth.CurrentHealth / targetHealth.MaxHealth;
        }
    }
}
