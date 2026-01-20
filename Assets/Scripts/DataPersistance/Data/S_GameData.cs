using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class S_GameData 
{
    public long lastUpdated;

    //~ Données à sauvegarder
    // Joueur
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public Vector3 cameraRotation;
    public bool isCrouching;

    // PlayTime
    public float playTime;

    // Infos des jours
    public int currentDay;
    public float timeLasted;
    public bool isDayActive;
    public int medicinesOfCurrentDay;

    // Notes
    public SerializedDictionary<string, S_Note> notesObtained;

    //Lucidity / Quests
    public float lucidityJauge;
    public SerializedDictionary<string, S_Quest> quests;
    public SerializedDictionary<string, SO_QuestInfo> questsOfTheDay;
    

    //& Constructeurs -> Contient les valeurs initiales
    public S_GameData()
    {
        // Joueur
        playerPosition = Vector3.zero;
        playerRotation = Quaternion.identity;
        isCrouching = false;
        cameraRotation = Vector3.zero;

        // PlayTime
        playTime = 0f;

        // Infos des jours
        currentDay = 0;
        timeLasted = 0f;
        isDayActive = false;
        medicinesOfCurrentDay = 0;

        // Notes
        notesObtained = new SerializedDictionary<string, S_Note>();

        //Lucidity / Quests
        lucidityJauge = 0f;
        quests = new SerializedDictionary<string, S_Quest>();
        questsOfTheDay = new SerializedDictionary<string, SO_QuestInfo>();
    }

    public string getPlayTime()
    {
        int hours = Mathf.FloorToInt(playTime / 3600f);
        int minutes = Mathf.FloorToInt(playTime % 3600f / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);

        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    public int getCurrentDay()
    {
        return currentDay;
    }
}
