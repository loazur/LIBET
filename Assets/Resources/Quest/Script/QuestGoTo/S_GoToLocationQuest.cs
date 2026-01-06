using UnityEngine;
using System.Collections;

public class S_GoToLocationQuest : S_QuestStep
{
    [Header("Conditions")]
    [SerializeField] private string playerTag = "Player";

    private bool isCompleted = false;
    private bool isSubscribed = false;

    private S_GoToZone targetZone;

    private void Start()
    {
        GameObject zoneObj = GameObject.FindWithTag("QuestZone");

        if (zoneObj == null)
        {
            Debug.LogError("[S_GoToLocationQuest] No object with tag 'QuestZone' found in scene");
            enabled = false;
            return;
        }

        targetZone = zoneObj.GetComponent<S_GoToZone>();

        if (targetZone == null)
        {
            Debug.LogError($"[S_GoToLocationQuest] '{zoneObj.name}' has no S_GoToZone component");
            enabled = false;
            return;
        }

        Debug.Log($"[S_GoToLocationQuest] Connected to zone: {zoneObj.name}");
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
