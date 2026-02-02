using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_DaysManager : MonoBehaviour, SI_DataPersistance
{
    //! S_DaysManager gère le gameplay général, le changement de jour etc...
    public static S_DaysManager instance { get; private set; }

    //~ Information du système de jours
    [Header("Information du système de jours")]
    [SerializeField] private S_PlayerController player; // Joueur
    [SerializeField] private Transform spawnPoint; // Le spawn de Libet chaque jour
    [SerializeField] private float percentageLucidityJaugeAward = 15; // Pourcentage récupérer de jauge de lucidité en pourcentage
    [SerializeField] private int maxDays = 15; // Jours max pour atteindre la fin du jeu
    [SerializeField] private float transitionScreenDuration = 2f;
    [SerializeField] private string[] lores;
    // [SerializeField] private S_KeyUnderDoor keyUnderDoor; // Reference to the key under door object
    

    //~ Génération des médicaments
    [Header("Gestion de la génération des médicaments")]
    [Range(1, 10)]
    [SerializeField] private int medicinesPerDay; //! Rajouter le nombre de spawnPoint équivalent

    //~ Information du jour actuel
    private int currentDay = 1; // Jour actuel par défaut 1
    private bool isDayActive = false; // Jour actif ou non

    //~ Actions
    public event Action OnDayEnd;
    public event Action OnDayLost; // Event quand le joueur perd un jour

    void Awake() //& Création du manager
    {
        Debug.Log($"[DaysManager] Awake appelé sur {gameObject.name}");
        
        if (instance == null)
        {
            instance = this;
            Debug.Log("[DaysManager] Instance créée avec succès");
        }
        else
        {
            Debug.LogWarning($"[DaysManager] Instance déjà existante! Destruction de {gameObject.name}");
            Destroy(gameObject);
            return;
        }

    }

    void Start() //& Initialize le 1er jour
    {
        // Assignement des events
        if (S_AlzheimerEventsManager.instance != null)
        {
            S_AlzheimerEventsManager.instance.OnLucidityZero += OnLucidityReachedZero;
            Debug.Log("[DaysManager] Abonné à OnLucidityZero");
        }
        else
        {
            Debug.LogWarning("[DaysManager] S_AlzheimerEventsManager.instance est NULL dans Awake!");
        }

        if (S_DaysTransitionScreen.instance != null)
        {
            S_DaysTransitionScreen.instance.OnTransitionScreenEnd += StartDay;
            Debug.Log("[DaysManager] Abonné à OnTransitionScreenEnd");
        }
        else
        {
            Debug.LogWarning("[DaysManager] S_DaysTransitionScreen.instance est NULL dans Awake!");
        }

        if (S_DayNightManager.instance != null)
        {
            S_DayNightManager.instance.onDayEnd += TriggerEndDay;
            Debug.Log("[DaysManager] Abonné à EndDay");
        }
        else
        {
            Debug.LogWarning("[DaysManager] S_DayNightManager.instance est NULL dans Awake!");
        }


        Debug.Log("===============================================================> Start appelé sur DaysManager");
        Debug.Log($"[DaysManager] Start appelé - enabled: {enabled}, gameObject.activeInHierarchy: {gameObject.activeInHierarchy}");
        
        InitializeFirstDay();


        
    }

    //!---------------- SI_DataPersistance ----------------

    //~ Sauvegarde jour actuel

    public void LoadData(S_GameData gameData)
    {
        currentDay = gameData.currentDay;
        isDayActive = gameData.isDayActive;

        // Génération des médicaments
        S_MedicinesManager.instance.GenerateMedicines(S_MedicinesManager.instance.GetRemainingMedicines(), medicinesPerDay);
    }

    public void SaveData(S_GameData gameData)
    {
        gameData.currentDay = currentDay;
        gameData.isDayActive = isDayActive;
    }

    public int GetLoadPriority() => 100; // Charger en dernier

    //! ---------- Gestion du temps ----------

  
    public void TriggerEndDay() //& Ce lance avant EndDay
    {
        if (AreQuestsDone())
        {
            EndDay();
        }
        else // Les quetes n'ont pas été effectuées
        {
            OnMainQuestsNotCompleted();
        }
    }

    public void EndDay() //& Fin du jour
    {
        isDayActive = false;

        // Stocker les médicaments non mangés AVANT de passer au jour suivant
        S_MedicinesManager.instance.StoreRemainingMedicines();

        OnDayEnd?.Invoke(); // Lance l'event de fin de jour

        Debug.Log($"Jour {currentDay} terminé après {S_DayNightManager.instance.GetCurrentTimeString()} secondes");

        Award(); // Récompense le joueur

        // Vérification de si on peut passer au jour suivant
        if (currentDay < maxDays)
        {
            PrepareNextDay();
        }
        else
        {
            Debug.Log("Fin du jeu atteinte ! Victoire !");
            //TODO Fin du jeu (victoire)
        }
    }

    private void Award() //& Récompense du joueur
    {
        S_AlzheimerEventsManager.instance.RecoverLucidity(percentageLucidityJaugeAward); // Augmente la lucidité

        Debug.Log($"Récompense de {percentageLucidityJaugeAward}% de lucidité");
    }

    //! ---------- Initialisation du premier jour ----------

    private void InitializeFirstDay() //& Initialise le jour 1 avec génération des éléments
    {
        Debug.Log("Initialisation du jour 1");
        
        // Générer les médicaments pour le jour 1
        GenerateMedicines();
        
        // Génération des quetes aléatoire
        GenerateQuests();

        // Démarrer le jour 1
        S_DaysTransitionScreen.instance.TriggerTransitionScreen(currentDay, 
            S_AlzheimerEventsManager.instance.Lucidity,
            S_MedicinesManager.instance.GetStoredMedicines(), 
            lores, 
            transitionScreenDuration);

    }

    private void PrepareNextDay() //& Prépare le jour d'après, gènère tout ce qu'il faut
    {
        SetCurrentDay(currentDay + 1);

        Debug.Log($"Préparation du jour {currentDay}");
        
        // Générer les médicaments en fonction medicinesPerDay
        GenerateMedicines();
    
        // Génération des quetes aléatoire
        GenerateQuests();
        Debug.Log($"<color=green>[DaysManager]</color> Quêtes générées pour le jour {currentDay}");

        // Diminuer le temps de perte de lucidité chaque jours
        float LucidityDecreaseRateAccessor = S_AlzheimerEventsManager.instance.GetLucidityDecreaseRate();
        LucidityDecreaseRateAccessor = LucidityDecreaseRateAccessor + LucidityDecreaseRateAccessor / 4;                                 //! ICI - Ajuster la diminution

        S_AlzheimerEventsManager.instance.SetlucidityDecreaseRate(LucidityDecreaseRateAccessor);


        //! A déplacer dans le HandleHistoryQuest
        
        //TODO Ajouter ICI la logique du 2eme jours (scenatio)
        // Mettre à jour les prefabs de quêtes via S_QuestDayManager
        if (S_QuestDayManager.instance != null)
        {
            S_QuestDayManager.instance.UpdateQuestPrefabsForDay(currentDay);
        }
        

        // On commence le prochain jour
        S_DaysTransitionScreen.instance.TriggerTransitionScreen(currentDay, 
            S_AlzheimerEventsManager.instance.Lucidity,
            S_MedicinesManager.instance.GetStoredMedicines(), 
            lores, 
            transitionScreenDuration);
    }

    //! --------- Génération des médicaments ---------

    private void GenerateMedicines()
    {
        int medicines = UnityEngine.Random.Range(1, medicinesPerDay + 1); // 1 ou 2 médicaments (ou selon la range définie)

        // Passer medicinesPerDay comme paramètre pour éviter la duplication
        S_MedicinesManager.instance.GenerateMedicines(medicines, medicinesPerDay);

        Debug.Log($"Génération de {medicines} nouveaux médicaments pour le jour {currentDay}.");
    }


    //! ---------- Système de perte de jour ----------

    /* Explication 
       Perd 1 jour si:
            - Jauge de lucidité à 0
            - Fin du jour sans avoir effectué toutes les quetes
     */
    public void LoseDay(string reason) //& Perte du jour
    {
        isDayActive = false;

        Debug.LogWarning($"Jour perdu ! Raison: {reason}");

        OnDayLost?.Invoke(); // Lance l'event de perte de jour

        // On recule d'un jour si on est pas au jour 1 (sinon recommance le jour 1)
        if (currentDay != 1)
        {
            SetCurrentDay(currentDay - 1);
            Debug.Log($"Retour au jour {currentDay}");
        }

        RestartCurrentDay();
    }

    private void RestartCurrentDay() //& Réinitialise le jour actuel
    {
        // Régénérer les quêtes
        GenerateQuests();

        // Nettoyer et régénérer les médicaments
        S_MedicinesManager.instance.CleanupForDayRestart();
        GenerateMedicines();

        // Réinitialiser la lucidité à un niveau de base
        S_AlzheimerEventsManager.instance.RecoverLucidity(10000);

        //! Déplacer dans le manager lié
        // Gérer l'état du KeyOnDoorPrefab en fonction du jour
        // if (currentDay >= 2 && keyUnderDoor.isKeyTaken == false)
        // {
        //     S_QuestDayManager.instance.UpdateQuestPrefabsOnRestart(currentDay);
        // }

        // Redémarrer le jour
        S_DaysTransitionScreen.instance.TriggerTransitionScreen(currentDay, 
            S_AlzheimerEventsManager.instance.Lucidity,
            S_MedicinesManager.instance.GetStoredMedicines(), 
            lores, 
            transitionScreenDuration);

        Debug.Log($"Jour {currentDay} réinitialisé");
    }

    public void OnLucidityReachedZero() //& Appellé quand la jauge de lucidité atteint 0
    {
        LoseDay("Jauge de lucidité à 0");
    }

    public void OnMainQuestsNotCompleted() //& Appellé quand les quetes principales incompletes
    {
        LoseDay("Quêtes principales non complétées");
    }

     //! --------- Gestion des quetes ---------

    private void GenerateQuests()
    {
        // Réinitialiser les quêtes du jour précédent
        if (S_LaunchRandomQuest.instance != null)
        {
            S_LaunchRandomQuest.instance.ResetAllRepeatableQuests();
            Debug.Log($"----> [DaysManager] Quêtes du jour précédent réinitialisées.");
            
            // Lancer 3 nouvelles quêtes aléatoires selon la difficulté du jour
            S_LaunchRandomQuest.instance.LaunchRandomQuestsForDay(currentDay);
            
            int difficulty = S_LaunchRandomQuest.instance.GetDifficultyForDay(currentDay);
            Debug.Log($"<color=green>[DaysManager]</color> Quêtes générées pour le jour {currentDay} (Difficulté: {difficulty})");
        }
        else
        {
            Debug.LogWarning("[DaysManager] S_LaunchRandomQuest.instance est null! Les quêtes ne peuvent pas être générées.");
        }
    }


    /**
     * Vérifie si toutes les quêtes du jour sont terminées
     * Utilise le vrai système de quêtes via S_QuestManager
     *
     * @return  bool    True si toutes les quêtes sont terminées
     */
    public bool AreQuestsDone()
    {
        //& Vérifier via le système de quêtes
        if (S_QuestManager.instance != null)
        {
            return S_LaunchRandomQuest.instance.AllQuestCompleted();
        }
        else //& Cas où S_QuestManager n'est pas initialisé
        {
            Debug.LogWarning("[DaysManager] S_QuestManager.instance est null!");
            return false;
        }
    }

    //! ---------- Méthodes publiques ----------

    public void StartDay()
    {
        isDayActive = true;

        // Début du matin
        S_DayNightManager.instance.StartDay();

        // Tp au spawn (avec la bonne orientation)
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
        
        // Sauvegarde
        S_DataPersistanceManager.instance.SaveGame();

        Debug.Log($"Jour {currentDay} commencé");
    }

    public void PauseDay()
    {
        isDayActive = false;
    }

    public void ResumeDay()
    {
        isDayActive = true;
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    private void SetCurrentDay(int newDay)
    {
        // Gestion des erreurs
        if (newDay == currentDay)
        {
            Debug.LogWarning("Erreur: NewDay équivaut à CurrentDay");
            return;
        }
        else if (newDay > maxDays)
        {
            Debug.LogWarning("Erreur: NewDay dépasse la limite MaxDays");
            return;
        }
        else if (newDay < 1)
        {
            Debug.LogWarning("Erreur: NewDay ne peut pas être inférieur à 1");
            return;
        }

        currentDay = newDay;
    }

    //& Events pour voir si on a atteint le jour 2
    public bool IsDay2Reached()
    {
        return currentDay >= 2;
    }

    #region  DEBUG

    [ContextMenu("DEBUG - Forcer fin de jour")]
    private void Debug_ForceEndDay()
    {
        EndDay();
    }

    [ContextMenu("DEBUG - Forcer perte de jour")]
    private void Debug_ForceLoseDay()
    {
        LoseDay("Debug - Forcé");
    }

    [ContextMenu("Mettre jour 2")]
    private void Debug_SetDay2()
    {
        SetCurrentDay(2);
    }

    [ContextMenu("Show current day info")]
    private void Debug_ShowCurrentDayInfo()
    {
        Debug.Log($"<color=yellow>[DaysManager DEBUG]</color> Jour actuel: {currentDay}, Temps écoulé: ..., Jour actif: {isDayActive}");
    }




    #endregion
}

