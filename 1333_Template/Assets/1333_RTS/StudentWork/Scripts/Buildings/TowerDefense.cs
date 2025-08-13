using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TowerDefense : MonoBehaviour
{
    public int collisionDamage = 10;
    [SerializeField] private float damageCooldown = 0.5f;
    private float nextHitTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < nextHitTime) return;
        //if (!other.TryGetComponent<IDamageable>(out var damageable)) return;

        //damageable.TakeDamage(collisionDamage);
        nextHitTime = Time.time + damageCooldown;
    }
}

