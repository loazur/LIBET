/**
 * S_LaunchRandomQuest.cs
 * Fonctionnalités:
 * - Gère les listes de quêtes par niveau de difficulté (1 à 5)
 * - Lance 3 quêtes aléatoires par jour selon la difficulté du jour
 * - Reset l'état de toutes les quêtes répétitives au début de chaque jour
 * - Plus le jour avance, plus la difficulté augmente
 */

using System.Collections.Generic;
using UnityEngine;

public class S_LaunchRandomQuest : MonoBehaviour
{
    public static S_LaunchRandomQuest instance { get; private set; }

    [Header("Quest Lists by Difficulty")]
    [Tooltip("Quêtes de difficulté 1 (Jours 1-3)")]
    [SerializeField] private SO_QuestInfo[] questListDifficulty1;

    [Tooltip("Quêtes de difficulté 2 (Jours 4-6)")]
    [SerializeField] private SO_QuestInfo[] questListDifficulty2;

    [Tooltip("Quêtes de difficulté 3 (Jours 7-9)")]
    [SerializeField] private SO_QuestInfo[] questListDifficulty3;

    [Tooltip("Quêtes de difficulté 4 (Jours 10-12)")]
    [SerializeField] private SO_QuestInfo[] questListDifficulty4;

    [Tooltip("Quêtes de difficulté 5 (Jours 13-15)")]
    [SerializeField] private SO_QuestInfo[] questListDifficulty5;

    [Header("Configuration")]
    [Tooltip("Nombre de quêtes à lancer par jour")]
    [SerializeField] private int questsPerDay = 3;
    
    [Tooltip("Jours par niveau de difficulté")]
    [SerializeField] private int daysPerDifficultyLevel = 3; //& Genre c'est le décalage 3-6-9-12-15 5 niveau de difficulté

    // Quêtes actuellement sélectionnées pour le jour
    private List<SO_QuestInfo> selectedQuestsForDay = new List<SO_QuestInfo>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #region Difficulty Calculation

    /**
     * Calcule le niveau de difficulté en fonction du jour actuel
     *
     * @param   int     currentDay  Le jour actuel
     * @return  int     Le niveau de difficulté (1-5)
     */
    public int GetDifficultyForDay(int currentDay)
    {
        // Calcul du niveau de difficulté basé sur le jour
        // Jours 1-3 = Difficulté 1, Jours 4-6 = Difficulté 2, etc.
        int difficulty = ((currentDay - 1) / daysPerDifficultyLevel) + 1;
        return Mathf.Clamp(difficulty, 1, 5);
    }

    /**
     * Récupère la liste de quêtes pour un niveau de difficulté donné
     *
     * @param   int             difficulty  Le niveau de difficulté (1-5)
     * @return  SO_QuestInfo[]  La liste des quêtes disponibles
     */
    private SO_QuestInfo[] GetQuestListForDifficulty(int difficulty)
    {
        return difficulty switch
        {
            1 => questListDifficulty1,
            2 => questListDifficulty2,
            3 => questListDifficulty3,
            4 => questListDifficulty4,
            5 => questListDifficulty5,
            _ => questListDifficulty1 //& Retour par défaut au cas où
        };
    }

    #endregion

    #region Quest Selection & Launch

    /**
     * Sélectionne et lance les quêtes aléatoires pour le jour donné
     * Appelé par S_DaysManager au début de chaque journée
     *
     * @param   int     currentDay  Le jour actuel
     * @return  List<S_Quest>   Les quêtes lancées
     */
    public List<S_Quest> LaunchRandomQuestsForDay(int currentDay)
    {
        int difficulty = GetDifficultyForDay(currentDay);
        Debug.Log($"<color=green>[LaunchRandomQuest]</color> Jour {currentDay} - Difficulté {difficulty}");

        // Sélectionner les quêtes aléatoires
        selectedQuestsForDay = SelectRandomQuests(difficulty, questsPerDay);

        // Lancer les quêtes sélectionnées
        List<S_Quest> launchedQuests = new List<S_Quest>();
        
        foreach (SO_QuestInfo questInfo in selectedQuestsForDay)
        {
            S_Quest quest = S_QuestManager.instance.StartQuestFromInfo(questInfo);
            if (quest != null)
            {
                S_QuestManager.instance.AddSideQuest(quest);
                launchedQuests.Add(quest);
            }
        }

        Debug.Log($"<color=green>[LaunchRandomQuest]</color> {launchedQuests.Count} quête(s) lancée(s) pour le jour {currentDay}");
        
        return launchedQuests;
    }

    /**
     * Sélectionne un nombre donné de quêtes aléatoires pour une difficulté
     *
     * @param   int                 difficulty  Le niveau de difficulté
     * @param   int                 count       Le nombre de quêtes à sélectionner
     * @return  List<SO_QuestInfo>  Les quêtes sélectionnées
     */
    private List<SO_QuestInfo> SelectRandomQuests(int difficulty, int count)
    {
        SO_QuestInfo[] availableQuests = GetQuestListForDifficulty(difficulty);
        List<SO_QuestInfo> selectedQuests = new List<SO_QuestInfo>();

        if (availableQuests == null || availableQuests.Length == 0)
        {
            Debug.LogWarning($"<color=yellow>[LaunchRandomQuest]</color> Aucune quête disponible pour la difficulté {difficulty}");
            return selectedQuests;
        }

        // Créer une liste temporaire pour éviter les doublons
        List<SO_QuestInfo> tempList = new List<SO_QuestInfo>(availableQuests);

        // Sélectionner aléatoirement
        for (int i = 0; i < count && tempList.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            selectedQuests.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        return selectedQuests;
    }

    /**
     * Lance une seule quête aléatoire d'une difficulté spécifique
     *
     * @param   int     difficulty  Le niveau de difficulté (1-5)
     * @return  S_Quest La quête lancée ou null
     */
    public S_Quest LaunchSingleRandomQuest(int difficulty)
    {
        SO_QuestInfo[] questList = GetQuestListForDifficulty(difficulty);
        
        if (questList == null || questList.Length == 0)
        {
            Debug.LogWarning($"<color=yellow>[LaunchRandomQuest]</color> Aucune quête pour la difficulté {difficulty}");
            return null;
        }

        SO_QuestInfo randomQuest = questList[Random.Range(0, questList.Length)];
        return S_QuestManager.instance.StartQuestFromInfo(randomQuest);
    }

    #endregion

    #region Quest Reset

    /**
     * Réinitialise l'état de toutes les quêtes des listes de difficulté
     * Appelé par S_DaysManager au début de chaque jour
     */
    public void ResetAllRepeatableQuests()
    {
        Debug.Log("<color=yellow>[LaunchRandomQuest]</color> Réinitialisation des quêtes répétitives...");

        List<SO_QuestInfo> allQuests = GetAllRepeatableQuests();
        S_QuestManager.instance.ResetDailyQuests(allQuests);
        
        // Nettoyer la sélection du jour
        selectedQuestsForDay.Clear();
        
        Debug.Log($"<color=yellow>[LaunchRandomQuest]</color> {allQuests.Count} quête(s) réinitialisée(s)");
    }

    /**
     * Récupère toutes les quêtes répétitives de toutes les listes de difficulté
     *
     * @return  List<SO_QuestInfo>  Toutes les quêtes répétitives
     */
    public List<SO_QuestInfo> GetAllRepeatableQuests()
    {
        List<SO_QuestInfo> allQuests = new List<SO_QuestInfo>();

        // Ajouter les quêtes de chaque niveau de difficulté
        AddQuestsToList(allQuests, questListDifficulty1);
        AddQuestsToList(allQuests, questListDifficulty2);
        AddQuestsToList(allQuests, questListDifficulty3);
        AddQuestsToList(allQuests, questListDifficulty4);
        AddQuestsToList(allQuests, questListDifficulty5);

        return allQuests;
    }

    /**
     * Ajoute les quêtes d'un tableau à une liste (helper method)
     */
    private void AddQuestsToList(List<SO_QuestInfo> list, SO_QuestInfo[] quests)
    {
        if (quests != null)
        {
            foreach (SO_QuestInfo quest in quests)
            {
                if (quest != null && !list.Contains(quest))
                {
                    list.Add(quest);
                }
            }
        }
    }

    #endregion

    #region Getters

    /**
     * Récupère les quêtes sélectionnées pour le jour actuel
     *
     * @return  List<SO_QuestInfo>  Les quêtes du jour
     */
    public List<SO_QuestInfo> GetSelectedQuestsForDay()
    {
        return new List<SO_QuestInfo>(selectedQuestsForDay);
    }

    /**
     * Récupère le nombre de quêtes par jour configuré
     *
     * @return  int     Le nombre de quêtes par jour
     */
    public int GetQuestsPerDay()
    {
        return questsPerDay;
    }

    #endregion

    #region Debug

    [ContextMenu("Debug - Show All Quest Lists")]
    public void DebugShowAllQuestLists()
    {
        Debug.Log("=== DEBUG QUEST LISTS ===");
        Debug.Log($"Difficulté 1: {questListDifficulty1?.Length ?? 0} quête(s)");
        Debug.Log($"Difficulté 2: {questListDifficulty2?.Length ?? 0} quête(s)");
        Debug.Log($"Difficulté 3: {questListDifficulty3?.Length ?? 0} quête(s)");
        Debug.Log($"Difficulté 4: {questListDifficulty4?.Length ?? 0} quête(s)");
        Debug.Log($"Difficulté 5: {questListDifficulty5?.Length ?? 0} quête(s)");
        Debug.Log($"Total: {GetAllRepeatableQuests().Count} quête(s)");
        Debug.Log("=========================");
    }

    [ContextMenu("Debug - Test Launch Day 1")]
    public void DebugTestLaunchDay1()
    {
        LaunchRandomQuestsForDay(1);
    }

    #endregion
}