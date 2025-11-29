using UnityEngine;
using System.Collections;

/**
 * Quête pour ouvrir une porte. Détecte quand le joueur ouvre un objet avec S_DoorInteractable
 * Utilise le système d'événements du GameManager pour détecter l'ouverture
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Friday, November 29th, 2025.
 * @global
 */
public class S_OpenDoorQuest : S_QuestStep
{
    [Header("Quest Settings")]
    [SerializeField] private string doorTag = "Door"; // Tag de la porte (optionnel)
    [SerializeField] private string specificDoorName = ""; // Optionnel : nom spécifique de la porte
    
    private bool hasOpened = false;
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

        Debug.Log("[S_OpenDoorQuest] GameManager ready, subscribing to events");
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed) return;
        
        S_GameManager.instance.playerEvents.onDoorOpened += OnDoorOpened;
        isSubscribed = true;
        Debug.Log("[S_OpenDoorQuest] Subscribed to onDoorOpened event");
    }

    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;
        
        S_GameManager.instance.playerEvents.onDoorOpened -= OnDoorOpened;
        isSubscribed = false;
        Debug.Log("[S_OpenDoorQuest] Unsubscribed from onDoorOpened event");
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /**
     * Callback appelé quand le joueur ouvre une porte
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	private
     * @param	gameobject	door	
     * @return	void
     */
    private void OnDoorOpened(GameObject door)
    {
        if (hasOpened) return;

        Debug.Log($"[S_OpenDoorQuest] Player opened door: {door.name}");

        if (IsTargetDoor(door))
        {
            OpenDoor();
        }
    }

    /**
     * Vérifie si l'objet est la porte recherchée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	private
     * @param	gameobject	obj	
     * @return	boolean
     */
    private bool IsTargetDoor(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[S_OpenDoorQuest] Door object is null");
            return false;
        }

        // Vérifier si l'objet a le component S_DoorInteractable
        S_DoorInteractable doorComponent = obj.GetComponent<S_DoorInteractable>();
        if (doorComponent == null)
        {
            Debug.Log($"[S_OpenDoorQuest] {obj.name} doesn't have S_DoorInteractable component");
            return false;
        }

        // Si un tag spécifique est défini, vérifier le tag
        if (!string.IsNullOrEmpty(doorTag))
        {
            if (!obj.CompareTag(doorTag))
            {
                Debug.Log($"[S_OpenDoorQuest] {obj.name} tag '{obj.tag}' doesn't match required tag '{doorTag}'");
                return false;
            }
        }

        // Si un nom spécifique est défini, vérifier le nom
        if (!string.IsNullOrEmpty(specificDoorName))
        {
            if (obj.name != specificDoorName)
            {
                Debug.Log($"[S_OpenDoorQuest] {obj.name} doesn't match required name '{specificDoorName}'");
                return false;
            }
        }

        Debug.Log($"[S_OpenDoorQuest] {obj.name} is the target door!");
        return true;
    }

    /**
     * Appelé quand le joueur ouvre la bonne porte
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	private
     * @return	void
     */
    private void OpenDoor()
    {
        if (hasOpened) return;

        hasOpened = true;
        Debug.Log("[S_OpenDoorQuest] Quest completed - player opened the door!");

        ChangeState("Player opened door", "COMPLETE");
        FinishQuestStep();
    }

    /**
     * Permet de charger l'état de la quest step depuis une sauvegarde
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	protected
     * @param	string	state	
     * @return	void
     */
    protected override void SetQuestStepState(string state)
    {
        Debug.Log($"[S_OpenDoorQuest] Loading state: {state}");

        if (state == "COMPLETE")
        {
            hasOpened = true;
        }
    }
}
