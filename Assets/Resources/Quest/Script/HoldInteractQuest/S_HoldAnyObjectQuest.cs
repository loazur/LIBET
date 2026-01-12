using UnityEngine;
using System.Collections;

/**
 * Quête : Maintenir une interaction sur un objet
 *
 * @author Lucas
 */
public class S_HoldAnyObjectQuest : S_QuestStep
{
    [Header("Quest Settings")]
    [SerializeField] private string requiredTag = "";          // optionnel
    [SerializeField] private string specificObjectName = "";   // optionnel

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

        S_GameManager.instance.playerEvents.onPlayerHoldInteractedWithAnyObject += OnPlayerHoldInteracted;
        isSubscribed = true;

        Debug.Log("[S_HoldAnyObjectQuest] Subscribed");
    }

    private void UnsubscribeFromEvents()
    {
        if (!isSubscribed || S_GameManager.instance == null) return;

        S_GameManager.instance.playerEvents.onPlayerHoldInteractedWithAnyObject -= OnPlayerHoldInteracted;
        isSubscribed = false;
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void OnPlayerHoldInteracted(GameObject obj)
    {
        if (hasCompleted) return;

        if (!IsValidTarget(obj)) return;

        CompleteQuest(obj);
    }

    private bool IsValidTarget(GameObject obj)
    {
        if (obj == null) return false;

        if (!string.IsNullOrEmpty(requiredTag) && !obj.CompareTag(requiredTag))
            return false;

        if (!string.IsNullOrEmpty(specificObjectName) && obj.name != specificObjectName)
            return false;

        return true;
    }

    private void CompleteQuest(GameObject obj)
    {
        hasCompleted = true;

        Debug.Log($"[S_HoldAnyObjectQuest] Completed with {obj.name}");

        ChangeState($"HoldInteraction:{obj.name}", "COMPLETE");
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        if (state == "COMPLETE")
            hasCompleted = true;
    }
}
