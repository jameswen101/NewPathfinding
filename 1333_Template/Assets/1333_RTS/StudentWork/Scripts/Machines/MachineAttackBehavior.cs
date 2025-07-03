using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MachineAttackBehavior : MonoBehaviour
{
    public abstract void TryAttack(Targetable target);
}

