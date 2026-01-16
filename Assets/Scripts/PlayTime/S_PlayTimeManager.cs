using UnityEngine;

public class S_PlayTimeManager : MonoBehaviour, SI_DataPersistance
{
    //! S_TimePlayedManager gère le temps écoulé depuis que le joueur a créé la save.
    
    public static S_PlayTimeManager instance;

    private float playTime = 0f;
    private bool isTracking = false; // Contrôle si on compte le temps

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        //  Compte le temps seulement si activé
        if (isTracking)
        {
            playTime += Time.deltaTime;
        }
    }

    //!---------------- PUBLIC METHODS ----------------

    public void StartTracking() //& Démarre le comptage du temps
    {
        isTracking = true;
        Debug.Log($"<color=cyan>[TimePlayedManager]</color> Comptage démarré à {GetFormattedTime()}");
    }

    public void StopTracking() //& Arrête le comptage du temps
    {
        isTracking = false;
    }

    /// <summary>
    /// Retourne le temps total de jeu en secondes
    /// </summary>
    public float GetTotalTimeInSeconds()
    {
        return playTime;
    }

    /// <summary>
    /// Retourne le temps formaté en HH:MM:SS
    /// </summary>
    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(playTime / 3600f);
        int minutes = Mathf.FloorToInt((playTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);

        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    /// <summary>
    /// Retourne les heures, minutes et secondes séparément
    /// </summary>
    public (int hours, int minutes, int seconds) GetTimeComponents()
    {
        int hours = Mathf.FloorToInt(playTime / 3600f);
        int minutes = Mathf.FloorToInt((playTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);

        return (hours, minutes, seconds);
    }

    //!---------------- SI_DataPersistance ----------------

    public int GetLoadPriority() => -200; // Charger AVANT tout le monde

    public void LoadData(S_GameData gameData)
    {
        // Charge le temps sauvegardé
        playTime = gameData.playTime;
        
        Debug.Log($"<color=green>[TimePlayedManager]</color> Temps chargé: {GetFormattedTime()}");
    }

    public void SaveData(S_GameData gameData)
    {
        // Sauvegarde le temps actuel
        gameData.playTime = playTime;
        
        Debug.Log($"<color=yellow>[TimePlayedManager]</color> Temps sauvegardé: {GetFormattedTime()}");
    }
}
