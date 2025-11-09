using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class S_DisplayMenus : MonoBehaviour
{
    //~ Références vers d'autre classes
    [SerializeField] private S_PlayerController playerController;
    [SerializeField] private S_FirstPersonCamera playerCamera;
    [SerializeField] private S_PlayerInteract playerInteract;

    //~ Containers des menus / sous-menus
    [Header("Containers des différents menu")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject keybindsSettingsMenu;
    private List<GameObject> listMenus;
    private GameObject currentOpenedMenu;

    private Volume volume; // Floutage du background
    private bool isOpen = false;
    private bool ableToOpenCloseMenu = true;

    void Awake() //& Avant de tout cacher on initialise les variables
    {
        volume = playerCamera.GetComponent<Volume>(); 
        currentOpenedMenu = mainMenu; // Le 1er menu qu'on va ouvrir c'est le mainMenu

        listMenus = new List<GameObject>
        {
            mainMenu,
            keybindsSettingsMenu
        };
        
    }

    void Start() //& Cache tout les menus
    {
        HideAll();
    }

    void Update()
    {
        if (S_UserInput.instance.MenuOpenCloseInput && ableToOpenCloseMenu)
        {
            if (!isOpen) // Ouvrir MainMenu
            {
                ShowMenu(mainMenu);
            }
            else // Fermer Menu actuel
            {
                HideCurrent(true);
            }
        }
        
    }

    //! --------------- Fonctions principales ---------------

    public void ShowMenu(GameObject menu) //& Ouvre le menu pris en paramètre
    {
        HideCurrent(false); // Cache l'ancien
        menu.SetActive(true); // Active le nouveau
        currentOpenedMenu = menu; // Et l'assigne à la variable

        //
        playerController.setMovementsEnabled(false);
        playerCamera.setCursorEnabled(true);
        playerCamera.setRotationEnabled(false);
        playerInteract.setInteractionEnabled(false);
        volume.enabled = true;
        isOpen = true;
    }

    public void HideCurrent(bool closingMenu) //& Fermeture du menu actuel
    {
        currentOpenedMenu.SetActive(false);

        if (closingMenu) // SI on ferme le menu 
        {
            playerController.setMovementsEnabled(true);
            playerCamera.setCursorEnabled(false);
            playerCamera.setRotationEnabled(true);
            playerInteract.setInteractionEnabled(true);
            volume.enabled = false;
            isOpen = false;
        }
    }

    public void HideAll() //& Fermeture tout les menus
    {
        foreach (GameObject menu in listMenus) // Désactive tout les menus
        {
            menu.SetActive(false);
        }

        playerController.setMovementsEnabled(true);
        playerCamera.setCursorEnabled(false);
        playerCamera.setRotationEnabled(true);
        playerInteract.setInteractionEnabled(true);
        volume.enabled = false;
        isOpen = false;
    }

    //? ------------------------------------------------    

    public bool canOpenCloseMenu() //& Si à le droit d'ouvrir/fermer le menu
    {
        return ableToOpenCloseMenu;
    }
    
    public void setAbleToOpenCloseMenu(bool canOpenClose) //& Active/Désactive le menu
    {
        ableToOpenCloseMenu = canOpenClose;
    }
}
