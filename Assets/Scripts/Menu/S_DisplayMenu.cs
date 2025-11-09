using UnityEngine;
using UnityEngine.Rendering;

public class S_DisplayMenu : MonoBehaviour
{
    //~ Gestion ouverture du menu
    [SerializeField] private GameObject menuContainer;
    [SerializeField] private S_PlayerController playerController;
    [SerializeField] private S_FirstPersonCamera playerCamera;
    [SerializeField] private S_PlayerInteract playerInteract;
    private Volume volume;
    private bool isOpen = false;
    private bool ableToOpenCloseMenu = true;

    void Awake()
    {
        volume = playerCamera.GetComponent<Volume>();
    }

    void Start()
    {
        Hide();
    }

    void Update()
    {
        if (S_UserInput.instance.MenuOpenCloseInput && ableToOpenCloseMenu) 
        {
            if (!isOpen) // Ouvrir Menu
            {
                Show();
            }
            else // Fermer Menu
            {
                Hide();
            }
        }
        
    }

    //! --------------- Fonctions privés ---------------

    private void Show() //& Ouverture Menu
    {
        menuContainer.SetActive(true);
        playerController.setMovementsEnabled(false);
        playerCamera.setCursorEnabled(true);
        playerCamera.setRotationEnabled(false);
        playerInteract.setInteractionEnabled(false);
        volume.enabled = true;
        isOpen = true;
    }

    private void Hide() //& Fermeture Menu
    {
        menuContainer.SetActive(false);
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
