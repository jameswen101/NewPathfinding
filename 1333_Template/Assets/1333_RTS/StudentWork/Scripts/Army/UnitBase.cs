using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField] protected UnitType unitType;
    public int Width => unitType != null ? unitType.Width : 1;
    public int Height => unitType != null ? unitType.Height : 1;
    public int Damage => unitType != null ? unitType.Damage : 0;
    public int Defence => unitType != null ? unitType.Defence : 0;
    public string UnitName => unitType != null ? unitType.unitTypeName : "Unknown";

    public float CurrentHealth { get; protected set; }
    public float MaxHealth { get; protected set; }
    public bool IsDead { get; protected set; }

    [SerializeField] protected HealthBar healthBar;

    public void TakeDamage(int incomingDamage)
    {
        int mitigated = Mathf.Max(incomingDamage - Defence, 1); //no need to say UnitType.Defence
        CurrentHealth -= mitigated;
        healthBar.UpdateHealthBar(CurrentHealth, MaxHealth);
        Debug.Log($"{UnitName} took {mitigated} damage (after {Defence} defence).");  //no need to say UnitType.unitTypeName

        if (CurrentHealth <= 0)
        {
            IsDead = true;
        }
    }

    public abstract void MoveTo(GridNode targetNode);
    protected UnitState State;
    public virtual void Tick()
    {
        switch (State)
        {
            case UnitState.Moving:
                //DoMove();
                break;
            case UnitState.Attacking:
                break;
                //case 
        }

    }

    protected virtual void Awake()
    {
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<HealthBar>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
