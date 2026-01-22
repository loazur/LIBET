using UnityEngine;

/**
 * Tiroir verrouillé qui se déverrouille uniquement via un événement.
 * Hérite de S_DoorInteractable pour réutiliser la logique d'ouverture/fermeture.
 * Pas de clé nécessaire - déverrouillage via événement externe uniquement.
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Tuesday, January 21st, 2026.
 * @global
 */
public class S_DrawerLocked : S_DoorInteractable
{
    [Header("Configuration du verrouillage")]
    [Tooltip("L'ID unique de ce tiroir (utilisé pour le déverrouillage via événement)")]
    [SerializeField] private string drawerID = "drawer_01";

    [Header("Sons verrouillage (Optionnel)")]
    [Tooltip("Son joué quand le tiroir est verrouillé")]
    [SerializeField] private bool playLockedSound = true;

    //~ État du verrouillage
    private bool isUnlocked = false;  // Une fois true, ne revient jamais à false

    protected override void Start()
    {
        base.Start(); // Appel du Start parent

        //& S'abonner à l'événement de déverrouillage du tiroir
        if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
        {
            S_GameManager.instance.playerEvents.onDrawerUnlocked += OnDrawerUnlockEvent;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy(); // Appel du OnDestroy parent

        if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
        {
            S_GameManager.instance.playerEvents.onDrawerUnlocked -= OnDrawerUnlockEvent;
        }
    }

    //! Override de l'interaction pour ajouter la logique de verrouillage
    //! =====================================================

    public override void Interact(Transform playerTransform)
    {
        //& Vérifier si le tiroir est déjà débloqué
        if (!isUnlocked)
        {
            //& Le tiroir est verrouillé
            OnLockedInteraction();
            return;
        }

        //& Le tiroir est débloqué, utiliser le comportement parent
        base.Interact(playerTransform);
    }

    //! --------------- Méthodes de verrouillage ---------------

    /**
     * Callback quand l'événement de déverrouillage est déclenché.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 21st, 2026.
     * @access	private
     * @param	string	unlockedDrawerID	L'ID du tiroir à déverrouiller
     * @return	void
     */
    private void OnDrawerUnlockEvent(string unlockedDrawerID)
    {
        if (unlockedDrawerID == drawerID)
        {
            UnlockDrawer();
        }
    }

    /**
     * Débloque le tiroir de manière permanente.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 21st, 2026.
     * @access	public
     * @return	void
     */
    public void UnlockDrawer()
    {
        if (isUnlocked) return; // Déjà déverrouillé
        
        isUnlocked = true;
        // Debug.Log($"[S_DrawerLocked] Tiroir '{drawerID}' débloqué!");
        
        // Jouer un son de déverrouillage 
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorUnlock, transform.position);

        UpdateInteractText();
    }

    /**
     * Appelé quand le joueur essaie d'interagir avec un tiroir verrouillé.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 21st, 2026.
     * @access	private
     * @return	void
     */
    private void OnLockedInteraction()
    {
        // Debug.Log($"[S_DrawerLocked] Tiroir '{drawerID}' verrouillé!");

        // Jouer un son de tiroir verrouillé
        if (playLockedSound)
        {
            S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorLocked, transform.position);
        }
    }

    //! --------------- Override du texte d'interaction ---------------

    protected override void UpdateInteractText()
    {
        bool isFrench = S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French;

        if (!isUnlocked)
        {
            //& Tiroir verrouillé
            if (isFrench)
            {
                interactText = "Verrouillé";
            }
            else
            {
                interactText = "Locked";
            }
        }
        else
        {
            //& Tiroir débloqué - utiliser le texte parent (Ouvrir/Fermer)
            base.UpdateInteractText();
        }
    }

    //! --------------- Getters / Debug ---------------

    public string GetDrawerID() => drawerID;
    public bool IsUnlocked() => isUnlocked;
}
