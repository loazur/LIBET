using UnityEngine;
using UnityEngine.SceneManagement;

public class S_HandlerPauseMenu : MonoBehaviour
{
    //! S_HandlerPauseMenu gère la gestion des menus avec les touches du clavier (dont l'ouverture intial)

    public static S_HandlerPauseMenu instance;

    [Header("Menu Navigation")]
    [SerializeField] private S_PauseMenu pauseMenu;

    //~ Références vers d'autre classes
    [SerializeField] private S_PlayerController playerController;
    [SerializeField] private S_PlayerCrouch playerCrouch;
    [SerializeField] private S_FirstPersonCamera playerCamera;
    [SerializeField] private S_PlayerInteract playerInteract;

    private string currentSceneName;
        
    private bool menuOpened = false;
    private bool ableToOpenCloseMenu = true;
    private S_Menu currentMenu = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            currentSceneName = SceneManager.GetActiveScene().name;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (S_UserInput.instance.MenuOpenCloseInput && ableToOpenCloseMenu && currentSceneName != "MainMenu") // Quand on appuis sur la touche d'ouverture du menu
        {
            if (!menuOpened) // Ouverture MenuPause
            {
                pauseMenu.ActivateMenu();
            }
            else // Fermeture n'importe quel menu
            {
                CompletelyCloseMenu();
            }
        }
        
    }

    //! --------------- Fonctions principales ---------------

    public void EnableAll()
    {
        playerController.setMovementsEnabled(true);
        playerCamera.setCursorEnabled(false);
        playerCamera.setRotationEnabled(true);
        playerInteract.setInteractionEnabled(true);
        playerCrouch.setAbleToCrouch(true);

        Time.timeScale = 1; // Réactive l'écoulement du temps
    }

    public void DisableAll()
    {
        playerController.setMovementsEnabled(false);
        playerCamera.setCursorEnabled(true);
        playerCamera.setRotationEnabled(false);
        playerInteract.setInteractionEnabled(false);
        playerCrouch.setAbleToCrouch(false);

        Time.timeScale = 0; // Désactive l'écoulement du temps
    }

    public void CompletelyCloseMenu()
    {
        currentMenu.DeactivateMenu();
        currentMenu = null;

        EnableAll();
        setMenuOpened(false);
    }

    //? ------------------------------------------------    

    public bool canOpenClosePauseMenu() //& Vérification de si on peut ouvrir/fermer le menu
    {
        return ableToOpenCloseMenu;
    }
    
    public void setAbleToOpenClosePauseMenu(bool canOpenClose) //& Active/Désactive le menu
    {
        ableToOpenCloseMenu = canOpenClose;
    }

    public void setMenuOpened(bool isMenuOpened) //& Permet de dire si un menu est ouvert actuellement
    {
        menuOpened = isMenuOpened;
    }

    public void setCurrentMenu(S_Menu menu) //& Permet de dire quel menu est utilisé actuellement (pour le fermer ensuite avec le clavier)
    {
        currentMenu = menu;
    }

    public bool IsMenuOpened() //& Récupère l'état du menu
    {
        return menuOpened;
    }

    public S_Menu GetCurrentMenu() //& Récupère le menu actuel
    {
        return currentMenu;
    }

}
