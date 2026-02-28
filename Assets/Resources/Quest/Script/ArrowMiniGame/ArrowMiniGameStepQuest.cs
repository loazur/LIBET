using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Étape de quête : Compléter le mini-jeu Arrow (S_ArrowMinigame)
 * Utilise l'événement global onMinigameCompleted et filtre par tag.
 *
 * @author  Lucas
 * @since   v0.0.1
 * @version v1.0.0  Thursday, February 12th, 2026.
 * @global
 */
public class ArrowMiniGameStepQuest : S_QuestStep
{
    [Header("Quest Settings")]
    [Tooltip("Tag du GameObject du mini-jeu Arrow à compléter")]
    [SerializeField] private string requiredTag = "";

    private bool hasCompleted = false;
    private bool isSubscribed = false;

    private void Start()
    {
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
    {
        while (S_GameManager.instance == null)
            yield return null;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (isSubscribed || S_GameManager.instance == null) return;

        S_GameManager.instance.playerEvents.onMinigameCompleted += OnMinigameCompleted;
        isSubscribed = true;

        Debug.Log("[ArrowMiniGameStepQuest] Subscribed");
    }

    private void UnsubscribeFromEvents()
    {
        if (!isSubscribed || S_GameManager.instance == null) return;

        S_GameManager.instance.playerEvents.onMinigameCompleted -= OnMinigameCompleted;
        isSubscribed = false;
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void OnMinigameCompleted(GameObject minigameObject)
    {
        if (hasCompleted) return;
        if (minigameObject == null) return;

        if (!string.IsNullOrEmpty(requiredTag) && !minigameObject.CompareTag(requiredTag))
            return;

        hasCompleted = true;
        Debug.Log($"[ArrowMiniGameStepQuest] Mini-jeu Arrow complété : {minigameObject.name}");
        ChangeState("ArrowMinigameCompleted", "COMPLETE");
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        if (state == "COMPLETE")
            hasCompleted = true;
    }
}