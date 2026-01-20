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
        if (scene.name == "MainMenu")
            return;

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
    }

    public void NewGame()
    {
        gameData = new S_GameData();
        
        Debug.Log("Nouvelle partie créée");
        
        // Démarre le comptage pour une nouvelle partie
        if (S_PlayTimeManager.instance != null)
        {
            S_PlayTimeManager.instance.StartTracking();
        }
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load(selectedProfileId);

        if (gameData == null)
        {
            Debug.Log("Aucune donnée trouvée, nouvelle partie initialisée");
            NewGame();
            return;
        }

        var sortedObjects = dataPersistanceObjects
            .OrderBy(obj => obj.GetLoadPriority())
            .ToList();

        Debug.Log($"<color=cyan>[DataPersistance]</color> Chargement de {sortedObjects.Count} objet(s) par ordre de priorité");

        foreach(SI_DataPersistance dataPersistanceObject in sortedObjects)
        {
            dataPersistanceObject.LoadData(gameData);
        }

        Debug.Log("Donnée chargés depuis le fichier");
        
        // Démarre le comptage du temps après le chargement
        if (S_PlayTimeManager.instance != null)
        {
            S_PlayTimeManager.instance.StartTracking();
        }
    }

    public void SaveGame()
    {
        // no data to save
        if (gameData == null)
        {
            Debug.LogWarning("Aucune donnée de jeu à sauvegarder");
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
