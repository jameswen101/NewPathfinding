using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BuildingInstance : BuildingBase, IHasHealth
{
    public float CurrentHealth { get; private set; } //Same as parent class' current health, but with a different name to avoid confusion
    public float MaxHealth { get; private set; }

    public HealthBar healthBar;

    [SerializeField] public ArmyData Army { get; set; } //Same as parent class' owning army, but with a different name to avoid confusion

    [SerializeField] public int ArmyID { get; set; }

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathFinder pathFinder;

    public int XStart { get; private set; }
    public int ZStart { get; private set; }
    public int XEnd { get; private set; }
    public int ZEnd { get; private set; }


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

    public override void Initialize(BuildingData buildingData, Vector2Int origin, GridManager gridManager, PathFinder pathFinder)
    {
        BuildingData = buildingData; // this stores it in the field
        Origin = origin;
        this.gridManager = gridManager;
        this.pathFinder = pathFinder;
        CurrentHp = buildingData.currentHealth;
        MaxHealth = buildingData.maxHealth;
        if (gridManager == null)
        {
            Debug.LogError("GridManager is not assigned in BuildingInstance.");
        }
        if (pathFinder == null)
        {
            Debug.LogError("PathFinder is not assigned in BuildingInstance.");
        }
        healthBar.Initialize(this.transform, this, Camera.main, new Vector3(0, 2f, 0));
        healthBar.SetHealthText(CurrentHealth, MaxHealth); // Set initial health text
        healthBar.UpdateHealthBar(CurrentHealth, MaxHealth); // Update health bar UI
        Debug.Log($"{name}'s starting health: {CurrentHealth}/{MaxHealth}");
        Debug.Log($"{healthBar.healthText.text}");
    }

    public void FindBounds()
    {
        // 1. Collider
        var col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"{name}: No Collider found in FindBounds()");
            return;
        }

        // 2. gridManager
        if (gridManager == null)
        {
            Debug.LogError($"{name}: gridManager is null in FindBounds()");
            return;
        }

        // 3. pathFinder
        if (pathFinder == null)
        {
            Debug.LogError($"{name}: pathFinder is null in FindBounds()");
            return;
        }

        Bounds b = col.bounds;
        Vector3 min = b.min;
        Vector3 max = b.max;

        XStart = gridManager.WorldToGridX(min.x);
        ZStart = gridManager.WorldToGridZ(min.z);
        XEnd = gridManager.WorldToGridX(max.x);
        ZEnd = gridManager.WorldToGridZ(max.z);
        Debug.Log($"Building bounds: ({XStart}, {ZStart}) to ({XEnd}, {ZEnd})");
        pathFinder.UpdateNodeWalkability(XStart, ZStart, XEnd, ZEnd, false);
        Debug.Log($"Gridnodes from ({XStart}, {ZStart}) to ({XEnd}, {ZEnd}) are now unwalkable.");

    }

    public void TakeDamage(int incomingDamage)
    {
        CurrentHealth -= incomingDamage;
        healthBar.UpdateHealthBar(CurrentHealth, MaxHealth);
        healthBar.SetHealthText(CurrentHealth, MaxHealth); // Update the health text display
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
        pathFinder.UpdateNodeWalkability(XStart, ZStart, XEnd, ZEnd, true);
        Debug.Log($"Gridnodes from ({XStart}, {ZStart}) to ({XEnd}, {ZEnd}) are now walkable.");
        if (OwningArmy != null)
        {
            OwningArmy.Buildings.Remove(this);
            OwningArmy.CheckFinalWave();
            Debug.Log($"{OwningArmy.name} has {OwningArmy.Buildings.Count} units remaining.");
            if (!OwningArmy.IsPlayerControlled && (!OwningArmy.Buildings.Any(b => b.name.Contains("Castle"))))
            {
                Debug.Log("Castle destroyed");
                Win();
            }

        }
        else
        {
            Debug.LogWarning($"OwningArmy is null, cannot remove {Data.name}.");
        }

    }

    public void Win()
    {
        Debug.Log("Player Wins!");
        // You can trigger a victory UI here:
        SceneManager.LoadScene("WinScreen");
    }
}


