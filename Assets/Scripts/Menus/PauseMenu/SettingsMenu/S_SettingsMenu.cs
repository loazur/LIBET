using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_SettingsMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_PauseMenu pauseMenu;
    [SerializeField] private S_GameMenu gameMenu;
    [SerializeField] private S_AudioMenu audioMenu;
    [SerializeField] private S_VideoMenu videoMenu;
    [SerializeField] private S_CameraMenu cameraMenu;
    [SerializeField] private S_KeyboardMenu keyboardMenu;
    [SerializeField] private S_ControllerMenu controllerMenu;

    [Header("Boutons Settings Menu")]
    [SerializeField] private Button gameButton;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button videoButton;
    [SerializeField] private Button cameraButton;
    [SerializeField] private Button keyboardButton;
    [SerializeField] private Button controllerButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button leaveButton;

    private string currentSceneName;
    private S_MainMenu mainMenu;

    protected override void OnEnable()
    {
        base.OnEnable(); // Utilise le OnEnable du parent

        currentSceneName = SceneManager.GetActiveScene().name;
        
        //~ Exclusif à la scene MainMenu
        if (currentSceneName == "MainMenu")
        {
            // Chercher S_MainMenu dans les frères du parent (ou parent de parent)
            Transform parent = transform.parent;
            
            if (parent != null)
            {
                // Chercher dans les frères du parent
                mainMenu = parent.GetComponentInChildren<S_MainMenu>(true);
                
                // Si pas trouvé, remonter d'un niveau
                if (mainMenu == null && parent.parent != null)
                {
                    mainMenu = parent.parent.GetComponentInChildren<S_MainMenu>(true);
                }
            }
            
            // Fallback : chercher dans toute la scène
            if (mainMenu == null)
            {
                mainMenu = FindFirstObjectByType<S_MainMenu>();
                
                if (mainMenu == null)
                {
                    Debug.LogWarning("S_MainMenu not found in MainMenu scene!");
                }
            }
        }

        S_HandlerPauseMenu.instance.setCurrentMenu(this);
        S_HandlerPauseMenu.instance.setMenuOpened(true);
    }

    public void OnGameClicked()
    {
        gameMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnAudioClicked()
    {
        audioMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnVideoClicked()
    {
        videoMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnCameraClicked()
    {
        cameraMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnKeyboardClicked()
    {
        keyboardMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnControllerClicked()
    {
        controllerMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnReturnClicked()
    {
        if (currentSceneName != "MainMenu")
        {
            pauseMenu.ActivateMenu();
        }
        else // Scene MainMenu
        {
            mainMenu.ActivateMenu();
        }

        DeactivateMenu();

    }

    public void OnLeaveButton()
    {
        S_HandlerPauseMenu.instance.CompletelyCloseMenu();
    }

}
