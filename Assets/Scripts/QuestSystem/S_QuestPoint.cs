/**
 * S_QuestPoint.cs
 * 
 * Représente un point de quête dans le jeu.
 * Gère le démarrage et la fin des quêtes via zones ou automatiquement.
 * 
 * LOGIQUE :
 * - startPoint/finishPoint : Ce point peut déclencher start/finish via zone
 * - requireSubmitToStart/Finish : Nécessite Submit dans la zone (sinon automatique dans zone)
 * - autoStartQuest : Démarre AUTOMATIQUEMENT dès que CAN_START (sans zone)
 * - autoFinishQuest : Termine AUTOMATIQUEMENT dès que CAN_FINISH (sans zone)
**/

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class S_QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private SO_QuestInfo questInfoForPoint;

    [Header("Zone Interaction")]
    [Tooltip("Ce QuestPoint peut démarrer la quête quand le joueur entre dans la zone")]
    [SerializeField] private bool startPoint = true;
    
    [Tooltip("Ce QuestPoint peut terminer la quête quand le joueur entre dans la zone")]
    [SerializeField] private bool finishPoint = true;
    
    [Tooltip("Nécessite d'appuyer sur Submit pour démarrer dans la zone (sinon automatique)")]
    [SerializeField] private bool requireSubmitToStart = false;
    
    [Tooltip("Nécessite d'appuyer sur Submit pour terminer dans la zone (sinon automatique)")]
    [SerializeField] private bool requireSubmitToFinish = false;
    
    [Header("Automatic Quest Control (Global)")]
    [Tooltip("Démarre automatiquement la quête dès que CAN_START (sans zone)")]
    [SerializeField] private bool autoStartQuest = false;
    
    [Tooltip("Termine automatiquement la quête dès que CAN_FINISH (sans zone)")]
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

    private void Update()
    {
        // Surveillance pour démarrage automatique (sans zone)
        if (autoStartQuest && currentQuestState == E_QuestState.CAN_START)
        {
            // Debug.Log($"<color=cyan>[QuestPoint]</color> Auto-démarrage de '{questId}' (CAN_START détecté)");
            S_GameManager.instance.questEvents.StartQuest(questId);
        }

        // Surveillance pour fin automatique (sans zone)
        if (autoFinishQuest && currentQuestState == E_QuestState.CAN_FINISH)
        {
            // Debug.Log($"<color=cyan>[QuestPoint]</color> Auto-finalisation de '{questId}' (CAN_FINISH détecté)");
            S_GameManager.instance.questEvents.FinishQuest(questId);
        }
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

    private void OnEnable()
    {
        // L'abonnement est géré par InitializeWhenReady() dans Start()
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /**
     * Gère l'appui sur Submit quand le joueur est dans la zone
     */
    private void SubmitPressed(E_InputEventContext inputEventContext)
    {
        if (!playerIsNear || inputEventContext != E_InputEventContext.DEFAULT)
            return;

        // Démarrage manuel dans la zone
        if (startPoint && requireSubmitToStart && currentQuestState == E_QuestState.CAN_START)
        {
            // Debug.Log($"<color=green>[QuestPoint]</color> Démarrage manuel (Submit) de '{questId}'");
            S_GameManager.instance.questEvents.StartQuest(questId);
        }

        // Fin manuelle dans la zone
        if (finishPoint && requireSubmitToFinish && currentQuestState == E_QuestState.CAN_FINISH)
        {
            // Debug.Log($"<color=green>[QuestPoint]</color> Finalisation manuelle (Submit) de '{questId}'");
            S_GameManager.instance.questEvents.FinishQuest(questId);
        }
    }

    /**
     * Met à jour l'état de la quête associée à ce point
     */
    private void QuestStateChange(S_Quest quest)
    {
        if (quest.info.id.Equals(questId))
        {
            E_QuestState oldState = currentQuestState;
            currentQuestState = quest.state;
            Debug.Log($"<color=yellow>[QuestPoint]</color> État de '{questId}': {oldState} → {currentQuestState} ");
        }
    }

    /**
     * Détecte quand le joueur entre dans la zone du point de quête
     */
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerIsNear = true;
        // Debug.Log($"<color=cyan>[QuestPoint]</color> Joueur entré dans zone de '{questId}'");

        // Démarrage automatique dans la zone (sans Submit)
        if (startPoint && !requireSubmitToStart && currentQuestState == E_QuestState.CAN_START)
        {
            // Debug.Log($"<color=green>[QuestPoint]</color> Démarrage automatique (zone) de '{questId}'");
            S_GameManager.instance.questEvents.StartQuest(questId);
        }

        // Fin automatique dans la zone (sans Submit)
        if (finishPoint && !requireSubmitToFinish && currentQuestState == E_QuestState.CAN_FINISH)
        {
            // Debug.Log($"<color=green>[QuestPoint]</color> Finalisation automatique (zone) de '{questId}'");
            S_GameManager.instance.questEvents.FinishQuest(questId);
        }
    }

    /**
     * Détecte quand le joueur quitte la zone du point de quête
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}