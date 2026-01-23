using UnityEngine;
using System.Collections;

public class S_GoToLocationQuest : S_QuestStep
{
    [Header("Zone Identification")]
    [Tooltip("L'ID unique de la zone cible (doit correspondre à l'ID sur le S_GoToZone)")]
    [SerializeField] private string targetZoneId = "";

    [Header("Conditions")]
    [SerializeField] private string playerTag = "Player";

    private bool isCompleted = false;
    private bool isSubscribed = false;
    private S_GoToZone targetZone = null;

    private void Start()
    {
        StartCoroutine(FindZoneAndInitialize());
    }

    private IEnumerator FindZoneAndInitialize()
    {
        // Attendre une frame pour que les zones s'enregistrent
        yield return null;

        targetZone = S_GoToZone.GetZoneById(targetZoneId);

        if (targetZone == null)
        {
            Debug.LogError($"[S_GoToLocationQuest] No zone found with ID '{targetZoneId}'");
            enabled = false;
            yield break;
        }

        Debug.Log($"[S_GoToLocationQuest] Connected to zone: {targetZone.name} (ID: {targetZoneId})");
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
    {
        while (!IsQuestStepInitialized())
            yield return null;

        Subscribe();
        UpdateState();
    }

    private void Subscribe()
    {
        if (isSubscribed) return;

        targetZone.onEntityEntered += OnEntityEntered;
        isSubscribed = true;
    }

    private void OnDisable()
    {
        if (!isSubscribed || targetZone == null) return;

        targetZone.onEntityEntered -= OnEntityEntered;
        isSubscribed = false;
    }

    private void OnEntityEntered(GameObject entity)
    {
        if (isCompleted) return;

        Debug.Log($"[S_GoToLocationQuest] Entity entered: {entity.name}");

        if (!entity.CompareTag(playerTag)) return;

        Debug.Log("[S_GoToLocationQuest] Player reached destination");
        CompleteQuest();
    }

    private void UpdateState()
    {
        ChangeState(isCompleted ? "1/1" : "0/1", "Se rendre à l'emplacement");
    }

    private void CompleteQuest()
    {
        if (isCompleted) return;

        isCompleted = true;
        UpdateState();

        Debug.Log("[S_GoToLocationQuest] Quest completed");
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        if (state == "1/1" || state == "complete")
        {
            isCompleted = true;
        }
    }
}
