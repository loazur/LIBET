
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
        LauchOneQuest();
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

        if (currentDay >= DailyQuestChange)
        {
            int nextIndex = (DailyQuestChange / principalQuestParameters[0].DecalageBetweenQuestDays);
            if (nextIndex < principalQuestParameters.Length)
            {
                InitialiseCurrentQuest(nextIndex);
                LauchOneQuest();
                CalcNextCallForQuest();
            }
        }

    }



    private void InitialiseCurrentQuest(int index)
    {
        currentQuest = principalQuestParameters[index].QuestPrincipal;
    }

    private void LauchOneQuest()
    {
        // Obtenir le questid de la quête principale
        string questId = currentQuest.id;

        S_GameManager.instance.questEvents.StartQuest(questId); // Lancer la quete actuelle


    }

    private void CalcNextCallForQuest()
    {
        int decalage = principalQuestParameters[0].DecalageBetweenQuestDays;
        DailyQuestChange += decalage;
    }

}