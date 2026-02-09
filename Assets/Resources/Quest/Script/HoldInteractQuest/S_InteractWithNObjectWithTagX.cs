using UnityEngine;
using System.Collections.Generic;

public class S_InteractWithNObjectWithTagX : S_QuestStep
{
    [Header("Quest Step Settings")]
    [SerializeField] private string targetTag = "";
    [SerializeField] private int requiredInteractions = 1;
    [SerializeField] private bool destroyObjectOnInteract = false;

    private int currentInteractions = 0;
    private bool hasCompleted = false;

    // Pour éviter de compter deux fois le même objet
    private HashSet<int> interactedObjectIds = new HashSet<int>();

    private void OnEnable()
    {
        if (S_GameManager.instance == null) return;

        S_GameManager.instance.playerEvents.onPlayerHoldInteractedWithAnyObject
            += OnPlayerHoldInteracted;

        Debug.Log($"[S_InteractWithNObjectWithTagX] Enabled - Tag: {targetTag}, Required: {requiredInteractions}");
    }

    private void OnDisable()
    {
        if (S_GameManager.instance == null) return;

        S_GameManager.instance.playerEvents.onPlayerHoldInteractedWithAnyObject
            -= OnPlayerHoldInteracted;
    }

    private void OnPlayerHoldInteracted(GameObject obj)
    {
        if (hasCompleted || obj == null)
            return;

        if (!IsValidTarget(obj))
            return;

        int id = obj.GetInstanceID();
        if (interactedObjectIds.Contains(id))
        {
            Debug.Log($"[QuestStep] {obj.name} already counted");
            return;
        }

        interactedObjectIds.Add(id);
        currentInteractions++;

        Debug.Log($"[QuestStep] Progress {currentInteractions}/{requiredInteractions}");

        if (destroyObjectOnInteract)
            Destroy(obj);

        if (currentInteractions >= requiredInteractions)
        {
            CompleteStep();
        }
        else
        {
            ChangeState($"Interactions:{currentInteractions}", "IN_PROGRESS");
        }
    }

    private bool IsValidTarget(GameObject obj)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            return true;
        }

        return obj.CompareTag(targetTag);
    }

    private void CompleteStep()
    {
        hasCompleted = true;

        ChangeState($"Interactions:{currentInteractions}", "COMPLETE");
        FinishQuestStep();

        Debug.Log("[QuestStep] Step completed");
    }

    protected override void SetQuestStepState(string state)
    {
        if (state == "COMPLETE")
        {
            hasCompleted = true;
            return;
        }

        if (state.StartsWith("Interactions:"))
        {
            if (int.TryParse(state.Split(':')[1], out int value))
            {
                currentInteractions = value;
            }
        }
    }
}
