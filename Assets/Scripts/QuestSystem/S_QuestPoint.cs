/**
 * S_QuestPoint.cs
 * 
 * Représente un point de quête dans le jeu.
 * Chaque point de quête peut être un lieu où le joueur doit se rendre,
 * interagir avec un objet, ou accomplir une tâche spécifique.
 * On peut chosir si c'est le début de la qûete ou la fin de la quête.
**/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class S_QuestPoint : MonoBehaviour
{

    [Header("Dialogue (optional)")]
    [SerializeField] private string dialogueKnotName;

    [Header("Quest")]
    [SerializeField] private SO_QuestInfo questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    private bool playerIsNear = false;
    private string questId;
    private E_QuestState currentQuestState;

    // private QuestIcon questIcon;

    private void Awake() 
    {
        questId = questInfoForPoint.id;
        // questIcon = GetComponentInChildren<QuestIcon>();
    }

    private void OnEnable()
    {
        //* Vérifier que le GameManager est initialisé avant de s'abonner aux événements
        if (S_GameManager.instance == null)
        {
            Debug.LogWarning("S_QuestPoint: GameManager instance is null. Make sure S_GameManager is in the scene and initialized first.");
            return;
        }

        S_GameManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        S_GameManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        //* Vérifier que le GameManager existe encore avant de se désabonner
        if (S_GameManager.instance == null)
        {
            Debug.LogWarning("S_QuestPoint: GameManager instance est null on OnDisable. Skipping unsubscription.");
            return;

        } 

        S_GameManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        S_GameManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if (!playerIsNear || !inputEventContext.Equals(InputEventContext.DEFAULT))
        {
            return;
        }

        //* si un nom de nœud de dialogue est défini, tenter de lancer le dialogue avec celui-ci
        //* commencer ou terminer la quête
        if (currentQuestState.Equals(E_QuestState.CAN_START) && startPoint)
        {
            S_GameManager.instance.questEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(E_QuestState.CAN_FINISH) && finishPoint)
        {
            S_GameManager.instance.questEvents.FinishQuest(questId);
        }
        
    }

    private void QuestStateChange(S_Quest quest)
    {
        // only update the quest state if this point has the corresponding quest
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            Debug.Log("Player is near quest point for quest: " + questId + ", current state: " + currentQuestState);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            Debug.Log("Player left quest point for quest: " + questId + ", current state: " + currentQuestState);
        }
    }
}
