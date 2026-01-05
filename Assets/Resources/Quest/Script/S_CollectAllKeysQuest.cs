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
    
    private int collectedKeysCount = 0;
    private bool questCompleted = false;
    private bool isSubscribed = false;

    // *==========================================================================

    private void Start()
    {
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
    {
        // Attendre que S_GameManager soit initialisé
        while (S_GameManager.instance == null)
        {
            yield return null;
        }

        Debug.Log("[S_CollectAllKeysQuest] GameManager ready, subscribing to events");
        
        // Récupérer le nombre de clés déjà collectées
        if (S_KeyManager.instance != null)
        {
            collectedKeysCount = S_KeyManager.instance.GetCollectedKeyCount(targetDoorID);
        }

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed) return;
        
        S_GameManager.instance.playerEvents.onKeyCollected += OnKeyCollected;
        isSubscribed = true;
        Debug.Log("[S_CollectAllKeysQuest] Subscribed to onKeyCollected event");
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
        if (questCompleted) return;

        Debug.Log($"[S_CollectAllKeysQuest] Player collected key: {key.name} for door: {doorID}");

        if (IsTargetKey(doorID))
        {
            CollectKey(key, keyID);
        }
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
        if (questCompleted) return;

        collectedKeysCount++;
        Debug.Log($"[S_CollectAllKeysQuest] Key '{keyID}' collected! Progress: {collectedKeysCount}/{requiredKeyCount}");

        // Mettre à jour l'état de la quête
        ChangeState($"{collectedKeysCount}/{requiredKeyCount}", $"Clés: {collectedKeysCount}/{requiredKeyCount}");

        // Vérifier si toutes les clés ont été collectées
        if (collectedKeysCount >= requiredKeyCount)
        {
            CompleteQuest();
        }
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
        Debug.Log($"[S_CollectAllKeysQuest] Quest completed - all {requiredKeyCount} keys collected for door '{targetDoorID}'!");

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
