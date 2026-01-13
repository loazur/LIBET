// /**
//  * S_UIQuestMenu.cs
//  * Fonctionnalités:
//  * Ouvrir un menu d'affichage des quêtes avec la touche "I"
//  * Choisir la quête à afficher dans l'UI des Objectifs qui se trouve dans le QuestManager
//  * Organigrame de l'UI des quêtes:
//     GameObject UIQuestMenu
//         |-- Panel Background
//         |-- Button Quest Story
//             |-- Text Quest Story Title
//             |-- Text Quest Story Description
//         |-- Button Quest Side 1
//             |-- Button Quest Side Button 1
//                 |-- Text Quest Side Title 1
//                 |-- Text Quest Side Description 1
//         |-- Button Quest Side 2
//             |-- Button Quest Side Button 2
//                 |-- Text Quest Side Title 2
//                 |-- Text Quest Side Description 2
//         |-- Button Quest Side 3
//             |-- Button Quest Side Button 3
//                 |-- Text Quest Side Title 3
//                 |-- Text Quest Side Description 3
//  * 
//  * Quand on appuie sur la touche "I", le menu des quêtes s'ouvre ou se ferme.
//  * Faire la traduction des textes en fonction de la langue sélectionnée (FR/EN)
//  * Quand le joueur clique sur une quête, afficher son titre et sa description dans l'UI des objectifs qui est gérée par le QuestManager
//  * 
//  * 
//  * 
//  * 
//  * 
// */

// using UnityEngine;
// using UnityEngine.UI;

// public class S_UIQuestMenu : MonoBehaviour
// {
//     // UI
//     // --------------------------------------------------
//     public GameObject uiQuestMenu;

//     //& Quete Histoire
//     public Text questStoryTitleText;
//     public Text questStoryDescriptionText;

//     //& Quete Secondaire
//     //1
//     public Button questSideButton1;
//     public Text questSideTitleText1;
//     public Text questSideDescriptionText1;

//     //2
//     public Button questSideButton2;
//     public Text questSideTitleText2;
//     public Text questSideDescriptionText2;

//     //3
//     public Button questSideButton3;
//     public Text questSideTitleText3;
//     public Text questSideDescriptionText3;

//     void Start()
//     {
        
//     }

//     void Update()
//     {
//         // Ouvrir/fermer le menu des quêtes avec la touche spécifiée est appuyée
//         if (S_UserInput.instance.QuestMenuAction)
//         {
//             uiQuestMenu.SetActive(!uiQuestMenu.activeSelf);

//             if (uiQuestMenu.activeSelf)
//             {
//                 UpdateQuestMenuUI();
//             }
//         }
//     }


//     #region Gestion des panels de quêtes

//     public void HideQuestPanel()
//     {
//         uiQuestMenu.SetActive(false);
//     }

//     public void ShowQuestPanel()
//     {
//         uiQuestMenu.SetActive(true);
//         UpdateQuestMenuUI();
//     }

//     #endregion

//     #region Mise à jour de l'UI des quêtes


//     private void UpdateQuestMenuUI()
//     {
//         S_QuestManager questManager = S_QuestManager.instance;

//         // Mettre à jour la quête d'histoire
//         S_Quest storyQuest = questManager.GetStoryQuest();
//         if (storyQuest != null)
//         {
//             questStoryTitleText.text = storyQuest.info.displayName;
//             questStoryDescriptionText.text = storyQuest.info.description;
//         }
//         else
//         {
//             questStoryTitleText.text = "Aucune quête d'histoire";
//             questStoryDescriptionText.text = "";
//         }

//         // Mettre à jour les quêtes secondaires
//         S_Quest[] sideQuests = questManager.GetSideQuests();

//         //1
//         if (sideQuests.Length > 0)
//         {
//             questSideTitleText1.text = sideQuests[0].info.displayName;
//             questSideDescriptionText1.text = sideQuests[0].info.description;
//         }
//         else
//         {
//             questSideTitleText1.text = "Aucune quête secondaire";
//             questSideDescriptionText1.text = "";
//         }

//         //2
//         if (sideQuests.Length > 1)
//         {
//             questSideTitleText2.text = sideQuests[1].info.displayName;
//             questSideDescriptionText2.text = sideQuests[1].info.description;
//         }
//         else
//         {
//             questSideTitleText2.text = "Aucune quête secondaire";
//             questSideDescriptionText2.text = "";
//         }

//         //3
//         if (sideQuests.Length > 2)
//         {
//             questSideTitleText3.text = sideQuests[2].info.displayName;
//             questSideDescriptionText3.text = sideQuests[2].info.description;
//         }
//         else
//         {
//             questSideTitleText3.text = "Aucune quête secondaire";
//             questSideDescriptionText3.text = "";
//         }
//     }


//     #endregion





// }