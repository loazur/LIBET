// S_QuestManager.cs
// Gère les quêtes dans le jeu

using System.Collections.Generic;
using UnityEngine;

public class S_QuestManager : MonoBehaviour
{
    #region ATTRIBUTS
    // *==========================================================================*
    // *                                 ATTRIBUTS                                *
    // *==========================================================================*
    #endregion


    private Dictionary<string, S_Quest> quesMap;

    private int currentPlayerLevel;

    #region METHODS
    // *==========================================================================*
    // *                                 METHODS                                  *
    // *==========================================================================*
    #endregion

    private void Awake()
    {
        quesMap = CreateQuestMap();
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

    private void OnEnable()
    {
        S_GameManager.instance.questEvents.OnStartQuest += StartQuest;
        S_GameManager.instance.questEvents.OnAdvanceQuest += AdvanceQuest;
        S_GameManager.instance.questEvents.OnFinishQuest += FinishQuest;

        S_GameManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;
    }

    private void OnDisable()
    {
        S_GameManager.instance.questEvents.OnStartQuest -= StartQuest;
        S_GameManager.instance.questEvents.OnAdvanceQuest -= AdvanceQuest;
        S_GameManager.instance.questEvents.OnFinishQuest -= FinishQuest;

        S_GameManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;
    }

    private void Start()
    {
        foreach(S_Quest quest in quesMap.Values)
        {
            S_GameManager.instance.questEvents.QuestStateChange(quest);
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
        Debug.Log("Starting quest: " + questID);
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
        Debug.Log("Advancing quest: " + questID);
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
        Debug.Log("Finishing quest: " + questID);}

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
        S_Quest quest = quesMap[questID];
        if (quest == null)
        {
            Debug.LogWarning("[S_QuestManager] Quest not found for ID: " + questID);
        }
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

    
}
