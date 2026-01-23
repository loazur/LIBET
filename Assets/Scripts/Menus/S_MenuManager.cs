using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manager centralisé pour gérer l'ouverture/fermeture de tous les menus du jeu
/// Empêche l'ouverture de plusieurs menus simultanément
/// </summary>
public class S_MenuManager : MonoBehaviour
{
    public static S_MenuManager instance { get; private set; }

    [Header("État actuel")]
    [SerializeField] private MenuType currentOpenMenu = MenuType.NONE;
    [SerializeField] private bool isAnyMenuOpen = false;

    //~ Helping UI
    [Header("UI D'aide")]
    [SerializeField] private GameObject uiHelpContainer;
    [SerializeField] private TextMeshProUGUI cancelKeybindText;
    [SerializeField] private TextMeshProUGUI helpText;
    private bool isHelpingUIOpened = false;

    public enum MenuType
    {
        NONE,
        PAUSE,
        DAYS_TRANSITION,
        DIALOGUE,
        MINIGAME,
        NOTES,
        PADLOCK,
        QUESTS
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Vérifie si un menu peut être ouvert
    /// </summary>
    /// <param name="menuType">Type de menu à ouvrir</param>
    /// <returns>True si le menu peut s'ouvrir</returns>
    public bool CanOpenMenu(MenuType menuType)
    {
        /*
        // Le menu Pause peut toujours s'ouvrir (pour mettre en pause pendant un dialogue, etc.)
        if (menuType == MenuType.PAUSE)
            return true;
        */

        // Les autres menus ne peuvent s'ouvrir que si aucun menu n'est ouvert
        return !isAnyMenuOpen;
    }

    /// <summary>
    /// Enregistre l'ouverture d'un menu
    /// </summary>
    /// <param name="menuType">Type de menu ouvert</param>
    /// <returns>True si l'enregistrement a réussi</returns>
    public bool RegisterMenuOpen(MenuType menuType)
    {
        if (!CanOpenMenu(menuType))
        {
            Debug.LogWarning($"[MenuManager] Impossible d'ouvrir {menuType}, un menu est déjà ouvert: {currentOpenMenu}");
            return false;
        }

        currentOpenMenu = menuType;
        isAnyMenuOpen = true;

        Debug.Log($"<color=cyan>[MenuManager]</color> Menu ouvert: {menuType}");

        // Désactive les mouvements du joueur
        DisablePlayerControls();

        return true;
    }

    /// <summary>
    /// Enregistre la fermeture d'un menu
    /// </summary>
    /// <param name="menuType">Type de menu fermé</param>
    public void RegisterMenuClose(MenuType menuType)
    {
        if (currentOpenMenu == menuType)
        {
            currentOpenMenu = MenuType.NONE;
            isAnyMenuOpen = false;

            Debug.Log($"<color=cyan>[MenuManager]</color> Menu fermé: {menuType}");

            // Réactive les mouvements du joueur
            EnablePlayerControls();
        }
    }

    /// <summary>
    /// Vérifie si un menu est actuellement ouvert
    /// </summary>
    public bool IsAnyMenuOpen()
    {
        return isAnyMenuOpen;
    }

    /// <summary>
    /// Récupère le type de menu actuellement ouvert
    /// </summary>
    public MenuType GetCurrentOpenMenu()
    {
        return currentOpenMenu;
    }

    /// <summary>
    /// Désactive tous les contrôles du joueur
    /// </summary>
    private void DisablePlayerControls()
    {
        if (S_GameManager.instance == null) return;

        var playerController = FindAnyObjectByType<S_PlayerController>();
        var playerCamera = FindAnyObjectByType<S_FirstPersonCamera>();
        var playerInteract = FindAnyObjectByType<S_PlayerInteract>();
        var playerCrouch = playerController?.GetComponent<S_PlayerCrouch>();
        var playerFootsteps = playerController?.GetComponent<S_PlayerFootsteps>();

        if (playerController != null) playerController.setMovementsEnabled(false);
        if (playerCamera != null)
        {
            playerCamera.setCursorEnabled(true);
            playerCamera.setRotationEnabled(false);
        }
        if (playerInteract != null) playerInteract.setInteractionEnabled(false);
        if (playerCrouch != null) playerCrouch.setAbleToCrouch(false);
        if (playerFootsteps != null) playerFootsteps.SetSoundsEnabled(false);
    }

    /// <summary>
    /// Réactive tous les contrôles du joueur
    /// </summary>
    private void EnablePlayerControls()
    {
        if (S_GameManager.instance == null) return;

        var playerController = FindAnyObjectByType<S_PlayerController>();
        var playerCamera = FindAnyObjectByType<S_FirstPersonCamera>();
        var playerInteract = FindAnyObjectByType<S_PlayerInteract>();
        var playerCrouch = playerController?.GetComponent<S_PlayerCrouch>();
        var playerFootsteps = playerController?.GetComponent<S_PlayerFootsteps>();

        if (playerController != null) playerController.setMovementsEnabled(true);
        if (playerCamera != null)
        {
            playerCamera.setCursorEnabled(false);
            playerCamera.setRotationEnabled(true);
        }
        if (playerInteract != null) playerInteract.setInteractionEnabled(true);
        if (playerCrouch != null) playerCrouch.setAbleToCrouch(true);
        if (playerFootsteps != null) playerFootsteps.SetSoundsEnabled(true);

        // Re-lock le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //!--------- UI D'aide ---------

    public void EnableHelpingUI(string text) //& Active l'UI d'aide
    {
        if (isHelpingUIOpened) return;

        UpdateKeybindText();

        helpText.text = text;
        uiHelpContainer.SetActive(true);

        isHelpingUIOpened = true;
    }

    public void DisableHelpingUI() //& Désactive l'UI d'aide
    {
        if (!isHelpingUIOpened) return;

        helpText.text = "...";
        uiHelpContainer.SetActive(false);

        isHelpingUIOpened = false;
    }

    private void UpdateKeybindText() //& Met à jour l'UI de la touche Cancel
    {
        cancelKeybindText.text = S_UserInput.instance.CancelInteractionAction.GetBindingDisplayString();
    }
}