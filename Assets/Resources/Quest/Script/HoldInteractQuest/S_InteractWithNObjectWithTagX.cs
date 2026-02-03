// Quete secondaire : Interagir avec N objets ayant le tag X

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_InteractWithNObjectWithTagX : S_QuestStep
{
    
    [Header("Quest Step Settings")]
    [SerializeField] private string targetTag = "";
    [SerializeField] private int requiredInteractions = 1;
    [SerializeField] private bool destroyObjectOnInteract = false; // Optionnel: détruire l'objet après interaction

    private int currentInteractions = 0;
    private bool isSubscribed = false;
    private bool hasCompleted = false;
    private HashSet<int> interactedObjectIds = new HashSet<int>(); // Pour éviter de compter le même objet plusieurs fois

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

        Debug.Log($"[S_InteractWithNObjectWithTagX] Subscribed - Looking for tag: '{targetTag}', Required: {requiredInteractions}");
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

        Debug.Log($"[S_InteractWithNObjectWithTagX] Received interaction with: {obj.name}, Tag: {obj.tag}");

        if (!IsValidTarget(obj)) return;

        // Vérifier si on n'a pas déjà interagi avec cet objet
        int objectId = obj.GetInstanceID();
        if (interactedObjectIds.Contains(objectId))
        {
            Debug.Log($"[S_InteractWithNObjectWithTagX] Already interacted with {obj.name}, skipping.");
            return;
        }

        interactedObjectIds.Add(objectId);
        IncrementInteraction(obj);
    }

    private bool IsValidTarget(GameObject obj)
    {
        if (obj == null) 
        {
            Debug.Log("[S_InteractWithNObjectWithTagX] Object is null");
            return false;
        }

        // Si aucun tag n'est spécifié, accepter tous les objets
        if (string.IsNullOrEmpty(targetTag))
        {
            Debug.Log($"[S_InteractWithNObjectWithTagX] No target tag specified, accepting {obj.name}");
            return true;
        }

        // Vérifier si le tag correspond
        bool tagMatch = obj.CompareTag(targetTag);
        Debug.Log($"[S_InteractWithNObjectWithTagX] Tag check: '{obj.tag}' == '{targetTag}' ? {tagMatch}");
        
        return tagMatch;
    }

    private void IncrementInteraction(GameObject obj)
    {
        currentInteractions++;
        Debug.Log($"[S_InteractWithNObjectWithTagX] Interacted with valid object: {obj.name}. Progress: {currentInteractions}/{requiredInteractions}");

        if (destroyObjectOnInteract)
        {
            Destroy(obj);
        }

        if (currentInteractions >= requiredInteractions)
        {
            CompleteQuest();
        }
        else
        {
            ChangeState($"Interactions:{currentInteractions}", "IN_PROGRESS");
        }
    }

    private void CompleteQuest()
    {
        hasCompleted = true;
        UnsubscribeFromEvents();
        
        Debug.Log("[S_InteractWithNObjectWithTagX] Quest step completed!");

        ChangeState($"Interactions:{currentInteractions}", "COMPLETE");
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        if (state == "COMPLETE")
        {
            hasCompleted = true;
        }
        else if (state.StartsWith("Interactions:"))
        {
            if (int.TryParse(state.Split(':')[1], out int savedInteractions))
            {
                currentInteractions = savedInteractions;
            }
        }
    }
}