using UnityEngine;

/**
 * Placard verrouillé qui se déverrouille uniquement via un événement.
 * Hérite de S_CupboardInteractable pour réutiliser la logique d'ouverture/fermeture.
 * Pas de clé nécessaire - déverrouillage via événement externe uniquement.
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Tuesday, January 21st, 2026.
 * @global
 */
public class S_CupboardLocked : S_CupboardInteractable, SI_DataPersistance
{
    [SerializeField] private string id;
    
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [Header("Configuration du verrouillage")]
    [Tooltip("L'ID unique de ce placard (utilisé pour le déverrouillage via événement)")]
    [SerializeField] private string cupboardID = "cupboard_01";

    [Header("Sons verrouillage (Optionnel)")]
    [Tooltip("Son joué quand le placard est verrouillé")]
    [SerializeField] private bool playLockedSound = true;

    //~ État du verrouillage
    private bool isUnlocked = false;  // Une fois true, ne revient jamais à false

    protected override void Start()
    {
        base.Start(); // Appel du Start parent

        //& S'abonner à l'événement de déverrouillage du placard
        if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
        {
            S_GameManager.instance.playerEvents.onCupboardUnlocked += OnCupboardUnlockEvent;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy(); // Appel du OnDestroy parent

        if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
        {
            S_GameManager.instance.playerEvents.onCupboardUnlocked -= OnCupboardUnlockEvent;
        }
    }

    //!---------------- SI_DataPersistance ----------------

    //~ Sauvegarde de si le cupboard est unlocked

    public void LoadData(S_GameData gameData)
    {
        if (gameData.unlockedCupboards.TryGetValue(id, out bool isUnlocked))
        {
            this.isUnlocked = isUnlocked;
        }
    }

    public void SaveData(S_GameData gameData)
    {
        if (gameData.unlockedCupboards.ContainsKey(id))
        {
            gameData.unlockedCupboards.Remove(id);
        }

        gameData.unlockedCupboards.Add(id, isUnlocked);
    }

    public int GetLoadPriority() => 0; // ✅ Priorité normale

    //! Override de l'interaction pour ajouter la logique de verrouillage
    //! =====================================================

    public override void Interact(Transform playerTransform)
    {
        //& Vérifier si le placard est déjà débloqué
        if (!isUnlocked)
        {
            //& Le placard est verrouillé
            OnLockedInteraction();
            return;
        }

        //& Le placard est débloqué, utiliser le comportement parent
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
     * @param	string	unlockedCupboardID	L'ID du placard à déverrouiller
     * @return	void
     */
    private void OnCupboardUnlockEvent(string unlockedCupboardID)
    {
        if (unlockedCupboardID == cupboardID)
        {
            UnlockCupboard();
        }
    }

    /**
     * Débloque le placard de manière permanente.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 21st, 2026.
     * @access	public
     * @return	void
     */
    public void UnlockCupboard()
    {
        if (isUnlocked) return; // Déjà déverrouillé
        
        isUnlocked = true;
        // Debug.Log($"[S_CupboardLocked] Placard '{cupboardID}' débloqué!");
        
        // Jouer un son de déverrouillage 
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorUnlock, transform.position);

        UpdateInteractText();
    }

    /**
     * Appelé quand le joueur essaie d'interagir avec un placard verrouillé.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 21st, 2026.
     * @access	private
     * @return	void
     */
    private void OnLockedInteraction()
    {
        // Debug.Log($"[S_CupboardLocked] Placard '{cupboardID}' verrouillé!");

        // Jouer un son de placard verrouillé
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
            //& Placard verrouillé
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
            //& Placard débloqué - utiliser le texte parent (Ouvrir/Fermer)
            base.UpdateInteractText();
        }
    }

    //! --------------- Getters / Debug ---------------

    public string GetCupboardID() => cupboardID;
    public bool IsUnlocked() => isUnlocked;
}