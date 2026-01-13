// S_QuestManager.cs
// Gère les quêtes dans le jeu

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class S_QuestManager : MonoBehaviour
{
    #region ATTRIBUTS
    // *==========================================================================*
    // *                                 ATTRIBUTS                                *
    // *==========================================================================*
    #endregion

    //~ Singleton
    public static S_QuestManager instance { get; private set; }

    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;
    [SerializeField] private bool resetAllQuestsOnStart = false; // Mettre à true pour réinitialiser toutes les quêtes

    [Header("Interface pour les quêtes")]
    [SerializeField] private GameObject questCanvas; // Canvas pour les quêtes
    [SerializeField] private Text QuestDisplayTitle;
    private Dictionary<string, S_Quest> questMap;

    //~ Quête sélectionnée pour l'affichage dans l'UI des objectifs
    private S_Quest selectedQuestForDisplay;
    
    //~ Quêtes actives du jour (histoire + secondaires)
    private S_Quest storyQuest; // La quête principale/histoire
    private List<S_Quest> dailySideQuests = new List<S_Quest>(); // Les 3 quêtes secondaires du jour

    private int currentPlayerLevel = 1; // Niveau par défaut (sera mis à jour par PlayerLevelChange)
    private bool isSubscribed = false;
    
    //~ Events pour la gestion des jours
    public event System.Action OnDailyQuestsReset;

    #region METHODS MonoBehaviour
    // *==========================================================================*
    // *                                 METHODS                                  *
    // *==========================================================================*
    #endregion

    private void Awake()
    {
        //~ Singleton setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Optionnel : réinitialiser toutes les quêtes en développement
        if (resetAllQuestsOnStart)
        {
            ResetAllQuests();
        }
        
        questMap = CreateQuestMap();
    }

    private void Start()
    {
        StartCoroutine(InitializeWhenReady());
    }

    public void Update()
    {
        // Vérifier si des quêtes peuvent passer de REQUIREMENTS_NOT_MET à CAN_START
        // // TODO: Optimiser en utilisant un système d'événements plutôt que Update()
        foreach (S_Quest quest in questMap.Values)
        {
            if (quest.state == E_QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, E_QuestState.CAN_START);
            }
        }
    }

    #region Event Subscription
    /**
     * Gère l'initialisation une fois que S_GameManager est prêt
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, November 26th, 2025.
     * @access	private
     * @return	void
     */
    private IEnumerator InitializeWhenReady()
    {
        // Attendre que S_GameManager soit initialisé
        while (S_GameManager.instance == null)
        {
            // Debug.Log("[S_QuestManager] En attente de l'initialisation de S_GameManager...");
            yield return null;
        }

        // S'abonner aux événements si pas encore fait
        if (!isSubscribed)
        {
            SubscribeToEvents();
        }

        // Attendre que tous les S_QuestPoint s'abonnent (ils le font dans Start())
        // et que PlayerLevelManager initialise le niveau du joueur
        yield return new WaitForSeconds(0.5f);

        // Notifier l'état initial de toutes les quêtes
        foreach(S_Quest quest in questMap.Values)
        {
            // Debug.Log($"[S_QuestManager] Quête '{quest.info.displayName}' (ID: {quest.info.id}) - État: {quest.state}");
            
            if (quest.state  ==  E_QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            S_GameManager.instance.questEvents.QuestStateChange(quest);
        }

        // Initialiser l'UI avec la quête active
        // Debug.Log("[S_QuestManager] Initialisation de l'UI...");
        UpdateQuestUI();
    }

    /**
     * Abonnement aux événements du GameManager
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, November 26th, 2025.
     * @access	private
     * @return	void
     */
    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed)
        {
            Debug.LogWarning("[S_QuestManager] Impossible de s'abonner : S_GameManager est null ou déjà abonné.");
            return;
        } 

        S_GameManager.instance.questEvents.OnStartQuest += StartQuest;
        S_GameManager.instance.questEvents.OnAdvanceQuest += AdvanceQuest;
        S_GameManager.instance.questEvents.OnFinishQuest += FinishQuest;
        S_GameManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;

        S_GameManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;
        S_GameManager.instance.questEvents.onQuestStateChange += OnQuestStateChanged;
        
        // S'abonner aux événements de menu pour gérer l'affichage du canvas
        S_GameManager.instance.playerEvents.onMenuOpened += HideQuestCanvas;
        S_GameManager.instance.playerEvents.onMenuClosed += ShowQuestCanvas;
        
        isSubscribed = true;
        // Debug.Log("[S_QuestManager] Abonné aux événements du GameManager.");
    }

    /**
     * Désabonnement des événements
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, November 26th, 2025.
     * @access	private
     * @return	void
     */
    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed)
        {
            Debug.LogWarning("[S_QuestManager] Impossible de se désabonner : S_GameManager est null ou déjà désabonné.");
            return;
        } 

        S_GameManager.instance.questEvents.OnStartQuest -= StartQuest;
        S_GameManager.instance.questEvents.OnAdvanceQuest -= AdvanceQuest;
        S_GameManager.instance.questEvents.OnFinishQuest -= FinishQuest;
        S_GameManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;

        S_GameManager.instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;
        S_GameManager.instance.questEvents.onQuestStateChange -= OnQuestStateChanged;
        
        // Se désabonner des événements de menu
        S_GameManager.instance.playerEvents.onMenuOpened -= HideQuestCanvas;
        S_GameManager.instance.playerEvents.onMenuClosed -= ShowQuestCanvas;
        
        isSubscribed = false;
    }

    
    /**
     * Active l'abonnement lors de l'activation
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 29th, 2025.
     * @access	private
     * @return	void
     */
    private void OnEnable()
    {
        // L'abonnement sera géré par InitializeWhenReady() dans Start()
        // pour garantir que S_GameManager est prêt
    }

    /**
     * Désabonnement lors de la désactivation
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 29th, 2025.
     * @access	private
     * @return	void
     */
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /**
     * Reset les quêtes en cours si l'application se ferme brusquement
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, November 26th, 2025.
     * @access	private
     * @return	void
     */
    private void OnApplicationQuit()
    {
        foreach (S_Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    #endregion


    #region QUEST ADVANCEMENT

    /**
     * Gère les changements d'état de quête pour mettre à jour l'UI
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	private
     * @param	s_quest	quest	
     * @return	void
     */
    private void OnQuestStateChanged(S_Quest quest)
    {
        UpdateQuestUI();
    }

    
    /**
     * Change l'état de l'étape d'une quête passée par son ID et son index
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 29th, 2025.
     * @access	private
     * @param	string          	id            	
     * @param	int             	stepIndex     	
     * @param	s_queststepstate	questStepState	
     * @return	void
     */
    private void QuestStepStateChange(string id, int stepIndex, S_QuestStepState questStepState)
    {
        S_Quest quest = GetQuestByID(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
        Debug.Log($"<color=magenta>[QuestManager]</color> État de l'étape {stepIndex} de la quête '{id}' mis à jour.");
    }

    /**
     * Permet de changer l'état d'une quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 23rd, 2025.
     * @access	private
     * @param	string      	id   	
     * @param	e_queststate	state	
     * @return	void
     */
    private void ChangeQuestState(string id, E_QuestState state)
    {
        S_Quest quest = GetQuestByID(id);
        quest.state = state;
        S_GameManager.instance.questEvents.QuestStateChange(quest);
        
    }

    /**
     * Débute une quête donnée par son ID
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 16th, 2025.
     * @access	private
     * @param	string	questID	
     * @return	void
     */
    private void StartQuest(string questID)
    {
        S_Quest quest = GetQuestByID(questID);
        
        // Protection contre les appels multiples - ne démarrer que si CAN_START
        if (quest.state != E_QuestState.CAN_START)
        {
            Debug.LogWarning($"<color=yellow>[QuestManager]</color> StartQuest ignoré pour '{questID}' - état actuel: {quest.state} (attendu: CAN_START)");
            return;
        }
        
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(questID, E_QuestState.IN_PROGRESS);

        Debug.Log($"<color=green>[QuestManager]</color> Quest '{questID}' started with first step");
    }

    /**
     * Avance la quête donnée par son ID
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 16th, 2025.
     * @access	private
     * @param	string	questID	
     * @return	void
     */
    private void AdvanceQuest(string questID)
    {
        S_Quest quest = GetQuestByID(questID);
        
        if (quest == null)
        {
            Debug.LogError("[S_QuestManager] Impossible de faire avancer la quête. Quest ID: " + questID);
            return;
        }

        // Protection: ne pas avancer si la quête n'est pas IN_PROGRESS
        if (quest.state != E_QuestState.IN_PROGRESS)
        {
            Debug.LogWarning($"<color=yellow>[QuestManager]</color> AdvanceQuest ignoré pour '{questID}' - état: {quest.state} (attendu: IN_PROGRESS)");
            return;
        }

        int previousIndex = quest.CurrentStepIndex;
        
        // move on to the next step
        quest.MoveToNextStep();

        Debug.Log($"<color=cyan>[QuestManager]</color> Quest '{questID}' avancée: étape {previousIndex} → {quest.CurrentStepIndex}");

        // if there are more steps, instantiate the next one
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        // if there are no more steps, then we've finished all of them for this quest
        else
        {
            ChangeQuestState(quest.info.id, E_QuestState.CAN_FINISH);
            Debug.Log($"<color=orange>[QuestManager]</color> Toutes les étapes de '{quest.info.id}' sont terminées. État: CAN_FINISH");
        }

        // Mettre à jour l'UI pour afficher le nouveau titre d'étape
        UpdateQuestUI();
    }

    /**
     * Termine la quête donnée par son ID
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 16th, 2025.
     * @access	private
     * @param	string	questID	
     * @return	void
     */
    private void FinishQuest(string questID)
    {
        Debug.Log($"<color=green>[QuestManager]</color> Terminer la quête: {questID}");
        
        S_Quest quest = GetQuestByID(questID);
        
        if (quest == null)
        {
            Debug.LogError($"<color=red>[QuestManager]</color> Impossible de trouver la quête avec l'ID: {questID}");
            return;
        }
        
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, E_QuestState.FINISHED);

        // Debug.Log("Quest " + questID + " finished. Rewards claimed.");
    }

    /**
     * Demande les récompenses de la quête au PlayerEvents
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 23rd, 2025.
     * @access	private
     * @param	s_quest	quest	
     * @return	void
     */
    private void ClaimRewards(S_Quest quest)
    {
        Debug.Log($"<color=cyan>[QuestManager]</color> Distribution des récompenses pour la quête: {quest.info.id}");
        
        // Distribue les récompenses ScriptableObject (lucidité, événements, etc.)
        if (quest.info.questRewards != null && quest.info.questRewards.Length > 0)
        {
            Debug.Log($"<color=cyan>[QuestManager]</color> {quest.info.questRewards.Length} récompense(s) trouvée(s)");
            
            foreach (QuestReward reward in quest.info.questRewards)
            {
                if (reward != null)
                {
                    Debug.Log($"<color=cyan>[QuestManager]</color> Distribution de: {reward.GetType().Name}");
                    reward.GiveReward();
                }
                else
                {
                    Debug.LogWarning($"<color=yellow>[QuestManager]</color> Une récompense est null dans la liste!");
                }
            }
        }
        else
        {
            Debug.Log($"<color=yellow>[QuestManager]</color> Aucune récompense configurée pour cette quête");
        }
        
        // Distribue l'expérience (ancien système)
        if (quest.info.experienceReward > 0)
        {
            Debug.Log($"<color=cyan>[QuestManager]</color> Distribution de {quest.info.experienceReward} points d'expérience");
            
            if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
            {
                S_GameManager.instance.playerEvents.ExperienceGained(quest.info.experienceReward);
            }
            else
            {
                Debug.LogError($"[QuestManager] GameManager ou PlayerEvents est null ! Impossible de donner l'expérience.");
            }
        }
    }

    #endregion

    /**
     * Récupère toutes les quêtes et les stocke dans un dictionnaire pour un accès facile
     *
     * @var		mixed	<string, S_Quest>
     * @var		mixed	CreateQuestMap()
     */
    private Dictionary<string, S_Quest> CreateQuestMap()
    {
        // Charge toutes les quêtes disponibles dans le dossier Resources/Quest
        SO_QuestInfo[] allQuest  = Resources.LoadAll<SO_QuestInfo>("Quest"); // ! Attention au chemin

        Dictionary<string, S_Quest> idToQuestMap = new Dictionary<string, S_Quest>();

        foreach (SO_QuestInfo questInfo in allQuest)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("[S_QuestManager] Duplicate quest ID found: " + questInfo.id);
            }
            // Charger la quête sauvegardée ou créer une nouvelle instance
            idToQuestMap[questInfo.id] = LoadQuest(questInfo);
        }
        return idToQuestMap;
    }

    /**
     * Récupère une quête par son ID
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 16th, 2025.
     * @access	private
     * @param	string	questID	
     * @return	mixed
     */
    public S_Quest GetQuestByID(string questID)
    {
        if (string.IsNullOrEmpty(questID))
        {
            Debug.LogError("[S_QuestManager] GetQuestByID appelé avec questID null ou vide!");
            return null;
        }

        if (!questMap.ContainsKey(questID))
        {
            Debug.LogWarning("[S_QuestManager] Quest not found for ID: " + questID);
            return null;
        }

        S_Quest quest = questMap[questID];
        return quest;
    }


    /**
     * Gère les événements de changement de niveau du joueur
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 23rd, 2025.
     * @access	private
     * @param	int	level	
     * @return	void
     */
    private void PlayerLevelChange(int level)
    {
        currentPlayerLevel = level;
    }


    /**
     * Vérifie si les conditions pour une quête sont remplies afin d'y accéder
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 23rd, 2025.
     * @access	private
     * @param	s_quest	quest	
     * @return	mixed
     */
    private bool CheckRequirementsMet(S_Quest quest)
    {
        // start true and prove to be false
        bool meetsRequirements = true;

        // check player level requirements
        if (currentPlayerLevel < quest.info.levelRequirement)
        {
            meetsRequirements = false;
        }

        // check quest prerequisites for completion
        foreach (SO_QuestInfo prerequisiteQuestInfo in quest.info.prerequisiteQuests)
        {
            if (GetQuestByID(prerequisiteQuestInfo.id).state != E_QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }

        return meetsRequirements;
    }

    #region Save & Load


    /**
     * Sauvegarde une quête dans les PlayerPrefs
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 29th, 2025.
     * @access	private
     * @param	s_quest	quest	
     * @return	void
     */
    private void SaveQuest(S_Quest quest)
    {
        try
        {
            S_QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);
            PlayerPrefs.SetString("Quest_" + quest.info.id, serializedData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[S_QuestManager] Error saving quest " + quest.info.id + ": " + e.Message);
        }
    }

    /**
     * Charge une quête depuis les données sauvegardées
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 29th, 2025.
     * @access	private
     * @param	so_questinfo	questInfo	
     * @return	mixed
     */
    private S_Quest LoadQuest(SO_QuestInfo questInfo)
    {
        S_Quest quest = null;
        try 
        {
            // load quest from saved data
            if (PlayerPrefs.HasKey("Quest_" + questInfo.id) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString("Quest_" + questInfo.id);
                S_QuestData questData = JsonUtility.FromJson<S_QuestData>(serializedData);
                quest = new S_Quest(questInfo, questData.state, questData.index, questData.questStepStates);
                Debug.Log($"<color=cyan>[S_QuestManager]</color> Quête '{questInfo.displayName}' chargée - État: {questData.state}, Étape: {questData.index}");
            }
            // otherwise, initialize a new quest
            else 
            {
                quest = new S_Quest(questInfo);
                Debug.Log($"<color=cyan>[S_QuestManager]</color> Nouvelle quête créée: '{questInfo.displayName}'");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }
        return quest;
    }
    
    /**
     * Réinitialise toutes les quêtes sauvegardées (utile pour le développement)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 12th, 2026.
     * @access	private
     * @return	void
     */
    private void ResetAllQuests()
    {
        Debug.Log("<color=yellow>[S_QuestManager]</color> Réinitialisation de toutes les quêtes sauvegardées...");
        
        // Charger toutes les quêtes disponibles
        SO_QuestInfo[] allQuests = Resources.LoadAll<SO_QuestInfo>("Quest");
        
        int resetCount = 0;
        foreach (SO_QuestInfo questInfo in allQuests)
        {
            string key = "Quest_" + questInfo.id;
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                resetCount++;
                Debug.Log($"<color=yellow>[S_QuestManager]</color> Quête '{questInfo.displayName}' (ID: {questInfo.id}) réinitialisée");
            }
        }
        
        PlayerPrefs.Save();
        Debug.Log($"<color=green>[S_QuestManager]</color> {resetCount} quête(s) réinitialisée(s)");
    }


    #endregion

    #region Interfaces

    /**
     * Cache l'interface de quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 29th, 2025.
     * @access	private
     * @return	void
     */
    public void HideQuestCanvas()
    {
        if (questCanvas != null)
        {
            questCanvas.SetActive(false);
        }
    }

    /**
     * Affiche l'interface de quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 29th, 2025.
     * @access	private
     * @return	void
     */
    public void ShowQuestCanvas()
    {
        if (questCanvas == null)
        {
            return;
        }

        bool hasActiveQuest = GetActiveQuest() != null;
        questCanvas.SetActive(hasActiveQuest);
        // Debug.Log($"[S_QuestManager] Canvas actif: {questCanvas.activeSelf} (has active quest: {hasActiveQuest})");
    }

    /**
     * Met à jour l'interface de quête en fonction de la quête sélectionnée ou active
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	private
     * @return	void
     */
    private void UpdateQuestUI()
    {
        if (questCanvas == null)
        {
            Debug.LogWarning("[S_QuestManager] questCanvas n'est pas assigné dans l'Inspector!");
            return;
        }

        if (QuestDisplayTitle == null)
        {
            Debug.LogWarning("[S_QuestManager] QuestDispalyTitle n'est pas assigné dans l'Inspector!");
            return;
        }

        // Utiliser la quête sélectionnée ou la première quête active
        S_Quest displayQuest = GetSelectedQuestForDisplay();
        
        if (displayQuest != null)
        {
            ShowQuestCanvas();
            QuestDisplayTitle.text = displayQuest.GetCurrentStepDisplayName();
        }
        else
        {
            HideQuestCanvas();
        }
    }


    #endregion



    #region Quest Access

    /**
     * Obtenir la quête actuellement active (IN_PROGRESS)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	public
     * @return	mixed	La première quête en cours, ou null si aucune
     */
    public S_Quest GetActiveQuest()
    {
        return questMap.Values.FirstOrDefault(q => q.state == E_QuestState.IN_PROGRESS);
    }
    
    /**
     * Obtenir toutes les quêtes actives (IN_PROGRESS)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	public
     * @return	mixed	Liste des quêtes en cours
     */
    public IEnumerable<S_Quest> GetActiveQuests()
    {
        return questMap.Values.Where(q => q.state == E_QuestState.IN_PROGRESS);
    }
    
    /**
     * Obtenir la première entrée du dictionnaire
     *
     * @var		mixed	<string
     *//**
     * Obtenir la première entrée du dictionnaire
     *
     * @var		mixed	GetFirstElement()
     */
    public KeyValuePair<string, S_Quest> GetFirstElement()
    {
        return questMap.FirstOrDefault();
    }

    /**
     * Méthode de debug pour afficher l'état de toutes les quêtes
     * Utile pour diagnostiquer les problèmes d'affichage
     *
     * ! Comment ça Marche ?
     * ! Quand le jeu est lancer, faire un clic droit sur le composant S_QuestManager
     * ! dans l'Inspector et dans les 3 points du script, sélectionner "Debug - Afficher toutes les quêtes".

     */
    [ContextMenu("Debug - Afficher toutes les quêtes")]
    public void DebugShowAllQuests()
    {
        Debug.Log("=== DEBUG QUÊTES ===");
        Debug.Log($"Nombre total de quêtes: {questMap.Count}");
        
        foreach (var quest in questMap.Values)
        {
            Debug.Log($"Quête: '{quest.info.displayName}' (ID: {quest.info.id}) - État: {quest.state}");
        }

        S_Quest activeQuest = GetActiveQuest();
        if (activeQuest != null)
        {
            Debug.Log($"<color=green>Quête active: {activeQuest.info.displayName}</color>");
        }
        else
        {
            Debug.Log("<color=red>Aucune quête IN_PROGRESS trouvée!</color>");
        }

        Debug.Log($"Canvas assigné: {questCanvas != null}");
        Debug.Log($"QuestDispalyTitle assigné: {QuestDisplayTitle != null}");
        if (questCanvas != null)
        {
            Debug.Log($"Canvas actif: {questCanvas.activeSelf}");
        }
        Debug.Log("===================");
    }

    [ContextMenu("Debug - Show Cureent level of player")]
    public void DebugShowPlayerLevel()
    {
        Debug.Log($"[S_QuestManager] Niveau actuel du joueur: {currentPlayerLevel}");
    }

    #endregion

    #region Quest Selection & Display

    /**
     * Définit la quête à afficher dans l'UI des objectifs
     * Les autres quêtes continuent de progresser en arrière-plan
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @param	S_Quest	quest	La quête à afficher
     * @return	void
     */
    public void SetSelectedQuestForDisplay(S_Quest quest)
    {
        selectedQuestForDisplay = quest;
        UpdateQuestUI();
        Debug.Log($"<color=cyan>[QuestManager]</color> Quête sélectionnée pour affichage: {quest?.info.displayName ?? "Aucune"}");
    }

    /**
     * Définit la quête à afficher par son ID
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @param	string	questId	L'ID de la quête à afficher
     * @return	void
     */
    public void SetSelectedQuestForDisplay(string questId)
    {
        S_Quest quest = GetQuestByID(questId);
        if (quest != null)
        {
            SetSelectedQuestForDisplay(quest);
        }
    }

    /**
     * Récupère la quête actuellement sélectionnée pour l'affichage
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @return	S_Quest	La quête sélectionnée ou null
     */
    public S_Quest GetSelectedQuestForDisplay()
    {
        // Si une quête est explicitement sélectionnée, la retourner
        if (selectedQuestForDisplay != null && selectedQuestForDisplay.state == E_QuestState.IN_PROGRESS)
        {
            return selectedQuestForDisplay;
        }
        // Sinon, retourner la première quête active
        return GetActiveQuest();
    }

    /**
     * Récupère la quête d'histoire principale
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @return	S_Quest	La quête principale ou null
     */
    public S_Quest GetStoryQuest()
    {
        return storyQuest;
    }

    /**
     * Définit la quête d'histoire principale
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @param	S_Quest	quest	La quête principale
     * @return	void
     */
    public void SetStoryQuest(S_Quest quest)
    {
        storyQuest = quest;
        Debug.Log($"<color=green>[QuestManager]</color> Quête principale définie: {quest?.info.displayName ?? "Aucune"}");
    }

    /**
     * Récupère les quêtes secondaires du jour
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @return	S_Quest[]	Tableau des quêtes secondaires
     */
    public S_Quest[] GetSideQuests()
    {
        return dailySideQuests.ToArray();
    }

    /**
     * Ajoute une quête secondaire au jour
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @param	S_Quest	quest	La quête à ajouter
     * @return	void
     */
    public void AddSideQuest(S_Quest quest)
    {
        if (quest != null && !dailySideQuests.Contains(quest))
        {
            dailySideQuests.Add(quest);
            Debug.Log($"<color=cyan>[QuestManager]</color> Quête secondaire ajoutée: {quest.info.displayName}");
        }
    }

    /**
     * Efface les quêtes secondaires du jour
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @return	void
     */
    public void ClearSideQuests()
    {
        dailySideQuests.Clear();
        Debug.Log("<color=yellow>[QuestManager]</color> Quêtes secondaires effacées");
    }

    #endregion

    #region Daily Quest Reset System

    /**
     * Réinitialise toutes les quêtes répétitives pour un nouveau jour
     * Appelé par S_DaysManager au début de chaque journée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @param	List<SO_QuestInfo>	questsToReset	Liste des quêtes à réinitialiser
     * @return	void
     */
    public void ResetDailyQuests(List<SO_QuestInfo> questsToReset)
    {
        Debug.Log("<color=yellow>[QuestManager]</color> Réinitialisation des quêtes journalières...");
        
        foreach (SO_QuestInfo questInfo in questsToReset)
        {
            if (questMap.ContainsKey(questInfo.id))
            {
                S_Quest quest = questMap[questInfo.id];
                
                // Détruire les instances de steps actives si nécessaire
                quest.CleanupCurrentStep();
                
                // Réinitialiser l'état
                quest.ResetQuest();
                
                // Supprimer la sauvegarde
                string key = "Quest_" + questInfo.id;
                if (PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.DeleteKey(key);
                }
                
                Debug.Log($"<color=yellow>[QuestManager]</color> Quête '{questInfo.displayName}' réinitialisée");
            }
        }
        
        PlayerPrefs.Save();
        
        // Effacer les quêtes secondaires du jour précédent
        ClearSideQuests();
        selectedQuestForDisplay = null;
        
        // Notifier les listeners
        OnDailyQuestsReset?.Invoke();
    }

    /**
     * Démarre une quête par son SO_QuestInfo (utilisé par S_LaunchRandomQuest)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @param	SO_QuestInfo	questInfo	Les informations de la quête
     * @return	S_Quest	La quête démarrée ou null
     */
    public S_Quest StartQuestFromInfo(SO_QuestInfo questInfo)
    {
        if (questInfo == null)
        {
            Debug.LogError("<color=red>[QuestManager]</color> StartQuestFromInfo: questInfo est null!");
            return null;
        }

        S_Quest quest = GetQuestByID(questInfo.id);
        if (quest == null)
        {
            Debug.LogError($"<color=red>[QuestManager]</color> Quête non trouvée: {questInfo.id}");
            return null;
        }

        // Forcer l'état CAN_START si nécessaire
        if (quest.state == E_QuestState.REQUIREMENTS_NOT_MET)
        {
            ChangeQuestState(quest.info.id, E_QuestState.CAN_START);
        }

        // Démarrer la quête via les events
        if (quest.state == E_QuestState.CAN_START)
        {
            S_GameManager.instance.questEvents.StartQuest(questInfo.id);
        }

        return quest;
    }

    /**
     * Vérifie si toutes les quêtes du jour sont terminées
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @return	bool	True si toutes les quêtes sont terminées
     */
    public bool AreAllDailyQuestsCompleted()
    {
        // Vérifier les quêtes secondaires
        foreach (S_Quest quest in dailySideQuests)
        {
            if (quest.state != E_QuestState.FINISHED)
            {
                return false;
            }
        }
        return true;
    }

    #endregion
    
}
