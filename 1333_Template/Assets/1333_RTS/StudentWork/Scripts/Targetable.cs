using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Targetable
{
    Vector3 GetPosition();
    void TakeDamage(float amount);
}

