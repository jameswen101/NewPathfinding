using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;
    //[SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0); // Adjust height above object
    [SerializeField] private Slider slider;

    private Transform targetTransform;
    private IHasHealth targetHealth; // interface or script holding health

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Initialize(Transform target, IHasHealth healthSource, Camera mainCamera)
    {
        targetTransform = target;
        targetHealth = healthSource;
        this.mainCamera = mainCamera;

        // Optionally, initialize the slider
        if (slider != null)
        {
            slider.maxValue = targetHealth.MaxHealth;
            slider.value = targetHealth.CurrentHealth;
        }
    }

    void Update()
    {
        if (targetTransform == null || targetHealth == null)
        {
            Destroy(gameObject);
            return;
        }

        // Follow target
        transform.position = mainCamera.transform.position;


        // Update fill
        //float fill = Mathf.Clamp01(targetHealth.CurrentHealth / targetHealth.MaxHealth);
        //fillImage.fillAmount = fill;

        //transform.forward = Camera.main.transform.forward;

    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue/maxValue;
    }
}
