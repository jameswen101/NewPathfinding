using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string sceneName;
    public int waveNumber;
    public List<UnitSaveData> playerUnits;
    public List<UnitSaveData> enemyUnits;
    public List<BuildingSaveData> playerBuildings;
    public List<BuildingSaveData> enemyBuildings;
}
