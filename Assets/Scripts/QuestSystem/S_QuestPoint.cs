/**
 * S_QuestPoint.cs
 * 
 * Représente un point de quête dans le jeu.
 * Chaque point de quête peut être un lieu où le joueur doit se rendre,
 * interagir avec un objet, ou accomplir une tâche spécifique.
 * On peut chosir si c'est le début de la qûete ou la fin de la quête.
**/

// ! SI BUG, vérifier ICI

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class S_QuestPoint : MonoBehaviour
{

    [Header("Quest")]
    [SerializeField] private SO_QuestInfo questInfoForPoint;

    private bool playerIsNear = false;
    private string questID;

    private E_QuestState currentQuestState; // Etat par défaut
    
    // Propriété helper pour éviter de répéter le cast partout
    private S_GameManager GameManager => (S_GameManager)S_GameManager.instance; //!

    private void Awake()
    {
        questID = questInfoForPoint.id;
    }

    private void OnEnable()
    {
        GameManager.questEvent.onQuestStateChange += QuestStateChange; //!
        ((S_InputEvent)GameManager.inputEvent).onSubmitPressed += SubmitPressed;    //!
    }

    private void OnDisable()
    {
        GameManager.questEvent.onQuestStateChange -= QuestStateChange;  //!
        ((S_InputEvent)GameManager.inputEvent).onSubmitPressed -= SubmitPressed;    //!
    }

    private void SubmitPressed()
    {
        if (!playerIsNear)
        {
            return;
        }

        GameManager.questEvent.StartQuest(questID);
        GameManager.questEvent.AdvanceQuest(questID);
        GameManager.questEvent.FinishQuest(questID);
        
    }

    private void QuestStateChange(S_Quest quest)
    {
        if (quest.info.id == questID)
        {
            currentQuestState = quest.state;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            Debug.Log("Player entered quest point for quest: " + questID + ", current state: " + currentQuestState);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
