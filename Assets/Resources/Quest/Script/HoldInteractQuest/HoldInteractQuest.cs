using UnityEngine;
using System.Collections;


class HoldInteractQuest : S_QuestStep
{

    [Header("Quest Settings")]
    [SerializeField] private string interactTag = "HoldInteract"; // Tag de l'objet
    [SerializeField] private string specificObjectName = ""; // Optionnel : nom spécifique de l'objet

    private bool hasInteracted = false;
    private bool isSubscribed = false;

    // *==========================================================================

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

        // S'abonner à l'événement d'interaction
        SubscribeToEvents();
        Debug.Log("[HoldInteractQuest] Quête d'interaction initialisée.");
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed) return;

        S_GameManager.instance.playerEvents.onPlayerHoldInteracted += OnPlayerHoldInteracted;
        isSubscribed = true;
    }

    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;

        S_GameManager.instance.playerEvents.onPlayerHoldInteracted -= OnPlayerHoldInteracted;
        isSubscribed = false;
    }

    private bool InteractedWithCorrectObject(string objectName, string objectTag)
    {
        if (objectTag != interactTag)
            return false;

        if (!string.IsNullOrEmpty(specificObjectName) && objectName != specificObjectName)
            return false;

        return true;
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void OnPlayerHoldInteracted(string objectName, string objectTag)
    {
        if (hasInteracted) return;

        if (InteractedWithCorrectObject(objectName, objectTag))
        {
            HasInteracted();
        }
    }


    private void HasInteracted()
    {
        if (hasInteracted) return;

        hasInteracted = true;
        Debug.Log("[HoldInteractQuest] Interaction detected, completing quest step");
        ChangeState("Player Interacted with object", "COMPLETE");

        FinishQuestStep();

    }


    protected override void SetQuestStepState(string state)
    {
        Debug.Log($"[HoldInteractQuest] Loading state: {state}");

        if (state == "COMPLETE")
        {
            hasInteracted = true;
            // Ne PAS appeler FinishQuestStep() ici !
            // L'étape est déjà complète et l'index a déjà avancé
        }
    }

}