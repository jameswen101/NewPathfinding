using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform pivotTransform; // << reference to the pivot
    [SerializeField] private Slider slider;
    public TextMeshProUGUI healthText; // for displaying health value
    private bool hasInitialized = false;

    private IHasHealth targetHealth;

    public void Initialize(Transform target, IHasHealth healthSource, Camera camera)
    {
        hasInitialized = true;
        targetTransform = target;
        targetHealth = healthSource;
        mainCamera = camera;

        if (targetHealth == null)
        {
            Debug.LogError("Target Health is not assigned in HealthBar.");
            return;
        }

        Debug.Log($"HealthBar initialized with target: {targetTransform.name}");

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

    public void HealthTextDebugLog()
    {
        Debug.Log(healthText.text);
    }
}
