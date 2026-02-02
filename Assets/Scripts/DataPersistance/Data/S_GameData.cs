using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class S_GameData 
{
    public long lastUpdated;

    //~ Données à sauvegarder
    // Player
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public Vector3 cameraRotation;

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

    // Keys
    public SerializedDictionary<string, List<string>> collectedKeys;

    // Drawers / Cupboards / Padlock
    public SerializedDictionary<string, bool> unlockedDrawers;
    public SerializedDictionary<string, bool> unlockedCupboards;
    public SerializedDictionary<string, bool> unlockedPadlocks;

    // Items (avec le tag CD)
    public SerializedDictionary<string, Vector3> cdsPositions;
    public SerializedDictionary<string, Quaternion> cdsRotations;
    

    //& Constructeurs -> Contient les valeurs initiales
    public S_GameData()
    {
        // Player
        playerPosition = Vector3.zero;
        playerRotation = Quaternion.identity;
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

        // Keys
        collectedKeys = new SerializedDictionary<string, List<string>>();

        // Drawers / Cupboards / Padlock
        unlockedDrawers = new SerializedDictionary<string, bool>();
        unlockedCupboards = new SerializedDictionary<string, bool>();
        unlockedPadlocks = new SerializedDictionary<string, bool>();

        // Items (avec le tag CD)
        cdsPositions = new SerializedDictionary<string, Vector3>();
        cdsRotations = new SerializedDictionary<string, Quaternion>();
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
