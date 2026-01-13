// /**
//  * S_LunchRandomQuest.cs
//  * Fonctionnalités:
//  * Lancer 3 quête aléatoire parmi les listes de quêtes prédéfinies
//  * 
// */

// using UnityEngine;
// using System.Collections;
// using Unity.VisualScripting;

// public class S_LaunchRandomQuest
// {
//     [Header("Quest List Difficulty 1")]
//     [SerializeField] private SO_QuestInfo[] questListDifficulty1;

//     [Header("Quest List Difficulty 2")]
//     [SerializeField] private SO_QuestInfo[] questListDifficulty2;

//     [Header("Quest List Difficulty 3")]
//     [SerializeField] private SO_QuestInfo[] questListDifficulty3;

//     [Header("Quest List Difficulty 4")]
//     [SerializeField] private SO_QuestInfo[] questListDifficulty4;

//     [Header("Quest List Difficulty 5")]
//     [SerializeField] private SO_QuestInfo[] questListDifficulty5;


//     private SO_QuestInfo[] selectedQuest; //& 3 quetes aléatoire seront stocker ici avant d'être lancer


//     #region Launch Random Quests
//     public void LaunchRandomQuestsDifficulty1()
//     {
//         S_QuestManager questManager = S_QuestManager.instance;
//         SO_QuestInfo randomQuest = questListDifficulty1[Random.Range(0, questListDifficulty1.Length)];
//         questManager.StartQuest(randomQuest);
//     }

//     public void LaunchRandomQuestsDifficulty2()
//     {
//         S_QuestManager questManager = S_QuestManager.instance;
//         SO_QuestInfo randomQuest = questListDifficulty2[Random.Range(0, questListDifficulty2.Length)];
//         questManager.StartQuest(randomQuest);
//     }

//     public void LaunchRandomQuestsDifficulty3()
//     {
//         S_QuestManager questManager = S_QuestManager.instance;
//         SO_QuestInfo randomQuest = questListDifficulty3[Random.Range(0, questListDifficulty3.Length)];
//         questManager.StartQuest(randomQuest);
//     }

//     public void LaunchRandomQuestsDifficulty4()
//     {
//         S_QuestManager questManager = S_QuestManager.instance;
//         SO_QuestInfo randomQuest = questListDifficulty4[Random.Range(0, questListDifficulty4.Length)];
//         questManager.StartQuest(randomQuest);
//     }

//     public void LaunchRandomQuestsDifficulty5()
//     {
//         S_QuestManager questManager = S_QuestManager.instance;
//         SO_QuestInfo randomQuest = questListDifficulty5[Random.Range(0, questListDifficulty5.Length)];
//         questManager.StartQuest(randomQuest);
//     }

//     #endregion

//     public void LaunchAllSelectedQuests()
//     {
//         S_QuestManager questManager = S_QuestManager.instance;
//         for (int i = 0; i < selectedQuest.Length; i++)
//         {
//             questManager.StartQuest(selectedQuest[i]);
//         }
//     }


//     public void ResetStateOfSelectedQuests()
//     {
//         for (int i = 0; i < selectedQuest.Length; i++)
//         {
//             selectedQuest[i].state = E_QuestState.REQUIREMENTS_NOT_MET;
//         }
//     }

// }