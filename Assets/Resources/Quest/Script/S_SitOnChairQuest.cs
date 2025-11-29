using UnityEngine;
using System.Collections;

/**
 * Quête pour s'asseoir sur une chaise. Détecte quand le joueur s'assoit sur un objet avec S_ChairInteractable
 * Utilise le système d'événements du GameManager pour détecter l'action de s'asseoir
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Friday, November 29th, 2025.
 * @global
 */
public class S_SitOnChairQuest : S_QuestStep
{
    [Header("Quest Settings")]
    [SerializeField] private string chairTag = "Chair"; // Tag de la chaise (optionnel)
    [SerializeField] private string specificChairName = ""; // Optionnel : nom spécifique de la chaise
    
    private bool hasSat = false;
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

        Debug.Log("[S_SitOnChairQuest] GameManager ready, subscribing to events");
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed) return;
        
        S_GameManager.instance.playerEvents.onPlayerSat += OnPlayerSat;
        isSubscribed = true;
        Debug.Log("[S_SitOnChairQuest] Subscribed to onPlayerSat event");
    }

    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;
        
        S_GameManager.instance.playerEvents.onPlayerSat -= OnPlayerSat;
        isSubscribed = false;
        Debug.Log("[S_SitOnChairQuest] Unsubscribed from onPlayerSat event");
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /**
     * Callback appelé quand le joueur s'assoit sur une chaise
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	private
     * @param	gameobject	chair	
     * @return	void
     */
    private void OnPlayerSat(GameObject chair)
    {
        if (hasSat) return;

        Debug.Log($"[S_SitOnChairQuest] Player sat on: {chair.name}");

        if (IsTargetChair(chair))
        {
            SitOnChair();
        }
    }

    /**
     * Vérifie si l'objet est la chaise recherchée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	private
     * @param	gameobject	obj	
     * @return	boolean
     */
    private bool IsTargetChair(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[S_SitOnChairQuest] Chair object is null");
            return false;
        }

        // Vérifier si l'objet a le component S_ChairInteractable
        S_ChairInteractable chairComponent = obj.GetComponent<S_ChairInteractable>();
        if (chairComponent == null)
        {
            Debug.Log($"[S_SitOnChairQuest] {obj.name} doesn't have S_ChairInteractable component");
            return false;
        }

        // Si un tag spécifique est défini, vérifier le tag
        if (!string.IsNullOrEmpty(chairTag))
        {
            if (!obj.CompareTag(chairTag))
            {
                Debug.Log($"[S_SitOnChairQuest] {obj.name} tag '{obj.tag}' doesn't match required tag '{chairTag}'");
                return false;
            }
        }

        // Si un nom spécifique est défini, vérifier le nom
        if (!string.IsNullOrEmpty(specificChairName))
        {
            if (obj.name != specificChairName)
            {
                Debug.Log($"[S_SitOnChairQuest] {obj.name} doesn't match required name '{specificChairName}'");
                return false;
            }
        }

        Debug.Log($"[S_SitOnChairQuest] {obj.name} is the target chair!");
        return true;
    }

    /**
     * Appelé quand le joueur s'assoit sur la bonne chaise
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	private
     * @return	void
     */
    private void SitOnChair()
    {
        if (hasSat) return;

        hasSat = true;
        Debug.Log("[S_SitOnChairQuest] Quest completed - player sat on the chair!");

        ChangeState("Player sat on chair", "COMPLETE");
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
        Debug.Log($"[S_SitOnChairQuest] Loading state: {state}");

        if (state == "COMPLETE")
        {
            hasSat = true;
        }
    }
}
