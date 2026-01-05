using UnityEngine;
using System.Collections;

/**
 * Quête pour collecter toutes les clés attribuées à une porte spécifique.
 * Détecte quand le joueur ramasse des clés via le S_KeyManager.
 * La quête se termine quand toutes les clés requises sont collectées.
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Sunday, January 5th, 2026.
 * @global
 */
public class S_CollectAllKeysQuest : S_QuestStep
{
    [Header("Quest Settings")]
    [Tooltip("L'ID de la porte dont il faut collecter les clés")]
    [SerializeField] private string targetDoorID = "door_01";
    
    [Tooltip("Nombre de clés requises pour terminer la quête")]
    [SerializeField] private int requiredKeyCount = 1;

    private int collectedKeysCount = 0;
    private bool questCompleted = false;
    private bool isSubscribed = false;

    // *==========================================================================

    private void Start()
    {
        StartCoroutine(InitializeWhenReady());
    }

    /**
     * Attends que le GameManager et le KeyManager soient initialisés avant de s'abonner aux événements
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	private
     * @return	IEnumerator
     */
    private IEnumerator InitializeWhenReady()
    {
        // Attendre que S_GameManager soit initialisé
        while (S_GameManager.instance == null)
        {
            yield return null;
        }

        // Attendre que S_KeyManager soit initialisé
        while (S_KeyManager.instance == null)
        {
            yield return null;
        }

        Debug.Log($"[S_CollectAllKeysQuest] Managers ready, subscribing to events for door '{targetDoorID}'");
        
        // Initialiser le compteur avec les clés déjà collectées
        collectedKeysCount = S_KeyManager.instance.GetCollectedKeyCount(targetDoorID);
        
        // Vérifier si la quête est déjà complète
        if (collectedKeysCount >= requiredKeyCount)
        {
            Debug.Log($"[S_CollectAllKeysQuest] All keys already collected for door '{targetDoorID}'");
            CompleteQuest();
            yield break;
        }

        SubscribeToEvents();
        UpdateQuestState();
    }

    /**
     * S'abonne aux événements du KeyManager
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	private
     * @return	void
     */
    private void SubscribeToEvents()
    {
        if (S_KeyManager.instance == null || isSubscribed) return;

        S_KeyManager.instance.OnKeyCollected += OnKeyCollected;
        isSubscribed = true;
        Debug.Log("[S_CollectAllKeysQuest] Subscribed to OnKeyCollected event");
    }

    /**
     * Se désabonne des événements du KeyManager
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	private
     * @return	void
     */
    private void UnsubscribeFromEvents()
    {
        if (S_KeyManager.instance == null || !isSubscribed) return;

        S_KeyManager.instance.OnKeyCollected -= OnKeyCollected;
        isSubscribed = false;
        Debug.Log("[S_CollectAllKeysQuest] Unsubscribed from OnKeyCollected event");
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /**
     * Callback appelé quand le joueur collecte une clé
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	private
     * @param	string	doorID	L'ID de la porte associée à la clé
     * @param	string	keyID 	L'ID unique de la clé collectée
     * @return	void
     */
    private void OnKeyCollected(string doorID, string keyID)
    {
        if (questCompleted) return;

        // Vérifier que la quête est bien initialisée (active)
        if (!IsQuestStepInitialized())
        {
            Debug.LogWarning($"[S_CollectAllKeysQuest] Key '{keyID}' collected but quest step not yet initialized. Waiting...");
            return;
        }

        // Vérifier si c'est une clé pour la porte ciblée
        if (doorID != targetDoorID)
        {
            Debug.Log($"[S_CollectAllKeysQuest] Key '{keyID}' collected but for different door '{doorID}' (looking for '{targetDoorID}')");
            return;
        }

        collectedKeysCount = S_KeyManager.instance.GetCollectedKeyCount(targetDoorID);
        Debug.Log($"[S_CollectAllKeysQuest] Key '{keyID}' collected for door '{targetDoorID}'. Progress: {collectedKeysCount}/{requiredKeyCount}");

        UpdateQuestState();

        // Vérifier si toutes les clés ont été collectées
        if (collectedKeysCount >= requiredKeyCount)
        {
            CompleteQuest();
        }
    }

    /**
     * Met à jour l'état de la quête avec la progression actuelle
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	private
     * @return	void
     */
    private void UpdateQuestState()
    {
        string state = $"{collectedKeysCount}/{requiredKeyCount}";
        string status = $"Clés collectées: {collectedKeysCount}/{requiredKeyCount}";
        ChangeState(state, status);
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
        Debug.Log($"[S_CollectAllKeysQuest] Quest completed! All {requiredKeyCount} keys collected for door '{targetDoorID}'");
        
        UnsubscribeFromEvents();
        FinishQuestStep();
    }

    /**
     * Restaure l'état de la quête depuis une sauvegarde
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 5th, 2026.
     * @access	protected
     * @param	string	state	L'état sauvegardé (format: "collectedCount/requiredCount")
     * @return	void
     */
    protected override void SetQuestStepState(string state)
    {
        // Parser l'état sauvegardé (format: "X/Y")
        if (!string.IsNullOrEmpty(state) && state.Contains("/"))
        {
            string[] parts = state.Split('/');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int savedCount))
            {
                collectedKeysCount = savedCount;
                Debug.Log($"[S_CollectAllKeysQuest] State restored: {collectedKeysCount}/{requiredKeyCount}");
            }
        }
    }
}
