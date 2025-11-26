// S_QuestManager.cs
// Gère les quêtes dans le jeu

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class S_QuestManager : MonoBehaviour
{
    #region ATTRIBUTS
    // *==========================================================================*
    // *                                 ATTRIBUTS                                *
    // *==========================================================================*
    #endregion

    [Header("Interface pour les quêtes")]
    [SerializeField] private GameObject questCanvas; // Canvas pour les quêtes
    [SerializeField]private Text QuestDispalyTitle;
    private Dictionary<string, S_Quest> quesMap;

    private int currentPlayerLevel;
    private bool isSubscribed = false;

    #region METHODS
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
        foreach (S_Quest quest in quesMap.Values)
        {
            if (quest.state == E_QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, E_QuestState.CAN_START);
                // Debug.Log("Quest " + quest.info.id + " requirements met. State changed to CAN_START.");
            }
            // Debug.Log("Quest " + quest.info.id + " is in state " + quest.state.ToString() + "CheckRequirementsMet returned " + CheckRequirementsMet(quest).ToString() + ".");
        }
    }

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

        // Notifier l'état initial de toutes les quêtes
        foreach(S_Quest quest in quesMap.Values)
        {
            S_GameManager.instance.questEvents.QuestStateChange(quest);
        }
    }

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
        
        isSubscribed = true;
        Debug.Log("[S_QuestManager] Abonné aux événements du GameManager.");
    }

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
        
        isSubscribed = false;
    }

    

    private void OnEnable()
    {
        // L'abonnement sera géré par InitializeWhenReady() dans Start()
        // pour garantir que S_GameManager est prêt
    }

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
            if (quest.state == E_QuestState.IN_PROGRESS)
            {
                ChangeQuestState(quest.info.id, E_QuestState.REQUIREMENTS_NOT_MET);
                Debug.Log("[S_QuestManager] Application quittée. Quest " + quest.info.id + " state reset to REQUIREMENTS_NOT_MET.");
            }
        }
    }


    #region QUEST ADVANCEMENT



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

    #region Interfaces

    private void HideQuestCanvas()
    {
        questCanvas.SetActive(false);
    }

    private void ShowQuestCanvas()
    {
        questCanvas.SetActive(true);
    }

    private void SetTitle()
    {
        S_Quest quest = GetFirstQuest();
        
        if (quest == null)
        {
            HideQuestCanvas();
            return;
        }
        else
        {
            if (quest.state == E_QuestState.REQUIREMENTS_NOT_MET)
            {
                ShowQuestCanvas();
                QuestDispalyTitle.text = quest.info.displayName;
            }
            else
            {
                HideQuestCanvas();
            }
            
        }
        
    }


    #endregion



    #region Quest Access

    /**
     * Obtenir la première quête du dictionnaire
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, November 26th, 2025.
     * @access	public
     * @return	mixed
     */
    public S_Quest GetFirstQuest()
    {
        return quesMap.Values.FirstOrDefault();
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

    #endregion
    
}
