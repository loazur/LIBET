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
    [Tooltip("Ce QuestPoint peut démarrer la quête")]
    [SerializeField] private bool startPoint = true;
    
    [Tooltip("Ce QuestPoint peut terminer la quête")]
    [SerializeField] private bool finishPoint = true;
    
    [Header("Interaction Mode")]
    [Tooltip("Si true, la quête démarre automatiquement quand le joueur entre dans le trigger. Si false, il faut appuyer sur Submit.")]
    [SerializeField] private bool autoStartQuest = true;
    
    [Tooltip("Si true, la quête se termine automatiquement quand le joueur entre dans le trigger (après avoir complété toutes les étapes). Si false, il faut appuyer sur Submit.")]
    [SerializeField] private bool autoFinishQuest = false;

    // *----------------------------------------------------------------*

    private bool playerIsNear = false;
    private string questId;
    private E_QuestState currentQuestState;
    private bool isSubscribed = false;


    // *----------------------------------------------------------------*

    private void Awake() 
    {
        questId = questInfoForPoint.id;
    }

    private void Start()
    {
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
    {
        // Attendre que S_GameManager soit initialisé
        while (S_GameManager.instance == null)
        {
            yield return null;
        }

        // S'abonner aux événements
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed) return;

        S_GameManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        S_GameManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
        isSubscribed = true;

    }

    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;

        S_GameManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        S_GameManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        isSubscribed = false;
    }


    /**
     * Active les événements de quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 23rd, 2025.
     * @access	private
     * @return	void
     */
    private void OnEnable()
    {
        // L'abonnement est géré par InitializeWhenReady() dans Start()
    }

    /**
     * Désactive les événements de quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 23rd, 2025.
     * @access	private
     * @return	void
     */
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }


    /**
     * Permet de gérer l'entrée et la sortie de la zone d'interaction
     * Appelé quand le joueur appuie sur Submit (mode manuel uniquement)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 23rd, 2025.
     * @access	private
     * @param	inputeventcontext	inputEventContext	
     * @return	void
     */
    private void SubmitPressed(E_InputEventContext inputEventContext)
    {
        if (!playerIsNear)
        {
            return;
        }

        if (!inputEventContext.Equals(E_InputEventContext.DEFAULT))
        {
            // Debug.Log($"[S_QuestPoint] Input context is {inputEventContext}, not DEFAULT. Ignoring.");
            return;
        }

        Debug.Log($"[QuestPoint] Submit pressé - Quête: '{questId}' | État: {currentQuestState} | AutoStart: {autoStartQuest} | AutoFinish: {autoFinishQuest}");

        // Démarrage manuel de la quête (si autoStartQuest est false)
        if (!autoStartQuest && currentQuestState.Equals(E_QuestState.CAN_START) && startPoint)
        {
            Debug.Log($"[QuestPoint] Démarrage manuel de la quête '{questId}'");
            S_GameManager.instance.questEvents.StartQuest(questId);
        }
        // Finalisation manuelle de la quête (si autoFinishQuest est false)
        else if (!autoFinishQuest && currentQuestState.Equals(E_QuestState.CAN_FINISH) && finishPoint)
        {
            Debug.Log($"[QuestPoint] Finalisation manuelle de la quête '{questId}'");
            S_GameManager.instance.questEvents.FinishQuest(questId);
        }
        else
        {
            Debug.Log($"[QuestPoint] Submit ignoré - Conditions non remplies pour '{questId}'");
        }
    }


    /**
     * permet de mettre à jour l'état de la quête associée à ce point de quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 23rd, 2025.
     * @access	private
     * @param	s_quest	quest	
     * @return	void
     */
    private void QuestStateChange(S_Quest quest)
    {
        // only update the quest state if this point has the corresponding quest
        if (quest.info.id.Equals(questId))
        {
            E_QuestState oldState = currentQuestState;
            currentQuestState = quest.state;
            Debug.Log($"[QuestPoint] Changement d'état pour '{questId}': {oldState} → {currentQuestState}");
        }
    }


    /**
     * Détecte quand le joueur entre dans la zone du point de quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 23rd, 2025.
     * @access	private
     * @param	collider	other	
     * @return	void
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            Debug.Log($"[QuestPoint] Joueur entre dans le trigger - Quête: '{questId}' | État: {currentQuestState} | StartPoint: {startPoint} | FinishPoint: {finishPoint} | AutoStart: {autoStartQuest} | AutoFinish: {autoFinishQuest}");

            // Démarrage automatique de la quête si configuré
            if (autoStartQuest && currentQuestState == E_QuestState.CAN_START && startPoint)
            {
                Debug.Log($"[QuestPoint] Démarrage automatique de la quête '{questId}'");
                S_GameManager.instance.questEvents.StartQuest(questId);
            }
            else if (!autoStartQuest && currentQuestState == E_QuestState.CAN_START && startPoint)
            {
                Debug.Log($"[QuestPoint] Quête '{questId}' peut démarrer mais AutoStartQuest est désactivé. Appuyez sur Submit pour démarrer.");
            }
            
            // Finalisation automatique de la quête si configuré
            if (autoFinishQuest && currentQuestState == E_QuestState.CAN_FINISH && finishPoint)
            {
                Debug.Log($"[QuestPoint] Finalisation automatique de la quête '{questId}'");
                S_GameManager.instance.questEvents.FinishQuest(questId);
            }
            else if (currentQuestState == E_QuestState.CAN_FINISH && finishPoint)
            {
                Debug.Log($"[QuestPoint] Quête '{questId}' peut être terminée mais AutoFinish est désactivé. Appuyez sur Submit pour terminer.");
            }
        }
    }

    /**
     * Détecte quand le joueur quitte la zone du point de quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 23rd, 2025.
     * @access	private
     * @param	collider	other	
     * @return	void
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            // Debug.Log("Player left quest point for quest: " + questId + ", current state: " + currentQuestState);
        }
    }
}
