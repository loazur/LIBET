using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_PauseMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SettingsMenu settingsMenu;

    [Header("Boutons Pause Menu")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    protected override void OnEnable()
    {
        base.OnEnable(); // Utilise le OnEnable du parent

        S_HandlerPauseMenu.instance.DisableAll();
        S_HandlerPauseMenu.instance.setMenuOpened(true);
        S_HandlerPauseMenu.instance.setCurrentMenu(this);
    }

    public void OnContinueClicked()
    {
        S_HandlerPauseMenu.instance.CompletelyCloseMenu();
    }

    public void OnSettingsClicked()
    {
        settingsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnQuitClicked()
    {
        // Changement de scene
        S_SceneLoader.instance.LoadScene("MainMenu");
    }

}
