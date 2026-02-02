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

    // Propriété publique pour accéder à l'ID de la quête
    public string QuestId => questInfoForPoint != null ? questInfoForPoint.id : string.Empty;

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
    private bool hasTriggeredStart = false; // Protection contre appels multiples
    private bool hasTriggeredFinish = false; // Protection contre appels multiples

    // *----------------------------------------------------------------*

    private void Awake() 
    {
        questId = questInfoForPoint.id;
        // État par défaut - sera mis à jour par QuestStateChange dès l'abonnement
        currentQuestState = E_QuestState.REQUIREMENTS_NOT_MET;
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
        // Ne rien faire si pas abonné
        if (!isSubscribed) return;

        // Auto-démarrage (sans zone) - avec protection contre appels multiples
        if (autoStartQuest && currentQuestState == E_QuestState.CAN_START && !hasTriggeredStart)
        {
            hasTriggeredStart = true;
            Debug.Log($"<color=green>[QuestPoint]</color> Auto-démarrage de '{questId}' (état actuel: {currentQuestState})");
            S_GameManager.instance.questEvents.StartQuest(questId);
        }

        // Auto-finalisation (sans zone) - avec protection contre appels multiples
        if (autoFinishQuest && currentQuestState == E_QuestState.CAN_FINISH && !hasTriggeredFinish)
        {
            hasTriggeredFinish = true;
            Debug.Log($"<color=green>[QuestPoint]</color> Auto-finalisation de '{questId}'");
            S_GameManager.instance.questEvents.FinishQuest(questId);
        }
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed) return;

        S_GameManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        S_GameManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
        isSubscribed = true;
        
        Debug.Log($"<color=blue>[QuestPoint]</color> '{questId}' abonné aux événements (autoStart={autoStartQuest}, startPoint={startPoint})");
    }

    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;

        S_GameManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        S_GameManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        isSubscribed = false;
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
            Debug.Log($"<color=green>[QuestPoint]</color> Démarrage manuel (Submit) de '{questId}'");
            S_GameManager.instance.questEvents.StartQuest(questId);
        }

        // Fin manuelle dans la zone
        if (finishPoint && requireSubmitToFinish && currentQuestState == E_QuestState.CAN_FINISH)
        {
            Debug.Log($"<color=green>[QuestPoint]</color> Finalisation manuelle (Submit) de '{questId}'");
            S_GameManager.instance.questEvents.FinishQuest(questId);
        }
    }

    /**
     * Met à jour l'état de la quête associée à ce point
     */
    private void QuestStateChange(S_Quest quest)
    {
        if (!quest.info.id.Equals(questId)) return;

        E_QuestState oldState = currentQuestState;
        currentQuestState = quest.state;
        
        Debug.Log($"<color=yellow>[QuestPoint]</color> '{questId}': {oldState} → {currentQuestState}");

        // Reset les flags si l'état change
        if (currentQuestState == E_QuestState.CAN_START)
        {
            hasTriggeredStart = false;
        }
        if (currentQuestState == E_QuestState.CAN_FINISH)
        {
            hasTriggeredFinish = false;
        }
    }

    /**
     * Détecte quand le joueur entre dans la zone du point de quête
     */
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerIsNear = true;

        // Démarrage automatique dans la zone (sans Submit)
        if (startPoint && !requireSubmitToStart && currentQuestState == E_QuestState.CAN_START)
        {
            Debug.Log($"<color=green>[QuestPoint]</color> Démarrage (zone) de '{questId}'");
            S_GameManager.instance.questEvents.StartQuest(questId);
        }

        // Fin automatique dans la zone (sans Submit)
        if (finishPoint && !requireSubmitToFinish && currentQuestState == E_QuestState.CAN_FINISH)
        {
            Debug.Log($"<color=green>[QuestPoint]</color> Finalisation (zone) de '{questId}'");
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