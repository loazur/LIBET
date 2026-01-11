using UnityEngine;
using System.Collections;

/**
 * Quête pour ouvrir un placard. Détecte quand le joueur ouvre un objet avec S_CupboardInteractable
 * Utilise le système d'événements du GameManager pour détecter l'ouverture
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Thursday, January 9th, 2026.
 * @global
 */
public class S_OpenCupboardQuest : S_QuestStep
{
    [Header("Quest Settings")]
    [SerializeField] private string cupboardTag = "Cupboard"; // Tag du placard (optionnel)
    [SerializeField] private string specificCupboardName = ""; // Optionnel : nom spécifique du placard
    
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

        Debug.Log("[S_OpenCupboardQuest] GameManager ready, subscribing to events");
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed) return;
        
        S_GameManager.instance.playerEvents.onCupboardOpened += OnCupboardOpened;
        isSubscribed = true;
        Debug.Log("[S_OpenCupboardQuest] Subscribed to onCupboardOpened event");
    }

    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;
        
        S_GameManager.instance.playerEvents.onCupboardOpened -= OnCupboardOpened;
        isSubscribed = false;
        Debug.Log("[S_OpenCupboardQuest] Unsubscribed from onCupboardOpened event");
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /**
     * Callback appelé quand le joueur ouvre un placard
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Thursday, January 9th, 2026.
     * @access	private
     * @param	gameobject	cupboard	
     * @return	void
     */
    private void OnCupboardOpened(GameObject cupboard)
    {
        if (hasOpened) return;

        Debug.Log($"[S_OpenCupboardQuest] Player opened cupboard: {cupboard.name}");

        if (IsTargetCupboard(cupboard))
        {
            OpenCupboard();
        }
    }

    /**
     * Vérifie si l'objet est le placard recherché
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Thursday, January 9th, 2026.
     * @access	private
     * @param	gameobject	obj	
     * @return	boolean
     */
    private bool IsTargetCupboard(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[S_OpenCupboardQuest] Cupboard object is null");
            return false;
        }

        //& Vérifier si l'objet a le component S_CupboardInteractable
        S_CupboardInteractable cupboardComponent = obj.GetComponent<S_CupboardInteractable>();
        if (cupboardComponent == null)
        {
            Debug.Log($"[S_OpenCupboardQuest] {obj.name} doesn't have S_CupboardInteractable component");
            return false;
        }

        //& Si un tag spécifique est défini, vérifier le tag
        if (!string.IsNullOrEmpty(cupboardTag))
        {
            if (!obj.CompareTag(cupboardTag))
            {
                Debug.Log($"[S_OpenCupboardQuest] {obj.name} tag '{obj.tag}' doesn't match required tag '{cupboardTag}'");
                return false;
            }
        }

        //& Si un nom spécifique est défini, vérifier le nom
        if (!string.IsNullOrEmpty(specificCupboardName))
        {
            if (obj.name != specificCupboardName)
            {
                Debug.Log($"[S_OpenCupboardQuest] {obj.name} doesn't match required name '{specificCupboardName}'");
                return false;
            }
        }

        Debug.Log($"[S_OpenCupboardQuest] {obj.name} is the target cupboard!");
        return true;
    }

    /**
     * Appelé quand le joueur ouvre le bon placard
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Thursday, January 9th, 2026.
     * @access	private
     * @return	void
     */
    private void OpenCupboard()
    {
        if (hasOpened) return;

        hasOpened = true;
        Debug.Log("[S_OpenCupboardQuest] Quest completed - player opened the cupboard!");

        ChangeState("Player opened cupboard", "COMPLETE");
        FinishQuestStep();
    }

    /**
     * Permet de charger l'état de la quest step depuis une sauvegarde
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Thursday, January 9th, 2026.
     * @access	protected
     * @param	string	state	
     * @return	void
     */
    protected override void SetQuestStepState(string state)
    {
        Debug.Log($"[S_OpenCupboardQuest] Loading state: {state}");

        if (state == "COMPLETE")
        {
            hasOpened = true;
        }
    }
}
