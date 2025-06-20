using System;
using UnityEngine;


    public abstract class BuildingBase : MonoBehaviour
    {
        [SerializeField] protected BuildingTypes BuildingType;
        [SerializeField] protected int CurrentHp;
        [SerializeField] protected Vector2Int Origin;
        protected IArmyData OwningArmy;

        public BuildingTypes Type => BuildingType;

        public int Hp => CurrentHp;
        public int ArmyId => OwningArmy.ArmyID;
        public Vector2Int OriginPoint => Origin;

        public abstract void Initialize(BuildingData buildingData, Vector2Int origin);

        public IArmyData ParentArmy => OwningArmy;

        public virtual void AssignToArmy(IArmyData army)
        {
            OwningArmy = army;
        }

        public virtual void OnDestroy()
        {
            ParentArmy.RemoveBuilding(this);
        }
    }
