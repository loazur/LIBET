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

        [Header("Fond du menu pause")]
        [SerializeField] private Image pauseMenuBackground;

    protected override void OnEnable()
    {
        base.OnEnable(); // Utilise le OnEnable du parent

        if (pauseMenuBackground != null)
            pauseMenuBackground.gameObject.SetActive(true);

        S_HandlerPauseMenu.instance.DisableAll();
        S_HandlerPauseMenu.instance.setMenuOpened(true);
        S_HandlerPauseMenu.instance.setCurrentMenu(this);
    }

    public void OnContinueClicked()
    {
        if (pauseMenuBackground != null)
            pauseMenuBackground.gameObject.SetActive(false);
        S_HandlerPauseMenu.instance.CompletelyCloseMenu();
    }

    public void OnSettingsClicked()
    {
        if (pauseMenuBackground != null)
            pauseMenuBackground.gameObject.SetActive(true);
        settingsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnQuitClicked()
    {
        if (pauseMenuBackground != null)
            pauseMenuBackground.gameObject.SetActive(false);

        // Remet l'écoulement du temps
        Time.timeScale = 1f;

        // Changement de scene
        S_SceneLoader.instance.LoadScene("MainMenu");
    }

}
