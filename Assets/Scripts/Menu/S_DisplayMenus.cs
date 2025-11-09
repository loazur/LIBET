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

    private Volume volume;
    private bool isOpen = false;
    private bool ableToOpenCloseMenu = true;
    private GameObject currentOpenedMenu;

    void Awake()
    {
        volume = playerCamera.GetComponent<Volume>();
        currentOpenedMenu = mainMenu;

        listMenus = new List<GameObject>
        {
            mainMenu,
            keybindsSettingsMenu
        };
        
    }

    void Start()
    {
        HideAll();
    }

    void Update()
    {
        if (S_UserInput.instance.MenuOpenCloseInput && ableToOpenCloseMenu) 
        {
            if (!isOpen) // Ouvrir Menu
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

    public void ShowMenu(GameObject menu) //& Ouverture MainMenu
    {
        HideCurrent(false);
        menu.SetActive(true);
        currentOpenedMenu = menu;

        playerController.setMovementsEnabled(false);
        playerCamera.setCursorEnabled(true);
        playerCamera.setRotationEnabled(false);
        playerInteract.setInteractionEnabled(false);
        volume.enabled = true;
        isOpen = true;
    }

    public void HideAll() //& Fermeture le menu actuel
    {
        foreach (GameObject menu in listMenus)
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
