using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    private string selectedProfileId = "test";

    void Awake()
    {
        if (instance == null) // Si aucune instance
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            dataHandler = new S_FileDataHandler(Application.persistentDataPath, fileName, useEncryption); 
        }
        else // Si une instance est déjà présente
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
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
        gameData = dataHandler.Load(selectedProfileId);

        // if no data load do nothing
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
        // no data to save
        if (gameData == null)
        {
            return;
        }

        // pass the data to other scripts so they can use it
        foreach(SI_DataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.SaveData(ref gameData);
        }
        
        // Save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileId);
    }

     public void DeleteSaveData()
    {
        dataHandler.Delete(selectedProfileId);
        gameData = null;
        Debug.Log("Sauvegarde supprimée");
    }

    private List<SI_DataPersistance> FindAllPersistanceObjects()
    {
        SI_DataPersistance[] dataPersistanceObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<SI_DataPersistance>()
            .ToArray();

        return new List<SI_DataPersistance>(dataPersistanceObjects);
    }

    
    public bool HasGameData()
    {
        return dataHandler.Load(selectedProfileId) != null;
    }

    public Dictionary<string, S_GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }
    
}
