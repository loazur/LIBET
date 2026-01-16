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

    private string selectedProfileId = "";

    void Awake()
    {
        if (instance == null) // Si aucune instance
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            dataHandler = new S_FileDataHandler(Application.persistentDataPath, fileName, useEncryption); 
            selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
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

    /*
    private void OnApplicationQuit() //& Ce lance lorsque l'application s'arrète (crash)
    {
        // Ne sauvegarder que si on n'est PAS dans le MainMenu
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        if (currentSceneName != "MainMenu")
        {
            SaveGame();
        }
    }
    */

    //!-----------------------------------------

    public void ChangeSelectedProfileId(string newProfileId)
    {
        // Met à jour le profile id à utilisé pour la sauvegarde et le chargement
        selectedProfileId = newProfileId;

        LoadGame();
    }

    public void NewGame()
    {
        // Charger les position/rotation par défaut
        gameData = new S_GameData();

        // Sauvegarder les positions/rotations actuelles de TOUS les objets persistants de la scène
        foreach(SI_DataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.SaveData(gameData);
        }

        // Crée un timestamp pour savoir quand ça a été sauvegardé pour la derniere fois
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

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

        // ✅ Trier par priorité avant de charger
        var sortedObjects = dataPersistanceObjects
            .OrderBy(obj => obj.GetLoadPriority())
            .ToList();

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
            dataPersistanceObject.SaveData(gameData);
        }
        
        // Save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileId);
    }

     public void DeleteProfileData(string profileId)
    {
        dataHandler.Delete(profileId);
        gameData = null;
        Debug.Log("Sauvegarde supprimée du profil: " + profileId);

        LoadGame();
    }

    private List<SI_DataPersistance> FindAllPersistanceObjects()
    {
        SI_DataPersistance[] dataPersistanceObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
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
