using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class S_PlaceObjectsQuest : S_QuestStep
{
    [Header("Zone")]
    [SerializeField] private GameObject targetZoneParameter;

    [Header("Validation Mode")]
    [SerializeField] private bool useTagMode = true;

    [SerializeField] private string requiredTag = "Ball";
    [SerializeField] private int requiredAmount = 3;

    [Header("Prefab Mode")]
    [SerializeField] private GameObject[] validPrefabs;

    private int placedCount = 0;
    private bool isSubscribed = false;

    private HashSet<GameObject> registeredObjects = new();

    private S_PlaceObjectZone targetZone;

    private void Start()
    {
        targetZone = targetZoneParameter.GetComponent<S_PlaceObjectZone>();
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
    {
        while (!IsQuestStepInitialized() || targetZone == null)
            yield return null;

        Subscribe();
        UpdateState();
    }

    private void Subscribe()
    {
        if (isSubscribed) return;

        targetZone.onObjectPlaced += OnObjectPlaced;
        isSubscribed = true;
    }

    private void OnDisable()
    {
        if (!isSubscribed || targetZone == null) return;

        targetZone.onObjectPlaced -= OnObjectPlaced;
        isSubscribed = false;
    }

    private void OnObjectPlaced(GameObject obj)
    {
        if (registeredObjects.Contains(obj)) return;
        if (!IsValidObject(obj)) return;

        registeredObjects.Add(obj);
        placedCount++;

        Debug.Log($"[S_PlaceObjectsQuest] Objet placé: {obj.name} ({placedCount}/{requiredAmount})");

        UpdateState();

        if (placedCount >= requiredAmount)
            CompleteQuest();
    }

    private bool IsValidObject(GameObject obj)
    {
        if (useTagMode)
            return obj.CompareTag(requiredTag);

        foreach (var prefab in validPrefabs)
            if (prefab != null && obj.name.Contains(prefab.name))
                return true;

        return false;
    }

    private void UpdateState()
    {
        ChangeState($"{placedCount}/{requiredAmount}", $"Placés: {placedCount}/{requiredAmount}");
    }

    private void CompleteQuest()
    {
        Debug.Log("[S_PlaceObjectsQuest] Quête complétée !");
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        if (string.IsNullOrEmpty(state)) return;

        var parts = state.Split('/');
        if (parts.Length == 2 && int.TryParse(parts[0], out int count))
            placedCount = count;
    }
}
