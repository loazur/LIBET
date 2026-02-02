
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;


// TODO : Optimiser avec des events au lieu de check en Update



/**
 * Lance une quête principale tous les X jours définis dans SO_ParamatersForPrincipalQuest
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, February 2nd, 2026.
 * @global
 */
class S_LaunchQuestPerDay : MonoBehaviour
{
    [SerializeField] private SO_ParamatersForPrincipalQuest[] principalQuestParameters;


    private SO_QuestInfo currentQuest;
    private int DailyQuestChange;

    /**
     * Initialise le premier jour
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 2nd, 2026.
     * @return	void
     */
    void Start()
    {
        //! SI PB POTENTIELLEMENT ICI
        if (principalQuestParameters.Length == 0)
        {
            Debug.LogError("[S_LaunchQuestPerDay] Aucun paramètre de quête principal défini !");
            return;
        }
        
        // Premier lancement au jour 0
        DailyQuestChange = principalQuestParameters[0].DecalageBetweenQuestDays;
        InitialiseCurrentQuest(0);
        LauchCurrentQuest();
        CalcNextCallForQuest();
    }

    /**
     * Update est appelé une fois par frame
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 2nd, 2026.
     * @return	void
     */
    void Update()
    {
        //! SI PB POTENTIELLEMENT ICI
        int currentDay = S_DaysManager.instance.GetCurrentDay();

        // Récupérer la quête et vérifier qu'elle est terminée avant d'en lancer une nouvelle
        S_Quest quest = S_GameManager.instance.questManager.GetQuestByID(currentQuest.id);

        if (quest != null && currentDay >= DailyQuestChange)
        {
            if (quest.state != E_QuestState.FINISHED)
            {
                // La quête courante n'est pas encore terminée -> ne rien faire
                return;
            }
            int nextIndex = (DailyQuestChange / principalQuestParameters[0].DecalageBetweenQuestDays);

            if (nextIndex < principalQuestParameters.Length)
            {
                InitialiseCurrentQuest(nextIndex);
                LauchCurrentQuest();
                CalcNextCallForQuest();
            }
        }

    }


    /**
     * Initialise la quête actuelle en fonction de l'index
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 2nd, 2026.
     * @access	private
     * @param	int	index	
     * @return	void
     */
    private void InitialiseCurrentQuest(int index)
    {
        currentQuest = principalQuestParameters[index].QuestPrincipal;
    }

    /**
     * Lance la quête actuelle
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 2nd, 2026.
     * @access	private
     * @return	void
     */
    private void LauchCurrentQuest()
    {
        // Obtenir le questid de la quête principale
        string questId = currentQuest.id;

        S_GameManager.instance.questEvents.StartQuest(questId); // Lancer la quete actuelle


    }

    /**
     * Cherche le jour où on doit lancer la prochaine quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 2nd, 2026.
     * @access	private
     * @return	void
     */
    private void CalcNextCallForQuest()
    {
        int decalage = principalQuestParameters[0].DecalageBetweenQuestDays;
        DailyQuestChange += decalage;
    }

}