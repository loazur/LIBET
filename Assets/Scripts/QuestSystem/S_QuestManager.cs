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

    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    [Header("Interface pour les quêtes")]
    [SerializeField] private GameObject questCanvas; // Canvas pour les quêtes
    [SerializeField]private Text QuestDispalyTitle;
    private Dictionary<string, S_Quest> quesMap;

    private int currentPlayerLevel = 1; // Niveau par défaut (sera mis à jour par PlayerLevelChange)
    private bool isSubscribed = false;

    #region METHODS MonoBehaviour
    // *==========================================================================*
    // *                                 METHODS                                  *
    // *==========================================================================*
    #endregion

    private void Awake()
    {
        quesMap = CreateQuestMap();
    }

    private void Start()
    {
        StartCoroutine(InitializeWhenReady());
    }

    public void Update()
    {
        // Vérifier si des quêtes peuvent passer de REQUIREMENTS_NOT_MET à CAN_START
        // TODO: Optimiser en utilisant un système d'événements plutôt que Update()
        foreach (S_Quest quest in quesMap.Values)
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
            Debug.Log("[S_QuestManager] En attente de l'initialisation de S_GameManager...");
            yield return null;
        }

        // S'abonner aux événements si pas encore fait
        if (!isSubscribed)
        {
            SubscribeToEvents();
        }

        // Attendre que PlayerLevelManager initialise le niveau du joueur
        yield return new WaitForSeconds(0.1f);

        // Notifier l'état initial de toutes les quêtes
        foreach(S_Quest quest in quesMap.Values)
        {
            Debug.Log($"[S_QuestManager] Quête '{quest.info.displayName}' (ID: {quest.info.id}) - État: {quest.state}");
            
            if (quest.state  ==  E_QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            S_GameManager.instance.questEvents.QuestStateChange(quest);
        }

        // Initialiser l'UI avec la quête active
        Debug.Log("[S_QuestManager] Initialisation de l'UI...");
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
        
        isSubscribed = true;
        Debug.Log("[S_QuestManager] Abonné aux événements du GameManager.");
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
        foreach (S_Quest quest in quesMap.Values)
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
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(questID, E_QuestState.IN_PROGRESS);

        Debug.Log("Quest " + questID + " started " + " with first step: " + quest.state.ToString());
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

        // move on to the next step
        quest.MoveToNextStep();

        // if there are more steps, instantiate the next one
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        // if there are no more steps, then we've finished all of them for this quest
        else
        {
            ChangeQuestState(quest.info.id, E_QuestState.CAN_FINISH);
        }

        Debug.Log("Quest " + questID + " advanced to step: " + quest.state.ToString());
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
        S_Quest quest = GetQuestByID(questID);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, E_QuestState.FINISHED);

        Debug.Log("Quest " + questID + " finished. Rewards claimed.");
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
        S_GameManager.instance.playerEvents.ExperienceGained(quest.info.experienceReward);
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
            idToQuestMap[questInfo.id] = new S_Quest(questInfo);
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
    private S_Quest GetQuestByID(string questID)
    {
        if (string.IsNullOrEmpty(questID))
        {
            Debug.LogError("[S_QuestManager] GetQuestByID appelé avec questID null ou vide!");
            return null;
        }

        if (!quesMap.ContainsKey(questID))
        {
            Debug.LogWarning("[S_QuestManager] Quest not found for ID: " + questID);
            return null;
        }

        S_Quest quest = quesMap[questID];
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
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                S_QuestData questData = JsonUtility.FromJson<S_QuestData>(serializedData);
                quest = new S_Quest(questInfo, questData.state, questData.index, questData.questStepStates);
            }
            // otherwise, initialize a new quest
            else 
            {
                quest = new S_Quest(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }
        return quest;
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
    private void HideQuestCanvas()
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
    private void ShowQuestCanvas()
    {
        if (questCanvas != null)
        {
            questCanvas.SetActive(true);
            Debug.Log($"[S_QuestManager] Canvas activé. IsActive: {questCanvas.activeSelf}");
        }
    }

    /**
     * Met à jour l'interface de quête en fonction de la quête active
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
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

        if (QuestDispalyTitle == null)
        {
            Debug.LogWarning("[S_QuestManager] QuestDispalyTitle n'est pas assigné dans l'Inspector!");
            return;
        }

        S_Quest activeQuest = GetActiveQuest();
        
        if (activeQuest != null)
        {
            Debug.Log($"[S_QuestManager] Quête active trouvée: {activeQuest.info.displayName} (État: {activeQuest.state})");
            ShowQuestCanvas();
            QuestDispalyTitle.text = activeQuest.info.displayName;
            Debug.Log($"[S_QuestManager] UI mise à jour avec le titre: {QuestDispalyTitle.text}");
        }
        else
        {
            Debug.Log("[S_QuestManager] Aucune quête active (IN_PROGRESS) trouvée.");
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
        return quesMap.Values.FirstOrDefault(q => q.state == E_QuestState.IN_PROGRESS);
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
        return quesMap.Values.Where(q => q.state == E_QuestState.IN_PROGRESS);
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
        return quesMap.FirstOrDefault();
    }

    /**
     * Méthode de debug pour afficher l'état de toutes les quêtes
     * Utile pour diagnostiquer les problèmes d'affichage
     */
    [ContextMenu("Debug - Afficher toutes les quêtes")]
    public void DebugShowAllQuests()
    {
        Debug.Log("=== DEBUG QUÊTES ===");
        Debug.Log($"Nombre total de quêtes: {quesMap.Count}");
        
        foreach (var quest in quesMap.Values)
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
        Debug.Log($"QuestDispalyTitle assigné: {QuestDispalyTitle != null}");
        if (questCanvas != null)
        {
            Debug.Log($"Canvas actif: {questCanvas.activeSelf}");
        }
        Debug.Log("===================");
    }

    #endregion
    
}
