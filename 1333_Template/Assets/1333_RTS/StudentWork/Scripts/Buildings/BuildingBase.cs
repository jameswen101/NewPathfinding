using System;
using UnityEngine;


    public abstract class BuildingBase : MonoBehaviour
    {
        [SerializeField] protected BuildingData BuildingData;
        [SerializeField] protected int CurrentHp;
        [SerializeField] protected Vector2Int Origin;
        protected IArmyData OwningArmy;

        public BuildingData Data => BuildingData;

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
        if (ParentArmy != null)
            ParentArmy.RemoveBuilding(this);
    }

}
