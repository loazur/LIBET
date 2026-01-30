// Quete secondaire : Interagir avec N objets ayant le tag X

using UnityEngine;

class S_InteractWithNObjectWithTagX : S_QuestStep
{
    
    [Header("Quest Settings")]
    [SerializeField] private string targetTag = "";
    [SerializeField] private int requiredInteractions = 1;

    private int currentInteractions = 0;
    private bool isSubscribed = false;
    private bool hasCompleted = false;

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

        Debug.Log("[S_InteractWithNObjectWithTagX] Subscribed");
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
        if (obj.CompareTag(targetTag))
        {
            return true;
        }
        return false;
    }

    private void CompleteQuest(GameObject obj)
    {
        currentInteractions++;
        Debug.Log($"[S_InteractWithNObjectWithTagX] Interacted with valid object: {obj.name}. Current interactions: {currentInteractions}/{requiredInteractions}");

        if (currentInteractions >= requiredInteractions)
        {
            hasCompleted = true;
            Debug.Log("[S_InteractWithNObjectWithTagX] Quest step completed!");
            OnQuestStepCompleted();
            UnsubscribeFromEvents();
        }
    }


    protected override void SetQuestStepState(string state)
    {
        
    }

}