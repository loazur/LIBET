using UnityEngine;
using System.Collections;

/**
 * Quête pour ramasser une balle. Détecte quand le joueur ramasse un objet avec le tag "Ball"
 * Utilise le système d'événements du GameManager pour détecter le ramassage
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v2.0.0	Friday, November 29th, 2025.
 * @global
 */
public class S_TakeBallQuest : S_QuestStep
{
    [Header("Quest Settings")]
    [SerializeField] private string ballTag = "Ball"; // Tag de la balle à ramasser
    [SerializeField] private string specificBallName = ""; // Optionnel : nom spécifique de la balle
    
    private bool ballTaken = false;
    private bool isSubscribed = false;

    // *=======================================================================

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

        // S'abonner à l'événement de ramassage d'item
        SubscribeToEvents();
        Debug.Log("[S_TakeBallQuest] Quête de ramassage de balle initialisée.");
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed) return;

        S_GameManager.instance.playerEvents.onItemPickedUp += OnItemPickedUp;
        isSubscribed = true;
    }

    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;

        S_GameManager.instance.playerEvents.onItemPickedUp -= OnItemPickedUp;
        isSubscribed = false;
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /**
     * Callback appelé quand le joueur ramasse un item
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	private
     * @param	gameobject	item	
     * @return	void
     */
    private void OnItemPickedUp(GameObject item)
    {
        if (ballTaken) return;

        Debug.Log($"[S_TakeBallQuest] Item ramassé: {item.name}, Tag: {item.tag}");

        // Vérifier si c'est la balle recherchée
        if (IsBall(item))
        {
            TakeBall(item);
        }
    }

    /**
     * Vérifie si l'objet est la balle recherchée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	private
     * @param	gameobject	obj	
     * @return	boolean
     */
    private bool IsBall(GameObject obj)
    {
        // Vérifier par tag
        if (!obj.CompareTag(ballTag))
        {
            Debug.Log($"[S_TakeBallQuest] {obj.name} n'a pas le tag '{ballTag}'");
            return false;
        }

        // Si un nom spécifique est défini, vérifier aussi le nom
        if (!string.IsNullOrEmpty(specificBallName))
        {
            if (!obj.name.Contains(specificBallName))
            {
                Debug.Log($"[S_TakeBallQuest] {obj.name} ne contient pas '{specificBallName}'");
                return false;
            }
        }

        Debug.Log($"[S_TakeBallQuest] Balle valide reconnue : {obj.name}");
        return true;
    }

    /**
     * Méthode appelée quand la balle est ramassée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, November 29th, 2025.
     * @access	private
     * @param	gameobject	ball	
     * @return	void
     */
    private void TakeBall(GameObject ball)
    {
        if (ballTaken) return;

        // Vérifier que la quête est bien initialisée (active)
        if (!IsQuestStepInitialized())
        {
            Debug.LogWarning($"[S_TakeBallQuest] Balle '{ball.name}' ramassée mais la quête n'est pas encore active. En attente...");
            return;
        }

        ballTaken = true;
        Debug.Log($"[S_TakeBallQuest] Balle '{ball.name}' ramassée ! Quête terminée.");

        // Désabonnement de l'événement
        UnsubscribeFromEvents();

        // Marquer l'étape de la quête comme terminée
        ChangeState("collected", ball.name);
        FinishQuestStep();
    }

    /**
     * Implémentation requise de la classe abstraite S_QuestStep
     * Permet de restaurer l'état de la quête lors du chargement d'une sauvegarde
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
        if (string.IsNullOrEmpty(state)) return;

        string s = state.ToLowerInvariant().Trim();

        // Considérer plusieurs valeurs pour marquer l'étape comme complétée
        if (s == "completed" || s == "finished" || s == "collected" || s == "true")
        {
            if (!ballTaken)
            {
                ballTaken = true;
                UnsubscribeFromEvents();
                Debug.Log("[S_TakeBallQuest] État défini sur complété.");
            }
        }
        // Réinitialiser l'état de la quête si demandé
        else if (s == "reset" || s == "false" || s == "incomplete")
        {
            ballTaken = false;
            SubscribeToEvents();
            Debug.Log("[S_TakeBallQuest] État réinitialisé.");
        }
        else
        {
            Debug.Log($"[S_TakeBallQuest] État non reconnu '{state}'.");
        }
    }
}
