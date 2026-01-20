using UnityEngine;

public interface SI_DataPersistance
{
    void LoadData(S_GameData gameData);
    void SaveData(S_GameData gameData);

    // AJOUT: Ordre de chargement (plus petit = chargé en premier)
    int GetLoadPriority() => 0; // Valeur par défaut
}
