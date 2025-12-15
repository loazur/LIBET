using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class S_DataPersistanceManager : MonoBehaviour
{
    public static S_DataPersistanceManager instance {get; private set;}

    [Header("Configuration du stockage en fichier")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    //~ Réferences
    private S_GameData gameData;
    private List<SI_DataPersistance> dataPersistanceObjects;
    private S_FileDataHandler dataHandler;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        dataHandler = new S_FileDataHandler(Application.persistentDataPath, fileName, useEncryption); 
        dataPersistanceObjects = FindAllPersistanceObjects();
        LoadGame(); // Charge la partie au démarrage
    }

    private void OnApplicationQuit() //& Ce lance lorsque l'application s'arrète
    {
        SaveGame();
    }

    //!-----------------------------------------

    public void NewGame()
    {
        // Charger les position/rotation par défaut
        gameData = new S_GameData();

        // Sauvegarder les positions/rotations actuelles de TOUS les objets persistants de la scène
        foreach(SI_DataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.SaveData(ref gameData);
        }

    }

    public void LoadGame()
    {
        // Load any data using the data handler
        gameData = dataHandler.Load();


        // if no data load a new game
        if (gameData == null)
        {
            NewGame();
            return;
        }

        // push the loaded data to all other scripts that need it
        foreach(SI_DataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        // pass the data to other scripts so they can use it
        foreach(SI_DataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.SaveData(ref gameData);
        }
        

        // Save that data to a file using the data handler
        dataHandler.Save(gameData);
    }



    private List<SI_DataPersistance> FindAllPersistanceObjects()
    {
        SI_DataPersistance[] dataPersistanceObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<SI_DataPersistance>()
            .ToArray();

        return new List<SI_DataPersistance>(dataPersistanceObjects);
    }


    
}
