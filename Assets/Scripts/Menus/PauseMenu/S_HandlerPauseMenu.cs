using UnityEngine;

public class S_HandlerPauseMenu : MonoBehaviour
{
    //! S_HandlerPauseMenu gère la gestion des menus avec les touches du clavier (dont l'ouverture intial)

    [Header("Menu Navigation")]
    [SerializeField] private S_PauseMenu pauseMenu;

    private bool isOpen = false;
    private bool ableToOpenCloseMenu = true;


    void Start() //& Cache tout les menus de base
    {
        HideAll();
    }


    void Update()
    {
        if (S_UserInput.instance.MenuOpenCloseInput && ableToOpenCloseMenu) // Quand on appuis sur la touche d'ouverture du menu
        {
            if (!isOpen)
            {
                pauseMenu.ActivateMenu();
            }
        }
        
    }

    //! --------------- Fonctions principales ---------------

    public void HideAll() //& Fermeture de tout les menus
    {
        
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
}
