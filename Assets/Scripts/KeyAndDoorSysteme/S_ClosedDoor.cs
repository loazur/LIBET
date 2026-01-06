using UnityEngine;

/**
 * Porte verrouillée nécessitant plusieurs clés pour s'ouvrir.
 * Hérite de S_DoorInteractable pour réutiliser la logique d'ouverture/fermeture.
 * Une fois débloquée, la porte ne peut plus être reverrouillée.
 * Fonctionne comme dans Hello Neighbor / Granny.
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, January 5th, 2026.
 * @global
 */
public class S_ClosedDoor : S_DoorInteractable
{
    [Header("Configuration du verrouillage")]
    [Tooltip("L'ID unique de cette porte (doit correspondre au doorID des clés)")]
    [SerializeField] private string doorID = "door_01";
    
    [Tooltip("Nombre de clés requises pour débloquer cette porte")]
    [SerializeField] private int requiredKeyCount = 1;

    [Header("Sons verrouillage (Optionnel)")]
    [Tooltip("Son joué quand la porte est verrouillée")]
    [SerializeField] private bool playLockedSound = true;

    //~ État du verrouillage
    private bool isUnlocked = false;  // Une fois true, ne revient jamais à false

    protected override void Start()
    {
        base.Start(); // Appel du Start parent

        //& S'abonner à l'événement de collecte de clé pour update l'UI
        if (S_KeyManager.instance != null)
        {
            S_KeyManager.instance.OnKeyCollected += OnKeyCollected;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy(); // Appel du OnDestroy parent

        if (S_KeyManager.instance != null)
        {
            S_KeyManager.instance.OnKeyCollected -= OnKeyCollected;
        }
    }

    //! Override de l'interaction pour ajouter la logique de verrouillage
    //! =====================================================

    public override void Interact(Transform playerTransform)
    {
        //& Vérifier si la porte est déjà débloquée
        if (!isUnlocked)
        {
            //& Vérifier si le joueur a toutes les clés
            if (S_KeyManager.instance != null && S_KeyManager.instance.HasAllKeys(doorID, requiredKeyCount))
            {
                UnlockDoor();
            }
            else
            {
                //& Le joueur n'a pas toutes les clés
                OnLockedInteraction();
                return;
            }
        }

        //& La porte est débloquée, utiliser le comportement parent
        base.Interact(playerTransform);
    }

    //! --------------- Méthodes de verrouillage ---------------

    /**
     * Débloque la porte de manière permanente.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 5th, 2026.
     * @access	private
     * @return	void
     */
    private void UnlockDoor()
    {
        isUnlocked = true;
        // Debug.Log($"[ClosedDoor] Porte '{doorID}' débloquée!");
        
        // Jouer un son de déverrouillage 
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorUnlock, transform.position);

        UpdateInteractText();
    }

    /**
     * Appelé quand le joueur essaie d'interagir avec une porte verrouillée sans avoir toutes les clés.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 5th, 2026.
     * @access	private
     * @return	void
     */
    private void OnLockedInteraction()
    {
        int currentKeys = S_KeyManager.instance != null ? S_KeyManager.instance.GetCollectedKeyCount(doorID) : 0;
        // Debug.Log($"[ClosedDoor] Porte '{doorID}' verrouillée! Clés: {currentKeys}/{requiredKeyCount}");

        // Jouer un son de porte verrouillée
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorLocked, transform.position);
    }

    /**
     * Callback quand une clé est collectée - met à jour le texte d'interaction.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 5th, 2026.
     * @access	private
     * @param	string	collectedDoorID	
     * @param	string	keyID          	
     * @return	void
     */
    private void OnKeyCollected(string collectedDoorID, string keyID)
    {
        if (collectedDoorID == doorID)
        {
            UpdateInteractText();
        }
    }

    //! --------------- Override du texte d'interaction ---------------

    protected override void UpdateInteractText()
    {
        int currentKeys = S_KeyManager.instance != null ? S_KeyManager.instance.GetCollectedKeyCount(doorID) : 0;
        bool isFrench = S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French;

        if (!isUnlocked)
        {
            //& Porte verrouillée - afficher combien de clés il manque
            if (isFrench)
            {
                interactText = $"Verrouillée ({currentKeys}/{requiredKeyCount} clés)";
            }
            else
            {
                interactText = $"Locked ({currentKeys}/{requiredKeyCount} keys)";
            }
        }
        else
        {
            //& Porte débloquée - utiliser le texte parent (Ouvrir/Fermer)
            base.UpdateInteractText();
        }
    }

    //! --------------- Getters / Debug ---------------

    public string GetDoorID() => doorID;
    public int GetRequiredKeyCount() => requiredKeyCount;
    public bool IsUnlocked() => isUnlocked;
}
