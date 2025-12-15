using UnityEngine;

public interface SI_DataPersistance
{
    void LoadData(S_GameData gameData);
    void SaveData(ref S_GameData gameData);
}
