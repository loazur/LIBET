using UnityEngine;
using System.Collections;

/**
 * Quête pour maintenir une interaction. Détecte quand le joueur maintient un bouton d'interaction
 * Utilise le système d'événements du GameManager pour détecter l'interaction
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Sunday, January 12th, 2026.
 * @global
 */
public class HoldInteractQuest : S_QuestStep
{
    [Header("Quest Settings")]
    [SerializeField] private string interactTag = "HoldInteract"; // Tag de l'objet
    [SerializeField] private string specificObjectName = ""; // Optionnel : nom spécifique de l'objet
    [SerializeField] private bool useAnyObjectMode = false; // Si true, utilise HoldInteractAnyObject au lieu de tags
    
    private bool hasInteractedWithObject = false;
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

        Debug.Log("[HoldInteractQuest] GameManager ready, subscribing to events");
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed) return;

        if (useAnyObjectMode)
        {
            S_GameManager.instance.playerEvents.onPlayerHoldInteractedWithAnyObject += OnPlayerHoldInteractedWithAnyObject;
            Debug.Log("<color=red>[HoldInteractQuest] Subscribed to onPlayerHoldInteractedWithAnyObject event (AnyObject Mode)</color>");
        }
        else
        {
            S_GameManager.instance.playerEvents.onPlayerHoldInteracted += OnPlayerHoldInteracted;
            Debug.Log("[HoldInteractQuest] Subscribed to onPlayerHoldInteracted event (Tag/Name Mode)");
        }
        
        isSubscribed = true;
    }

    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;

        if (useAnyObjectMode)
        {
            S_GameManager.instance.playerEvents.onPlayerHoldInteractedWithAnyObject -= OnPlayerHoldInteractedWithAnyObject;
            Debug.Log("<color=red>[HoldInteractQuest] Unsubscribed from onPlayerHoldInteractedWithAnyObject event</color>");
        }
        else
        {
            S_GameManager.instance.playerEvents.onPlayerHoldInteracted -= OnPlayerHoldInteracted;
            Debug.Log("[HoldInteractQuest] Unsubscribed from onPlayerHoldInteracted event");
        }
        
        isSubscribed = false;
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /**
     * Callback appelé quand le joueur maintient une interaction (Mode AnyObject)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 12th, 2026.
     * @access	private
     * @param	GameObject	obj	
     * @return	void
     */
    private void OnPlayerHoldInteractedWithAnyObject(GameObject obj)
    {
        if (hasInteractedWithObject) return;

        Debug.Log($"<color=red>[HoldInteractQuest] Player interacted with any object: {obj.name}</color>");

        if (IsTargetAnyObject(obj))
        {
            CompleteInteraction();
        }
    }

    /**
     * Callback appelé quand le joueur maintient une interaction (Mode Tag/Name)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 12th, 2026.
     * @access	private
     * @param	string	objectName	
     * @param	string	objectTag 	
     * @return	void
     */
    private void OnPlayerHoldInteracted(string objectName, string objectTag)
    {
        if (hasInteractedWithObject) return;

        Debug.Log($"[HoldInteractQuest] Player held interaction: {objectName} with tag {objectTag}");

        if (IsTargetObject(objectName, objectTag))
        {
            CompleteInteraction();
        }
    }

    /**
     * Vérifie si l'objet interagi est valide (Mode AnyObject)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 12th, 2026.
     * @access	private
     * @param	GameObject	obj	
     * @return	bool
     */
    private bool IsTargetAnyObject(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("<color=red>[HoldInteractQuest] Object is null!</color>");
            return false;
        }

        // Vérifier si l'objet a le component HoldInteractAnyObject
        HoldInteractAnyObject interactComponent = obj.GetComponent<HoldInteractAnyObject>();
        if (interactComponent == null)
        {
            Debug.Log($"<color=red>[HoldInteractQuest] {obj.name} doesn't have HoldInteractAnyObject component</color>");
            return false;
        }

        // Si un nom spécifique est défini, vérifier le nom
        if (!string.IsNullOrEmpty(specificObjectName))
        {
            if (obj.name != specificObjectName)
            {
                Debug.Log($"<color=red>[HoldInteractQuest] {obj.name} doesn't match required name '{specificObjectName}'</color>");
                return false;
            }
        }

        Debug.Log($"<color=red>[HoldInteractQuest] {obj.name} is the target object!</color>");
        return true;
    }

    /**
     * Vérifie si l'objet interagi est le bon objet (Mode Tag/Name)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 12th, 2026.
     * @access	private
     * @param	string	objectName	
     * @param	string	objectTag 	
     * @return	bool
     */
    private bool IsTargetObject(string objectName, string objectTag)
    {
        // Vérifier le tag
        if (!string.IsNullOrEmpty(interactTag))
        {
            if (objectTag != interactTag)
            {
                Debug.Log($"[HoldInteractQuest] {objectName} tag '{objectTag}' doesn't match required tag '{interactTag}'");
                return false;
            }
        }

        // Vérifier le nom si spécifié
        if (!string.IsNullOrEmpty(specificObjectName))
        {
            if (objectName != specificObjectName)
            {
                Debug.Log($"[HoldInteractQuest] {objectName} doesn't match required name '{specificObjectName}'");
                return false;
            }
        }

        Debug.Log($"[HoldInteractQuest] {objectName} is the target object!");
        return true;
    }

    /**
     * Appelé quand le joueur interagit avec le bon objet
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 12th, 2026.
     * @access	private
     * @return	void
     */
    private void CompleteInteraction()
    {
        if (hasInteractedWithObject) return;

        hasInteractedWithObject = true;
        
        string mode = useAnyObjectMode ? "AnyObject Mode" : "Tag/Name Mode";
        Debug.Log($"<color=red>[HoldInteractQuest] Quest step completed - player held interaction! ({mode})</color>");

        ChangeState("Player held interaction", "COMPLETE");
        FinishQuestStep();
    }

    /**
     * Permet de charger l'état de la quest step depuis une sauvegarde
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, January 12th, 2026.
     * @access	protected
     * @param	string	state	
     * @return	void
     */
    protected override void SetQuestStepState(string state)
    {
        Debug.Log($"<color=red>[HoldInteractQuest] Loading state: {state}</color>");

        if (state == "COMPLETE")
        {
            hasInteractedWithObject = true;
        }
    }
}
