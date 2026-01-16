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

    // Items
    public SerializedDictionary<string, Vector3> itemsPosition;
    public SerializedDictionary<string, Quaternion> itemsRotation;

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

        // Items
        itemsPosition = new SerializedDictionary<string, Vector3>();
        itemsRotation = new SerializedDictionary<string, Quaternion>();

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

    //TODO - Créer des fonction publiques pour récupérer les données
}
