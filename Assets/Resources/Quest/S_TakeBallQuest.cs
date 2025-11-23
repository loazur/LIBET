using UnityEngine;

/**
 * Quête pour ramasser une balle. Détecte quand le joueur ramasse un objet avec le tag "Ball"
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Sunday, November 23rd, 2025.
 * @global
 */
public class S_TakeBallQuest : S_QuestStep
{
    [Header("Quest Settings")]
    [SerializeField] private string ballTag = "Ball"; // Tag de la balle à ramasser
    [SerializeField] private string specificBallName = ""; // Optionnel : nom spécifique de la balle
    
    private S_PlayerInteract playerInteract;
    private bool ballTaken = false;
    private S_ItemInteraction lastCheckedItem = null;

    // *=======================================================================

    private void Start()
    {
        // Trouver le composant PlayerInteract
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInteract = player.GetComponent<S_PlayerInteract>();
        }
        else
        {
            Debug.LogWarning("S_TakeBallQuest: Vous devez ajouter le tag 'Player' à votre joueur pour que la quête fonctionne correctement.");
        }
    }

    private void Update()
    {
        if (ballTaken || playerInteract == null) return;

        
        if (playerInteract.isHoldingItem()) //& Vérifier si le joueur tient un item
        {
            //& Utiliser la réflexion pour accéder au champ privé holdingItem
            var holdingItemField = typeof(S_PlayerInteract).GetField("holdingItem", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (holdingItemField != null)
            {
                S_ItemInteraction heldItem = holdingItemField.GetValue(playerInteract) as S_ItemInteraction;
                
                //& Vérifier si c'est un nouvel item (pour ne déclencher qu'une fois)
                if (heldItem != null && heldItem != lastCheckedItem)
                {
                    lastCheckedItem = heldItem;
                    
                    if (IsBall(heldItem.gameObject))
                    {
                        TakeBall();
                    }
                }
            }
        }
        else
        {
            //& Réinitialiser si le joueur ne tient plus d'item
            lastCheckedItem = null;
        }
    }

    /**
     * Vérifie si l'objet est la balle recherchée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 15th, 2025.
     * @access	private
     * @param	gameobject	obj	
     * @return	boolean
     */
    private bool IsBall(GameObject obj)
    {
        //* Vérifier par tag
        if (!obj.CompareTag(ballTag))
        {
            return false;
        }

        //& Si un nom spécifique est défini, vérifier aussi le nom
        if (!string.IsNullOrEmpty(specificBallName))
        {
            return obj.name.Contains(specificBallName);
        }
        Debug.Log("Balle reconnue : " + obj.name); //! ASUP
        return true;
    }

    
    /**
     * Méthode appelée quand la balle est ramassée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 15th, 2025.
     * @access	private
     * @return	void
     */
    private void TakeBall()
    {
        if (ballTaken) return;

        ballTaken = true;
        Debug.Log("Balle ramassée ! Quête terminée.");

        //& Marquer l'étape de la quête comme terminée
        FinishQuestStep();
    }

    /**
     * Implémentation requise de la classe abstraite S_QuestStep
     *
     * @var		override	voi
     */
    protected override void SetQuestStepState(string state)
    {
        if (string.IsNullOrEmpty(state)) return;

        string s = state.ToLowerInvariant().Trim();

        // Considérer plusieurs valeurs pour marquer l'étape comme complétée
        if (s == "completed" || s == "finished" || s == "true")
        {
            if (!ballTaken)
            {
                ballTaken = true;
                FinishQuestStep();
                Debug.Log("SetQuestStepState: État défini sur complété.");
            }
        }
        // Réinitialiser l'état de la quête si demandé
        else if (s == "reset" || s == "false" || s == "incomplete")
        {
            ballTaken = false;
            lastCheckedItem = null;
            Debug.Log("SetQuestStepState: État réinitialisé.");
        }
        else
        {
            Debug.Log($"SetQuestStepState: État non reconnu '{state}'.");
        }
    }
}
