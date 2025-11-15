using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class S_DisplayMenus : MonoBehaviour
{
    //~ Références vers d'autre classes
    [SerializeField] private S_PlayerController playerController;
    [SerializeField] private S_PlayerCrouch playerCrouch;
    [SerializeField] private S_FirstPersonCamera playerCamera;
    [SerializeField] private S_PlayerInteract playerInteract;

    //~ Containers des menus / sous-menus
    [Header("Containers des différents menu")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject gameSettingsMenu;
    [SerializeField] private GameObject audioSettingsMenu;
    [SerializeField] private GameObject videoSettingsMenu;
    [SerializeField] private GameObject cameraSettingsMenu;
    [SerializeField] private GameObject keyboardSettingsMenu;
    [SerializeField] private GameObject controllerSettingsMenu;
    public enum MenuType // Tout les types de menus
    {
        Main,
        Settings,
        GameSettings,
        AudioSettings,
        VideoSettings,
        CameraSettings,
        KeyboardSettings,
        ControllerSettings
    }

    private Dictionary<MenuType, GameObject> menusList; // Lie l'enum à de vrai gameobjects
    private MenuType currentMenu; // Menu ouvert actuellement

    //?
    private Volume volume; // Floutage du background
    private bool isOpen = false;
    private bool ableToOpenCloseMenu = true;

    void Awake() //& Avant de tout cacher on initialise les variables
    {
        menusList = new Dictionary<MenuType, GameObject>
        {
            {MenuType.Main, mainMenu},
            {MenuType.Settings, settingsMenu},
            {MenuType.GameSettings, gameSettingsMenu},
            {MenuType.AudioSettings, audioSettingsMenu},
            {MenuType.VideoSettings, videoSettingsMenu},
            {MenuType.CameraSettings, cameraSettingsMenu},
            {MenuType.KeyboardSettings, keyboardSettingsMenu},
            {MenuType.ControllerSettings, controllerSettingsMenu}

        };

        volume = playerCamera.GetComponent<Volume>(); 
        currentMenu = MenuType.Main; // Le 1er menu qu'on va ouvrir c'est le mainMenu     
    }

    void Start() //& Cache tout les menus
    {
        HideAll();
    }

    void Update()
    {
        if (S_UserInput.instance.MenuOpenCloseInput && ableToOpenCloseMenu) // Quand on appuit sur la touche d'ouverture du menu
        {
            if (!isOpen) // Ouvrir MainMenu
            {
                ShowMenu((int)MenuType.Main);
            }
            else // Fermeture des menus en fonction de la hiérarchie
            {
                switch(currentMenu)
                {
                    case MenuType.Settings:
                        ShowMenu((int) MenuType.Main);
                        break;

                    case MenuType.GameSettings:
                    case MenuType.AudioSettings:
                    case MenuType.VideoSettings:
                    case MenuType.CameraSettings:
                    case MenuType.KeyboardSettings:
                    case MenuType.ControllerSettings:
                        ShowMenu((int) MenuType.Settings);
                        break;

                    default:
                        HideCurrent(true);
                        break;
                }
                
                
            }
        }
        
    }

    //! --------------- Fonctions principales ---------------

    public void ShowMenu(int indexMenu) //& Ouvre le menu pris en paramètre (indexMenu est basé sur l'index dans l'enum)
    {
        HideCurrent(false); // Cache l'ancien

        menusList[(MenuType) indexMenu].SetActive(true); // Active le nouveau
        currentMenu = (MenuType) indexMenu; // Et l'assigne à la variable

        //
        playerController.setMovementsEnabled(false);
        playerCamera.setCursorEnabled(true);
        playerCamera.setRotationEnabled(false);
        playerInteract.setInteractionEnabled(false);
        playerCrouch.setAbleToCrouch(false);
        volume.enabled = true;
        isOpen = true;
    }

    public void HideCurrent(bool closingMenu) //& Fermeture du menu actuel
    {
        menusList[currentMenu].SetActive(false);

        if (closingMenu) // SI on ferme le menu 
        {
            playerController.setMovementsEnabled(true);
            playerCamera.setCursorEnabled(false);
            playerCamera.setRotationEnabled(true);
            playerInteract.setInteractionEnabled(true);
            playerCrouch.setAbleToCrouch(true);
            volume.enabled = false;
            isOpen = false;
        }
    }

    public void HideAll() //& Fermeture tout les menus
    {
        foreach (GameObject menu in menusList.Values) // Désactive tout les menus
        {
            menu.SetActive(false);
        }

        playerController.setMovementsEnabled(true);
        playerCamera.setCursorEnabled(false);
        playerCamera.setRotationEnabled(true);
        playerInteract.setInteractionEnabled(true);
        playerCrouch.setAbleToCrouch(true);
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
