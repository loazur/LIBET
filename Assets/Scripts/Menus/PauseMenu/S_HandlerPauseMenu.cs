using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_HandlerPauseMenu : MonoBehaviour
{
    //! S_HandlerPauseMenu gère la gestion des menus avec les touches du clavier (dont l'ouverture intial)

    public static S_HandlerPauseMenu instance;

    [Header("Menu Navigation")]
    [SerializeField] private S_PauseMenu pauseMenu;

    [Header("Fond du menu pause")]
    [SerializeField] private Image pauseMenuBackground;

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
                if (S_MenuManager.instance != null)
                {
                    if (!S_MenuManager.instance.RegisterMenuOpen(S_MenuManager.MenuType.PAUSE))
                    {
                        Debug.LogWarning("[DialogueManager] Impossible de démarrer le menu pause, un menu est ouvert");
                        return;
                    }
                }

                pauseMenu.ActivateMenu();
                Time.timeScale = 0; // Désactive l'écoulement du temps
            }
            else // Fermeture n'importe quel menu
            {
                CompletelyCloseMenu();
            }
        }
        
    }

    //! --------------- Fonctions principales ---------------

    public void CompletelyCloseMenu()
    {
        if (S_MenuManager.instance != null)
        {
            S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.PAUSE);
        }

        pauseMenuBackground.gameObject.SetActive(false);
        currentMenu.DeactivateMenu();
        currentMenu = null;

        Time.timeScale = 1; // Réactive l'écoulement du temps
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
