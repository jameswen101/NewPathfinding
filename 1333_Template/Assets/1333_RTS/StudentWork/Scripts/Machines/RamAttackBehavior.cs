using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamAttackBehavior : MachineAttackBehavior
{
    public float Damage = 200f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Targetable>(out var target))
        {
            Debug.Log("Ram collided with target! Dealing damage.");
            target.TakeDamage(Damage);
        }
    }

    public override void TryAttack(Targetable target)
    {
        Debug.Log("Rams cannot attack at a distance. Move them into the target.");
    }
}

