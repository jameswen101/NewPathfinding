using UnityEngine;
using UnityEngine.SceneManagement;


public class BuildingInstance : BuildingBase, IHasHealth
{
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    public HealthBar healthBar;

    [SerializeField] public ArmyData Army { get; set; }

    [SerializeField] public int ArmyID { get; set; }

    [SerializeField] private AudioManager audioManager;

    void Awake()
    {
        //MaxHealth = BuildingData.maxHealth;
        //CurrentHealth = BuildingData.currentHealth;
        //IsDead = false;
        //// Assign the building to the owning army
        //if (OwningArmy != null)
        //{
        //    OwningArmy.Buildings.Add(this);
        //    ArmyID = OwningArmy.ArmyID;
        //    Debug.Log($"{OwningArmy.name} has {OwningArmy.Buildings.Count} buildings.");
        //}
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
            if (audioManager == null)
            {
                Debug.LogError("AudioManager not found in the scene!");
            }
        }
    }

    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>(); //automatically finds the health bar child instance attached to the spawned prefab
        audioManager = FindObjectOfType<AudioManager>();
    }

    public override void Initialize(BuildingData buildingData, Vector2Int origin)
    {
        BuildingData = buildingData; // this stores it in the field
        Origin = origin;
        CurrentHp = buildingData.currentHealth;
    }
    public void TakeDamage(int incomingDamage)
    {
        CurrentHealth -= incomingDamage;
        healthBar.UpdateHealthBar(CurrentHealth, MaxHealth);
        Debug.Log($"{BuildingData.buildingName} took {incomingDamage} damage.");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        IsDead = true;
        //animator.SetTrigger("Die");
        // Play explosion SFX
        audioManager.PlaySFX("Explosion");
        // play particle FX
        // Disable the unit after a short delay
        Destroy(gameObject, 2f);
        if (OwningArmy != null)
        {
            OwningArmy.Buildings.Remove(this);
            Debug.Log($"{OwningArmy.name} has {OwningArmy.Buildings.Count} units remaining.");
            if (!OwningArmy.IsPlayerControlled && OwningArmy.Buildings.Count == 0)
            {
                Debug.Log("Player Wins!");
                // You can trigger a victory UI here:
                SceneManager.LoadScene("WinScreen");
            }

        }

    }
}


