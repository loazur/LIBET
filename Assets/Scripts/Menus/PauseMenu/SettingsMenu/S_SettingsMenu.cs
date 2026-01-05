using UnityEngine;
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

    protected override void OnEnable()
    {
        base.OnEnable(); // Utilise le OnEnable du parent

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
        pauseMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnLeaveButton()
    {
        S_HandlerPauseMenu.instance.CompletelyCloseMenu();
    }

}
