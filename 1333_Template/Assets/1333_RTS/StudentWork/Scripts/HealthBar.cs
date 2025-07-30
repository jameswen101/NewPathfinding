using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform pivotTransform; // << reference to the pivot
    [SerializeField] private Slider slider;
    [SerializeField] private Vector3 offset;
    public TextMeshProUGUI healthText; // for displaying health value

    private IHasHealth targetHealth;

    public void Initialize(Transform target, IHasHealth healthSource, Camera camera, Vector3 offset)
    {
        targetTransform = target;
        targetHealth = healthSource;
        mainCamera = camera; //something is wrong with setting mainCamera to be camera?
        this.offset = offset;

        if (targetTransform == null)
        {
            Debug.LogError("Target Transform is not assigned in HealthBar.");
            return;
        }
        else
        {
            Debug.Log($"Target assigned to health bar: {targetTransform.name} ");
        }

        if (targetHealth == null)
        {
            Debug.LogError("Target Health is not assigned in HealthBar.");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned in HealthBar.");
            return;
        }
        else
        {
            Debug.Log($"Camera assigned to health bar: {camera.name} ");
        }

        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>();
            if (slider == null)
            {
                Debug.LogError("Slider component not found in HealthBar.");
            }
        }
    }

    private void Start()
    {
        targetTransform = targetTransform.parent != null ? targetTransform.parent : targetTransform;
        Debug.Log($"Target Transform: {targetTransform.name}");
        // Keep the bar at a fixed world position above the target
        transform.position = targetTransform.position + Vector3.up * 2f;

        if (mainCamera != null)
        {
            // Rotate only the pivot to face camera
            pivotTransform.rotation = Quaternion.LookRotation(
                pivotTransform.position - mainCamera.transform.position
            );
        }
        else
        {
            Debug.LogWarning("Main Camera is not assigned in HealthBar.");
            mainCamera = Camera.main; // Fallback to Camera.main
            Debug.Log($"Main Camera set to: {mainCamera.name}");
        }
    }

    private void Update()
    {
        
    }
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if (slider != null && targetHealth != null)
        {
            slider.value = targetHealth.CurrentHealth / targetHealth.MaxHealth;
            Debug.Log($"HealthBar updated: {currentValue}/{maxValue}");
        }
    }

    public void SetHealthText(float currentValue, float maxValue)
    {
        if (healthText != null)
        {
            healthText.text = $"{currentValue}/{maxValue}";
            Debug.Log(maxValue + " " + currentValue);
        }
        else
        {
            Debug.LogWarning("HealthText is not assigned in HealthBar.");
        }
    }
}
