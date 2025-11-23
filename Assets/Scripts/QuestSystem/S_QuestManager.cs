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



    #region METHODS
    // *==========================================================================*
    // *                                 METHODS                                  *
    // *==========================================================================*
    #endregion

    private void Awake()
    {
        quesMap = CreateQuestMap();
    }


    //! BUG POTENTIEL ICI
    private void OnEnable()
    {
        S_GameManager.instance.questEvents.OnStartQuest += StartQuest;
        S_GameManager.instance.questEvents.OnAdvanceQuest += AdvanceQuest;
        S_GameManager.instance.questEvents.OnFinishQuest += FinishQuest;
    }

    private void OnDisable()
    {
        S_GameManager.instance.questEvents.OnStartQuest -= StartQuest;
        S_GameManager.instance.questEvents.OnAdvanceQuest -= AdvanceQuest;
        S_GameManager.instance.questEvents.OnFinishQuest -= FinishQuest;
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

    
}
