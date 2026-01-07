using UnityEngine;
using System.Collections;

/**
 * Quête pour collecter toutes les clés attribuées à une porte spécifique.
 * Utilise le système d'événements du GameManager pour détecter la collecte de clés.
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Sunday, January 5th, 2026.
 * @global
 */
public class S_CollectAllKeysQuest : S_QuestStep
{
    [Header("Quest Settings")]
    [SerializeField] private string targetDoorID = "door_01"; // ID de la porte dont il faut collecter les clés
    [SerializeField] private int requiredKeyCount = 1; // Nombre de clés requises
    
    
    private bool questCompleted = false;
    private bool isSubscribed = false;

    private int collectedKeysCount = 0;   // état de la quête (UI / save), PAS source de vérité


    // *==========================================================================

    private void Start()
    {
        Debug.Log("[CollectAllKeysQuest] Starting quest step");
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
{
    Debug.Log("[CollectAllKeysQuest] Waiting for GameManager & KeyManager...");

    while (S_GameManager.instance == null || S_KeyManager.instance == null)
        yield return null;

    Debug.Log("[CollectAllKeysQuest] Managers ready");

    collectedKeysCount = S_KeyManager.instance.GetCollectedKeyCount(targetDoorID);
    Debug.Log($"[CollectAllKeysQuest] Initial key count for '{targetDoorID}': {collectedKeysCount}");

    ChangeState($"{collectedKeysCount}/{requiredKeyCount}",
                $"Clés: {collectedKeysCount}/{requiredKeyCount}");

    if (collectedKeysCount >= requiredKeyCount)
    {
        Debug.Log("[CollectAllKeysQuest] Already completed at init");
        CompleteQuest();
        yield break;
    }

    SubscribeToEvents();
}



    private void SubscribeToEvents()
{
    if (S_GameManager.instance == null)
    {
        Debug.LogWarning("[CollectAllKeysQuest] Subscribe failed: GameManager null");
        return;
    }

    if (isSubscribed)
    {
        Debug.Log("[CollectAllKeysQuest] Already subscribed");
        return;
    }

    S_GameManager.instance.playerEvents.onKeyCollected += OnKeyCollected;
    isSubscribed = true;
    Debug.Log("[CollectAllKeysQuest] Subscribed to onKeyCollected");
}


    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;
        
        S_GameManager.instance.playerEvents.onKeyCollected -= OnKeyCollected;
        isSubscribed = false;
        Debug.Log("[S_CollectAllKeysQuest] Unsubscribed from onKeyCollected event");
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /**
     * Callback appelé quand le joueur ramasse une clé
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	private
     * @param	GameObject	key   	L'objet clé ramassé
     * @param	string    	doorID	L'ID de la porte associée
     * @param	string    	keyID 	L'ID unique de la clé
     * @return	void
     */
    private void OnKeyCollected(GameObject key, string doorID, string keyID)
    {
        Debug.Log($"[CollectAllKeysQuest] 🗝 Event received → key:{key?.name}, door:{doorID}, questCompleted:{questCompleted}");

        if (questCompleted) return;

        if (IsTargetKey(doorID))
            CollectKey(key, keyID);
    }


    /**
     * Vérifie si la clé est pour la porte recherchée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	private
     * @param	string	doorID	L'ID de la porte associée à la clé
     * @return	boolean
     */
    private bool IsTargetKey(string doorID)
    {
        if (string.IsNullOrEmpty(doorID))
        {
            Debug.LogWarning("[S_CollectAllKeysQuest] Door ID is null or empty");
            return false;
        }

        // Vérifier si c'est une clé pour la porte ciblée
        if (doorID != targetDoorID)
        {
            Debug.Log($"[S_CollectAllKeysQuest] Key is for door '{doorID}', not for target door '{targetDoorID}'");
            return false;
        }

        Debug.Log($"[S_CollectAllKeysQuest] Key is for target door '{targetDoorID}'!");
        return true;
    }

    /**
     * Appelé quand le joueur ramasse une clé valide
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	private
     * @param	GameObject	key  	L'objet clé ramassé
     * @param	string    	keyID	L'ID de la clé
     * @return	void
     */
    private void CollectKey(GameObject key, string keyID)
    {
        Debug.Log("[CollectAllKeysQuest] CollectKey called");

        if (questCompleted) return;

        int previous = collectedKeysCount;
        collectedKeysCount = S_KeyManager.instance.GetCollectedKeyCount(targetDoorID);

        Debug.Log($"[CollectAllKeysQuest] Count changed: {previous} → {collectedKeysCount} / {requiredKeyCount}");

        ChangeState($"{collectedKeysCount}/{requiredKeyCount}",
                    $"Clés: {collectedKeysCount}/{requiredKeyCount}");

        if (collectedKeysCount >= requiredKeyCount)
            CompleteQuest();
    }




    /**
     * Termine la quête quand toutes les clés sont collectées
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	private
     * @return	void
     */
    private void CompleteQuest()
    {
        if (questCompleted) return;

        questCompleted = true;
        Debug.Log($"[CollectAllKeysQuest] QUEST COMPLETE for door '{targetDoorID}'");

        ChangeState("COMPLETE", "Toutes les clés collectées");
        FinishQuestStep();
    }


    /**
     * Permet de charger l'état de la quest step depuis une sauvegarde
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	protected
     * @param	string	state	
     * @return	void
     */
    protected override void SetQuestStepState(string state)
    {
        Debug.Log($"[S_CollectAllKeysQuest] Loading state: {state}");

        if (state == "COMPLETE")
        {
            questCompleted = true;
        }
        else if (!string.IsNullOrEmpty(state) && state.Contains("/"))
        {
            // Parser l'état sauvegardé (format: "X/Y")
            string[] parts = state.Split('/');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int savedCount))
            {
                collectedKeysCount = savedCount;
            }
        }
    }
}
