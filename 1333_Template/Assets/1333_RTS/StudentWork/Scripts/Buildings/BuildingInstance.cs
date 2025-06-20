using UnityEngine;


    public class BuildingInstance : BuildingBase
    {
        /// <summary>
        /// Initializes the building with a given BuildingType.
        /// </summary>

        public override void Initialize(BuildingData buildingData, Vector2Int origin)
        {
            BuildingData BD = buildingData; // if you want to store it
            Origin = origin;
            CurrentHp = buildingData.currentHealth;
        }
    }

