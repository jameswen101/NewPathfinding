using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasHealth
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
}

