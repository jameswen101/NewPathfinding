using System;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;


    public abstract class BuildingBase : MonoBehaviour
    {
        [SerializeField] protected BuildingData BuildingData;
        [SerializeField] protected int CurrentHp;
        [SerializeField] protected Vector2Int Origin;
        [SerializeField] protected ArmyData OwningArmy;

    public BuildingData Data => BuildingData;

        public int Hp => CurrentHp;
        public int ArmyId => OwningArmy.ArmyID;
        public Vector2Int OriginPoint => Origin;

        public abstract void Initialize(BuildingData buildingData, Vector2Int origin, GridManager gridManager, PathFinder pathFinder, ArmyData armyData);

        public ArmyData ParentArmy => OwningArmy;

    public bool IsDead { get; protected set; }

    public virtual void AssignToArmy(ArmyData army)
        {
            OwningArmy = army;
        }

    public virtual void OnDestroy()
    {
        if (ParentArmy != null)
            ParentArmy.RemoveBuilding(this);
    }

    
}
